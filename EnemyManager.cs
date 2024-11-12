using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Player player;
    public EnemyActivator[] enemies; // 유효한 적 저장 배열
    public float checkInterval = 0.5f; // 체크 간격 (초 단위)

    // Start is called before the first frame update
    void Start()
    {
        if(player == null) {
            player = FindObjectOfType<Player>();
        }
        // 모든 적 오브젝트를 배열에 저장
        enemies = FindObjectsOfType<EnemyActivator>();
        StartCoroutine(CheckEnemiesDistance());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CheckEnemiesDistance()
    {
        while (true)
        {
            // 유효한 적을 남기기 위해 List로 필터링
            List<EnemyActivator> validEnemies = new List<EnemyActivator>();

            foreach (EnemyActivator enemy in enemies)
            {
                if (enemy != null) {
                    validEnemies.Add(enemy);
                }
            }

            enemies = validEnemies.ToArray();

            foreach (EnemyActivator enemy in enemies) {
                enemy.CheckDistance(player.transform.position);
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
}
