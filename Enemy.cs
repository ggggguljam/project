using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Player player;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    BoxCollider2D attackCollider;
    CapsuleCollider2D capsuleCollider;
    public AudioClip attackSound;
    public AudioClip damageSound;
    private AudioSource audioSource;
    public int hp = 10;
    public int nextMove;
    private float damageCooldown = 0.3f; // 피해를 입고 다시 받을 수 있는 쿨타임 시간
    private float lastDamageTime; // 마지막으로 피해를 입은 시간
    public float detectionRadius = 10f;
    public float attackRange = 1f;
    public float thinkIntervalMin = 5f;
    public float thinkIntervalMax = 30f;
    public float attackDelay = 1f;
    public float attackDuration = 0.6f;
    public int attackDamage = 1;
    private bool canAttack = true;
    bool isChasing = false;
    bool isDead = false;
    private bool onFire = false;
    private Color fireColor = new Color(1f, 0.537f, 0.227f);
    private Renderer objrenderer;
    private bool isExploded = false;
    [SerializeField] private float verticalTolerance = 1.5f; // 수직 허용 범위
    public bool isCollidingWithPlayer = false;
    public Vector2 playerNormalSpeed;
    private float lastXPosition;            // 이전 x 위치를 저장하는 변수
    private float lastChangedTime;          // 마지막으로 x 위치가 변경된 시간
    private float delayTime = 1f;         // 지연 시간


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        player = FindObjectOfType<Player>();
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.enabled = false;
        objrenderer = GetComponent<Renderer>();
        
        Think();
        Invoke("Think", thinkIntervalMin);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        float horizontalDistanceToPlayer = player != null ? Mathf.Abs(player.transform.position.x - transform.position.x) : float.MaxValue;
        float verticalDistanceToPlayer = player != null ? Mathf.Abs(player.transform.position.y - transform.position.y) : float.MaxValue;

        if (!isCollidingWithPlayer) {
            if (player != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
                 //범위 내이면 플레이어 추적 시작
                if (horizontalDistanceToPlayer <= detectionRadius) {
                    isChasing = true;
                    ChasePlayer();

                   
                    if (horizontalDistanceToPlayer <= attackRange && verticalDistanceToPlayer <= verticalTolerance)
                        AttackPlayer();
                }
                else {
                    if (isChasing) {
                        // 추적 상태에서 다시 원래 상태로 돌아옴
                        isChasing = false;
                        spriteRenderer.flipX = nextMove == 1; // 이동 방향에 따라 스프라이트 반전
                    }
                    rigid.velocity = new Vector2(nextMove * 5, rigid.velocity.y);
                    anim.SetBool("IsMoving", nextMove != 0);
                }
            }
        }
        else {
            if (horizontalDistanceToPlayer <= attackRange && verticalDistanceToPlayer <= verticalTolerance)
                AttackPlayer();
        }

        // 현재 x 위치 가져오기
        float currentXPosition = transform.position.x;

        // x 위치가 변경되었는지 확인함. 미세한 변화도 감지
        if (Mathf.Abs(currentXPosition - lastXPosition) > Mathf.Epsilon && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            // x 위치가 변경되었으면 시간을 갱신하고 위치 저장
            lastChangedTime = Time.time;
            lastXPosition = currentXPosition;
        }
        else
        {
            // x 위치가 변경되지 않았고, 일정시간이 경과했으면 메서드 호출
            if (Time.time - lastChangedTime >= delayTime && nextMove !=0)
            {
                Turn();
                lastChangedTime = Time.time;  // 메서드를 실행한 시점으로 시간 갱신
            }
        }

        if (onFire) {
            if (hp > 0) {
                objrenderer.material.color = fireColor;
            }
            else {
                StartCoroutine(Die());
            }
        }

        if(isExploded) {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // 바닥 체크 (낭떠러지 감지)
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y - 0.6f);
        Debug.DrawRay(frontVec, Vector2.right, new Color(1, 1, 1));
        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 5.0f, LayerMask.GetMask("Floor", "BlockableObject"));
        if (rayHit.collider == null) {
            Turn();
        }

        if (onFire) {
            if (hp > 0) {
                hp--;
            }
        }
    }


    void Think()
    {
        //다음 행동 설정: -1(왼쪽 이동) 0(정지) 1(오른쪽 이동)
        nextMove = UnityEngine.Random.Range(-1, 2);

        if (nextMove != 0)
            anim.SetBool("IsMoving", true);
        else  {
            anim.SetBool("IsMoving", false);
            rigid.velocity = Vector2.zero;
        }

        //스프라이트 반전
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove != 1;

        //일정 범위 내에 생각할 시간 설정
        float nextThinkTime = UnityEngine.Random.Range(thinkIntervalMin, thinkIntervalMax);
        Invoke("Think", nextThinkTime);
    }



    void SetNextMove()
    {
        nextMove = UnityEngine.Random.Range(-1, 2);   // -1 (왼쪽), 0 (정지), 1 (오른쪽)
        anim.SetBool("IsMoving", nextMove != 0);

        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove != 1;
        CancelInvoke();
        Invoke("Think", thinkIntervalMin);
    }

    public void TakeDamage(int damage)
    {
        // 데미지 간격을 체크하여 쿨타임이 지나지 않았으면 리턴
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time; // 마지막으로 데미지를 입은 시간 갱신

        // 공격 중이면 공격 콜라이더 비활성화
        if (attackCollider.enabled)
            attackCollider.enabled = false;

        hp -= damage;
        if (hp <= 0)
            StartCoroutine(Die());
        else
            anim.SetTrigger("TakeHit");

        if (damageSound != null)
            SoundManager.instance.PlaySoundForce(damageSound);
    }

    private IEnumerator Die()
    {
        isDead = true;
        attackCollider.enabled = false;
        nextMove = 0;


        if (anim != null) {
            yield return new WaitForSeconds(0.2f);
            anim.SetTrigger("Death");
            anim.SetBool("IsMoving", false);
            float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, animationLength + 5);
        }

        if (capsuleCollider != null) {
            capsuleCollider.enabled = false;
        }

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");


        if (rigid != null) {
            rigid.gravityScale = 0;
            rigid.velocity = Vector2.zero;
            rigid.isKinematic = true;
        }

        float currentScore = 0f;
        if (gameObject.CompareTag("Goblin"))
            currentScore = 100f;
        else if (gameObject.CompareTag("Flying eye"))
            currentScore = 115f;
        else if (gameObject.CompareTag("Mushroom"))
            currentScore = 200f;
        else if (gameObject.CompareTag("Skeleton"))
            currentScore = 300f;

        //상태에 따라 추가점수
        float randomValue = UnityEngine.Random.Range(1.0f, 1.5f);
        if (onFire) currentScore *= randomValue;
        if (isExploded) currentScore *= (randomValue + UnityEngine.Random.Range(0.1f, 0.7f));

        player.score += Mathf.FloorToInt(currentScore);
    }


    void ChasePlayer()
    {
        // 플레이어의 수직 위치와의 차이를 확인
        float verticalDistanceToPlayer = player != null ? Mathf.Abs(player.transform.position.y - transform.position.y) : float.MaxValue;

        // 수직 거리가 허용 범위 내일 경우에만 수평 추적
        if (verticalDistanceToPlayer <= verticalTolerance) {
            Vector2 direction = new Vector2(player.transform.position.x - transform.position.x, 0).normalized;
            rigid.velocity = new Vector2(direction.x * 5, rigid.velocity.y);

            anim.SetBool("IsMoving", true);

            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
        else {
            // 수직 위치가 허용 범위를 초과할 경우 수평 속도를 0으로 설정하여 추적 중지
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            anim.SetBool("IsMoving", false);
        }
    }

    void AttackPlayer()
    {
        if (canAttack)
            StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        if (player != null) {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange) {
                anim.SetTrigger("Attack");
                anim.SetBool("IsMoving", false);

                rigid.velocity = Vector2.zero;

                if (spriteRenderer.flipX == false)
                    attackCollider.offset = new Vector2(1.7f, -0.5f);
                else
                    attackCollider.offset = new Vector2(-1.7f, -0.5f);

                if (attackSound != null)
                    SoundManager.instance.PlaySoundForce(attackSound);
            }
        }

        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    // 애니메이션과 연결되어 실행
    void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    // 애니메이션과 연결되어 실행
    void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //피해
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            if (player != null && anim.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
                player.TakeDamage(attackDamage);
            }
        }

        if(other.CompareTag("Burnable")) {
            Campfire campfire = other.GetComponent<Campfire>();

            if (campfire != null) {
                onFire = false;
            }
            else {
                if (other is CapsuleCollider2D) {
                    onFire = true;
                }

            }
        }

        if(other.CompareTag("Explosive")) {
            if(other is CapsuleCollider2D) {
                isExploded = true;
                TakeDamage(1000);
            }
        }

        if(other.CompareTag("GGASIE")) {
            TakeDamage(50);
        }
    }

    //플레이어와 충돌 중이면 플레이어 밀림 방지
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            isCollidingWithPlayer = true;
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            playerNormalSpeed = player.GetComponent<Rigidbody2D>().velocity;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            isCollidingWithPlayer = false;
            isChasing= true;
            player.GetComponent<Rigidbody2D>().velocity = playerNormalSpeed;
        }
    }
}
