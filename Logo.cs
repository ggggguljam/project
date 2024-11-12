using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Logo : MonoBehaviour
{
    public float fadeDuration = 2.0f;   // 페이드 인/아웃 시간
    public float displayTime = 3.0f;    // 로고 화면을 보여줄 시간
    public TMP_Text text;
    public string nextSceneName = "Title";
    public bool fadeIn = true;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeTextAlpha());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FadeTextAlpha()
    {
        float timer = 0f;
        Color originalColor = text.color;

        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
