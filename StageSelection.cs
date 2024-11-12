#if UNITY_EDITOR
using UnityEditor.Rendering;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using TMPro;

public class StageSelection : MonoBehaviour
{
    public GameObject loadingScreen;
    public Button titleButton;
    public Button stage1Button;
    public Button stage2Button;
    public Button stage3Button;
    public SaveLoadManager saveLoadManager;
    public TMP_Text stage1ScoreText;
    public TMP_Text stage1TimeText;
    public TMP_Text stage2ScoreText;
    public TMP_Text stage2TimeText;
    public TMP_Text stage3ScoreText;
    public TMP_Text stage3TimeText;
    public GameObject canvas1;
    public GameObject canvas2;
    public Button nextButton;
    public Button backButton;


    // Start is called before the first frame update
    void Start()
    {
        titleButton.onClick.AddListener(BackToTitle);
        stage1Button.onClick.AddListener(() => StartStage("Stage1"));
        stage2Button.onClick.AddListener(() => StartStage("Stage2"));
        stage3Button.onClick.AddListener(() => StartStage("Stage3"));
        nextButton.onClick.AddListener(Next);
        backButton.onClick.AddListener(Back);

        loadingScreen.SetActive(false);

        if (stage1ScoreText != null && stage1TimeText != null)
            DisplayStageData("Stage1", stage1ScoreText, stage1TimeText);
        if (stage2ScoreText != null && stage2TimeText != null)
            DisplayStageData("Stage2", stage2ScoreText, stage2TimeText);
        if (stage3ScoreText != null && stage3TimeText != null)
            DisplayStageData("Stage3", stage3ScoreText, stage3TimeText);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DisplayStageData(string stageName, TMP_Text scoreText, TMP_Text timeText)
    {
        StageData stageData = saveLoadManager.GetStageData(stageName);

        if (stageData != null)
        {
            string scoreString = stageData.bestScore.ToString("D10");
            scoreText.text = "최고 점수: " + scoreString;
            int minutes = Mathf.FloorToInt(stageData.bestTime / 60F);
            int seconds = Mathf.FloorToInt(stageData.bestTime % 60F);
            timeText.text = "시간: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            int score = 0;
            scoreText.text = "최고 점수: " + score.ToString("D10");
            timeText.text = "최단 시간: --";
        }
    }

    public void StartStage(string stageName)
    {
        Debug.Log($"StartStage called for: {stageName}");

        SoundManager.instance.StopBGM();

        if (!loadingScreen.activeSelf)
        {
            Debug.Log("Activating loadingScreen...");
            loadingScreen.SetActive(true);
        }
        else
        {
            Debug.Log("loadingScreen was already active.");
        }


        if (!canvas1.activeSelf)
            canvas1.SetActive(true);

        StartCoroutine(StartGameWithLoading(stageName));
    }

    IEnumerator StartGameWithLoading(string stageName)
    {
        yield return new WaitForSeconds(0.5f);

        // 비동기로 화면 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(stageName);
        asyncLoad.allowSceneActivation = false; // 화면 로드 완료 후 자동 활성화 방지

        // 로딩 상태 체크
        while (!asyncLoad.isDone)
        {
            yield return null;

            // 로딩이 완료되면 활성화
            if (asyncLoad.progress >= 0.9f)
                asyncLoad.allowSceneActivation = true;
        }
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Main");
    }

    public void Next()
    {
        canvas1.SetActive(false);
        canvas2.SetActive(true);
    }

    public void Back()
    {
        canvas1.SetActive(true);
        canvas2.SetActive(false);
    }
}
