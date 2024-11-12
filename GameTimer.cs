using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timeText;
    public float gameTime = 0f; // 게임 플레이 시간

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 플레이 시간 증가
        gameTime += Time.deltaTime;

        // 시간을 "분:초" 형식으로 변환
        int minutes = Mathf.FloorToInt(gameTime / 60F);
        int seconds = Mathf.FloorToInt(gameTime % 60F);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
