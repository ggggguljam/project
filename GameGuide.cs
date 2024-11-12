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

public class GameGuide : MonoBehaviour
{
    public Button titleButton1;
    public Button titleButton2;
    public Button nextButton;
    public Button backButton;

    public GameObject canvas1;
    public GameObject canvas2;

    // Start is called before the first frame update
    void Start()
    {
        titleButton1.onClick.AddListener(BackToTitle);
        titleButton2.onClick.AddListener(BackToTitle);
        nextButton.onClick.AddListener(Next);
        backButton.onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Update()
    {

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

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
