using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string stageName; // 플레이한 스테이지 이름
    public int bestScore; // 스테이지 최고 점수
    public float bestTime; // 스테이지 최단 시간
}


[System.Serializable]
public class GameData
{
    public List<StageData> stages = new List<StageData>();
}