using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string filePath;
    public GameData gameData = new GameData();

    void Awake()
    {
        filePath = Application.persistentDataPath + "/GameData.json";
        LoadData();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData()
    {
        // gameData를 JSON 문자열로 변환하여 파일에 저장
        string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Data saved to " + filePath);
    }

    public void LoadData()
    {
        // 파일이 존재하는지 확인하고, 존재하면 데이터 로드
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(jsonData);
            Debug.Log("Data loaded from " + filePath);
        }
        else
        {
            Debug.Log("Save file not found, creating a new one.");
            gameData = new GameData(); // 없으면 초기화
        }
    }

    public StageData GetStageData(string stageName)
    {
        return gameData.stages.Find(stage => stage.stageName == stageName);
    }

    //새로운 데이터 업데이트
    public void UpdateStageData(string stageName, int score, float time)
    {
        StageData stageData = GetStageData(stageName);

        if (stageData == null)
        {
            stageData = new StageData { stageName = stageName, bestScore = score, bestTime = time };
            gameData.stages.Add(stageData);
        }
        else
        {
            if (score > stageData.bestScore) stageData.bestScore = score;
            if (time < stageData.bestTime || stageData.bestTime == 0) stageData.bestTime = time;
        }

        SaveData();
    }
}
