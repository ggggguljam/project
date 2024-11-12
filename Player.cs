using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public GameManager gameManager;

    public int hp = 100;
    public int score = 0;
    public float maxSpeed = 5.0f;

    public float jumpPower = 17.0f;
    public float jumpMultiplier = 2.5f; // 점프 중 가속 비율
    public float fallMultiplier = 3.5f; // 낙하 중 가속 비율
    public bool isGrounded = true;
    public float groundCheckRadius = 0.2f; // 바닥 체크 범위
    public Transform groundCheck; // 플레이어의 발 부분 위치
    public LayerMask groundLayer; // 바닥 레이어
    public float attackDelay = 0.2f;
    public float attackDuration = 0.5f;
    public bool canAttack = true;
    private float damageCooldown = 0.3f; // 대미지를 입고 나서 다시 입을 때까지의 쿨다운 시간
    private float lastDamageTime; // 마지막으로 대미지를 입은 시간;
    private bool isDead = false;
    public int fireItem = 0;
    public int waterItem = 0;
    
    //상호작용
    public bool fireNearby = false;
    private int fireResist = 25;
    private bool onGGASIE = false;
    private bool colorChanged = false;
    private Color fireColor = new Color(1f, 0.537f, 0.227f);
    private Color originColor;
    private Renderer objRenderer;

    Rigidbody2D rigid;
    CapsuleCollider2D capsuleCollider;
    BoxCollider2D attackCollider;
    Animator anim;
    SpriteRenderer spriteRenderer;
    Player player;
    public AudioClip attackSound; 
    public AudioClip jumpSound;
    public AudioClip damageSound;
    private AudioSource audioSource;
    private bool jumpRequested = false;
    private Vector2 moveDirection;    // 이동 방향 벡터
    private bool isMoving;            // 이동 중 여부

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.enabled = false;
        audioSource = GetComponent<AudioSource>();
        lastDamageTime = -damageCooldown; // 피해 입은 시간초기화

        objRenderer = GetComponent<Renderer>();
        originColor = objRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        // 이동 입력 확인 및 방향 설정
        if (Input.GetKey(KeyCode.RightArrow) && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
            moveDirection = Vector2.right;
            transform.localScale = new Vector3(1, 1, 1); // 캐릭터 방향 설정
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
            moveDirection = Vector2.left;
            transform.localScale = new Vector3(-1, 1, 1); // 캐릭터 방향 설정
            isMoving = true;
        }
        else {
            moveDirection = Vector2.zero;
            isMoving = false;
        }

        // 애니메이션 설정
        anim.SetBool("IsMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.A) && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack") && !anim.GetBool("IsJumping") && !anim.GetBool("IsFalling") && canAttack) {
            if (attackCollider.enabled)
                attackCollider.enabled = false;
            Attack();
        }

        // 바닥에 있는지 여부를 확인
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 점프 애니메이션 실행
        if (isGrounded && !anim.GetBool("IsJumping") && !anim.GetBool("IsFalling") && Input.GetKeyDown(KeyCode.Space)) {
            if (jumpSound != null && audioSource != null)
                audioSource.PlayOneShot(jumpSound);

            canAttack = false;
            jumpRequested = true;
        }

        if (rigid.velocity.y > 0.5f && !isGrounded) {
            anim.SetBool("IsJumping", true);
            anim.SetBool("IsFalling", false);
        }
        else if (rigid.velocity.y < -0.5f && !isGrounded) {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", true);
        }
        else if(isGrounded) {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
            canAttack = true;
        }
        
        if(fireNearby) {
            if(!colorChanged) {
                objRenderer.material.color = fireColor;
                colorChanged = true;
            }
        }
        
        if(!fireNearby) {
            if(colorChanged) {
                objRenderer.material.color = originColor;
                colorChanged = false;
            }
        }
    }


    void FixedUpdate()
    {
        //이동
        if (isMoving) {
            transform.Translate(moveDirection * maxSpeed * Time.fixedDeltaTime);
        }

        //점프
        if (jumpRequested) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpRequested = false;
        }
        // 점프 중일 때 추가 가속 적용
        if (rigid.velocity.y > 0.5f) {
            rigid.velocity += Vector2.up * Physics2D.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        // 낙하 중일 때 추가 가속 적용
        else if (rigid.velocity.y < -0.5f && !isGrounded) {
            rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        if(fireNearby) {
            if(fireResist > 0) {
                fireResist--;
            }
            else {
                hp--;
                fireResist = 25;
            }
        }
        
        if(!fireNearby){
            fireResist = 25;
        }
    }


    void Attack()
    {
        anim.SetBool("IsMoving", false);
        anim.SetTrigger("Attack");

        if (spriteRenderer.flipX == false)
            attackCollider.offset = new Vector2(1.7f, 0.3f);
        else
            attackCollider.offset = new Vector2(-1.7f, 0.3f);

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        canAttack = false; // 공격 중에는 추가 공격을 막음
    }

    // 애니메이션과 연결되어 실행
    void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    void DisableAttackCollider()
    {
        attackCollider.enabled = false;
        Invoke("ResetAttackCooldown", attackDelay); // 공격 쿨다운 시작
    }

    void ResetAttackCooldown()
    {
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (attackCollider.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
                enemy.TakeDamage(10);
            }
        }


        //클리어
        if (collision.CompareTag("Finish")) {
            gameManager.ShowGameClear();
        }

        if (collision.IsTouching(capsuleCollider)) {
            if (collision.CompareTag("GGASIE")) {
                if (!onGGASIE) {
                    TakeDamage(40);
                    audioSource.PlayOneShot(damageSound);
                    Debug.Log("Ouch!");
                    onGGASIE = true;
                }
            }
        }

        if(collision is CapsuleCollider2D) {
            if(collision.IsTouching(capsuleCollider)) {
                if(collision.CompareTag("Burnable")) {
                    fireNearby = true;
                }

                if(collision.CompareTag("Explosive")) {
                    TakeDamage(90);
                }
            }
        }

    }

    public void TakeDamage(int damage)
    {

        // 쿨다운 시간 내에는 피해를 입지 않도록 함
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time; // 마지막으로 피해를 입은 시간 갱신

        // 공격 중에 피해 시 공격 콜라이더 비활성화
        if (attackCollider.enabled) {
            attackCollider.enabled = false;
        }

        canAttack = true;

        hp -= damage;

        if (hp <= 0)
            Die();
        else
            anim.SetTrigger("TakeHit");
        
        //현재 체력 갱신
        if (gameManager != null)
            gameManager.DecreaseHP(damage);

    }

    private IEnumerator PlayHitAnimationWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("TakeHit");
    }


    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 플레이어 사망 처리
        attackCollider.enabled = false;

        if (player != null)
            player.enabled = false;

        // 사망 애니메이션 재생
        if (anim != null) {
            anim.SetTrigger("Death");
            anim.SetBool("IsMoving", false);
            float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
        }

        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");


        if (rigid != null)
        {
            rigid.gravityScale = 0;
            rigid.velocity = Vector2.zero;
            rigid.isKinematic = true;
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision is CapsuleCollider2D) {    
            if(collision.CompareTag("Burnable")) {
                    fireNearby = false;
            }
        }

        if(collision.CompareTag("GGASIE")) {
            onGGASIE = false;
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = new Vector2(0f, 0f);
    }
}