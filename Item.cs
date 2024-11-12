using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Animator anim;
    Player player;
    CircleCollider2D circleCollider;
    public AudioClip boxOpenSound;
    public AudioClip bagOpenSound;
    public int rechargeThreshold = 7;

    public bool hasPassed = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasPassed)
        {
            CapsuleCollider2D capsule = player.GetComponent<CapsuleCollider2D>();
            if (capsule != null && capsule.IsTouching(circleCollider))
            {
                if (player.fireItem < rechargeThreshold || player.waterItem < rechargeThreshold)
                {
                    if (gameObject.CompareTag("ItemChest"))
                    {
                        player.fireItem = rechargeThreshold;
                        player.waterItem = rechargeThreshold;
                        anim.SetBool("IsOpened", true);
                        SoundManager.instance.PlaySound(boxOpenSound);
                    }

                    if (gameObject.CompareTag("ItemBag"))
                    {
                        player.fireItem = rechargeThreshold;
                        player.waterItem = rechargeThreshold;
                        anim.SetBool("IsOpened", true);
                        SoundManager.instance.PlaySound(bagOpenSound);
                    }
                }

                hasPassed = true;
            }
        }
    }
}
