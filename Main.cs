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
using System.Security.Cryptography.X509Certificates;

public class Main : MonoBehaviour
{
    public Button gamestartButton;
    public Button instructionButton;
    public Button quitButton;
    public Button creditsButton;
    
    // Start is called before the first frame update
    void Start()
    {
        gamestartButton.onClick.AddListener(StartGame);
        instructionButton.onClick.AddListener(Guide);
        quitButton.onClick.AddListener(QuitGame);
        creditsButton.onClick.AddListener(Credits);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        SceneManager.LoadScene("StageSelection");
    }

    public void Guide()
    {
        SceneManager.LoadScene("GameGuide");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();

        // 에디터에서 종료 기능 테스트
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
