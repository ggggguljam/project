using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform Surae; //스테이지 시작 위치
    public Transform Finish; //스테이지 끝 위치
    public Transform player;

    private float startPosX = 4f;      // 배경의 시작 X 위치
    private float endPosX = -4f;       // 배경의 끝 X 위치
    private float smoothFactor = 1000;   // 부드러운 이동을 위한 분할

    private Vector3 initialBackgroundPos; // 배경의 초기 위치

    void Start()
    {
        initialBackgroundPos = transform.localPosition;
    }

    void Update()
    {
        float A = Finish.position.x - Surae.position.x; //Surae GameOver 사이의 X축 거리

        float B = player.position.x - Surae.position.x; //Surae 플레이어 사이의 X축 거리

        float ratio = Mathf.Clamp(B / A, 0f, 1f); // B/A 비율 계산

        float targetX = Mathf.Lerp(startPosX, endPosX, ratio); //배경 위치를 부드럽게 이동 (0일 때 4, 1일 때 -4)

        Vector3 newBackgroundPos = initialBackgroundPos;
        newBackgroundPos.x = targetX; // 배경의 X축 위치만 업데이트 (Y, Z는 초기 위치 고정)

        transform.localPosition = Vector3.Lerp(transform.localPosition, newBackgroundPos, Time.deltaTime * smoothFactor);  // 배경의 X축 위치를 업데이트 (부드러운 이동을 위해 Lerp 사용)
    }
}
