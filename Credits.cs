using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public RectTransform creditText; // 크레딧 텍스트의 RectTransform
    public string nextSceneName;
    public float scrollSpeed = 50f;

    private Vector3 startPosition;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = creditText.localPosition;  // 시작 위치 저장
    }

    // Update is called once per frame
    void Update()
    {
        creditText.localPosition += new Vector3(0, scrollSpeed * Time.deltaTime, 0);

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(nextSceneName);

        //스크롤이 끝나면 다음 화면으로 전환
        if (creditText.localPosition.y >= Screen.height + creditText.rect.height)
            StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(7f);

        SceneManager.LoadScene(nextSceneName);
    }
}
