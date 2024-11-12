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

public class GameManager : MonoBehaviour
{
    public Player player;
    public TMP_Text hp;
    public TMP_Text score;
    public TMP_Text gameOverScoreText;
    public Image hpbar;
    public GameObject gameOverPanel;
    public Button retryButton;
    public Button quitButton1;
    public AudioClip gameOverSound;
    public AudioClip gamePauseSound;
    public AudioClip gameResumeSound;
    public AudioClip gameClearSound;
    public GameObject gamePausePanel;
    public Button resumeButton;
    public Button titleButton1;
    public Button quitButton2;
    public AudioClip bgmClip;
    public bool isGamePaused = false;
    public bool isGameCleared = false;
    public GameObject gameClearPanel;
    public Button reStartButton;
    public Button quitButton3;
    public Button titleButton2;
    public TMP_Text timeText;
    public TMP_Text clearMessage;
    public GameTimer gameTimer;
    public GameObject loadingScreen;
    public TMP_Text fireItemText;
    public TMP_Text waterItemText;
    public SaveLoadManager saveLoadManager;

    // Start is called before the first frame update
    void Start()
    {
        gameOverPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gameClearPanel.SetActive(false);
        retryButton.onClick.AddListener(RestartGame);
        reStartButton.onClick.AddListener(RestartGame);
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton1.onClick.AddListener(QuitGame);
        quitButton2.onClick.AddListener(QuitGame);
        quitButton3.onClick.AddListener(QuitGame);
        titleButton1.onClick.AddListener(GoToTitle);
        titleButton2.onClick.AddListener(GoToTitle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (player != null)
        {
            if (player.hp > 0)
                hp.text = "HP : " + player.hp + " / 100";
            else
            {
                hp.text = "HP : 0 / 100";
                StartCoroutine(GameOver());
            }

            score.text = player.score.ToString("D10");

            if (hpbar != null)
                hpbar.fillAmount = player.hp / 100f;

            if (fireItemText != null)
                fireItemText.text = "x " + player.fireItem.ToString();

            if (waterItemText != null)
                waterItemText.text = "x " + player.waterItem.ToString();
        }

        
    }

    public void DecreaseHP(int damage)
    {
        if (player != null) {
            player.TakeDamage(damage);

            // 이미지 업데이트
            if (hpbar != null)
                hpbar.fillAmount = player.hp / 100f;
        }
    }

    public void ShowGameClear()
    {
        // 스테이지를 클리어 했으면 애니메이션을 기본 상태로 전환
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null) {
            playerAnimator.SetBool("IsMoving", false);
            playerAnimator.SetBool("IsJumping", false);
            playerAnimator.SetBool("IsFalling", false);
        }

        StartCoroutine(GameClear());
    }

    IEnumerator GameClear()
    {

        if (player != null)
            player.enabled = false;

        float finalTime = gameTimer.gameTime; // 걸린 시간
        int finalScore = player.score;

        string currentStageName = SceneManager.GetActiveScene().name;

        if (saveLoadManager != null)
            saveLoadManager.UpdateStageData(currentStageName, finalScore, finalTime);

        yield return new WaitForSeconds(3.0f);

        SoundManager.instance.StopBGM();
        SoundManager.instance.StopLoopSound();

        if (gameClearSound != null)
            SoundManager.instance.PlaySoundForce(gameClearSound);

        gameClearPanel.gameObject.SetActive(true);

        Time.timeScale = 0f;

        // 시간을 "분:초" 형식으로 변환
        int minutes = Mathf.FloorToInt(finalTime / 60F);
        int seconds = Mathf.FloorToInt(finalTime % 60F);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        string scoreString = finalScore.ToString("D10");

        clearMessage.text = $"Your Score\n {scoreString}\nYour Time\n {timeString}";
    }

    IEnumerator GameOver()
    {
        SoundManager.instance.StopBGM();
        SoundManager.instance.StopLoopSound();

        yield return new WaitForSeconds(3.0f);
     
        if (gameOverSound != null) 
            SoundManager.instance.PlaySoundForce(gameOverSound);

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = "Your Score \n" + player.score;
    }

    IEnumerator GoToTitleWithLoading()
    {
        SoundManager.instance.StopBGM();
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Title");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 게임 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 스테이지 재시작
                                        
        // 게임 재시작 시 배경음악을 처음부터 재생
        if (SoundManager.instance != null)
            SoundManager.instance.PlayBGM(bgmClip, true); 
    }

    public void ResumeGame()
    {
        if (gameResumeSound != null)
            SoundManager.instance.PlaySoundForce(gameResumeSound);

        SoundManager.instance.ResumeBGM();
        SoundManager.instance.ResumeLoopSound();

        gamePausePanel.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
        isGamePaused = false;
        SoundManager.instance.SetGamePaused(gamePauseSound);
    }

    void PauseGame()
    {
        SoundManager.instance.PauseBGM();

        if (gamePauseSound != null)
            SoundManager.instance.PlaySoundForce(gamePauseSound);

        SoundManager.instance.PauseLoopSound();

        gamePausePanel.SetActive(true);
        Time.timeScale = 0f; // 게임 시간 멈춤
        isGamePaused = true;
        SoundManager.instance.SetGamePaused(gamePauseSound); 
    }

    public void GoToTitle()
    {
        SoundManager.instance.StopBGM();

        Time.timeScale = 1f; // 게임 재개
        StartCoroutine(GoToTitleWithLoading());
    }

    public void QuitGame()
    {
        Application.Quit(); // 게임 종료

        //에디터에서 종료 기능 실행
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
