using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    public Player player;
    public float activationDistance = 50f; // 활성화 거리

    // Start is called before the first frame update
    void Start()
    {
        if(player == null) {
            player = FindObjectOfType<Player>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckDistance(Vector2 playerPosition)
    {
        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, playerPosition);

        // 거리 기반 활성화/비활성화
        if (distance <= activationDistance && !gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }
        else if (distance > activationDistance && gameObject.activeInHierarchy) {
            gameObject.SetActive(false);
        }
    }
}
