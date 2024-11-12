using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public Player player;
    public ObjectActivator[] objects; //오브젝트 배열
    public float checkInterval = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null) {
            player = FindObjectOfType<Player>();
        }
        // 모든 오브젝트를 배열에 저장
        objects = FindObjectsOfType<ObjectActivator>();
        StartCoroutine(CheckObjectsDistance());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CheckObjectsDistance()
    {
        while (true)
        {
            // 유효한 오브젝트만 남기기 위해 List로 필터링
            List<ObjectActivator> validObjects = new List<ObjectActivator>();

            foreach (ObjectActivator objectActivator in objects) {
                if (objectActivator != null) {
                    validObjects.Add(objectActivator);
                }
            }

            objects = validObjects.ToArray();

            foreach (ObjectActivator objectActivator in objects) {
                objectActivator.CheckDistance(player.transform.position);
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
}
