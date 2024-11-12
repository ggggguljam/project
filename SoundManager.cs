using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // 싱글톤 인스턴스
    public AudioSource soundEffectSource; // 효과음을 재생하는 오디오 소스
    public AudioSource loopingEffectSource; // 반복 재생되는 효과음을 재생하는 오디오 소스
    public AudioSource bgmSource;    // 배경음악을 재생하는 오디오 소스
    public AudioClip[] sceneBGMs;    // 각 씬에 맞는 배경음악을 저장할 배열
    public AudioClip currentBGMClip; // 현재 재생 중인 배경음악
    public List<BurnableObject> activeBurnableObjects = new List<BurnableObject>(); // 활성화된 BurnableObject 저장 리스트
    public float maxDistance = 30f; // 효과음이 들리는 최대 거리
    public Player player;
    private bool isGamePaused = false;

    private void Awake()
    {
        // 싱글톤 패턴
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // 화면 전환 시 파괴되지 않도록 설정
        }
        else
            Destroy(gameObject); // 이미 SoundManager가 존재하면 새로 생성된 객체 파괴

        if (instance == null)
            Debug.LogError("SoundManager instance is missing!");

        // 화면 로드 완료 시 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayBGMForCurrentScene();
    }

    // Update is called once per frame
    void Update()
    {
        AdjustSoundVolume();
    }

    private void OnDestroy()
    {
        // 화면 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 화면이 로드될 때마다 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForCurrentScene();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (player == null && currentSceneIndex == 4 || currentSceneIndex == 5) {
            player = FindObjectOfType<Player>();
        }
    }

    // 배경음악
    // 배경음악 재생
    private void PlayBGMForCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 화면 인덱스에 맞는 배경음악 재생
        if (currentSceneIndex < sceneBGMs.Length && sceneBGMs[currentSceneIndex] != null)
            PlayBGM(sceneBGMs[currentSceneIndex]);
        else
            StopBGM(); // 없으면 배경음악을 멈춤
    }

    // 배경음악 재생
    public void PlayBGM(AudioClip clip, bool forcePlay = false)
    {
        // 같은 배경음악이 이미 재생 중이면 중복 재생하지 않음
        if (bgmSource != null && clip != null)
        {
            // AudioSource가 비활성화된 경우 활성화
            if (!bgmSource.enabled)
                bgmSource.enabled = true;

            if (bgmSource.clip != clip || forcePlay) {
                bgmSource.clip = clip;
                bgmSource.loop = true; 
                bgmSource.Play();
                currentBGMClip = clip;  // 현재 재생 중인 배경음악을 저장
            }
        }
    }

    // 배경음악 정지
    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    // 배경음악 일시정지
    public void PauseBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Pause();
    }

    // 배경음악 다시 재생 (일시정지된 부분부터)
    public void ResumeBGM()
    {
        if (bgmSource != null)
            bgmSource.UnPause();
    }


    // 효과음
    // 효과음 재생
    public void PlaySound(AudioClip clip)
    {
        if (soundEffectSource != null && clip != null)
        {
            if (soundEffectSource != null && clip != null) {
                soundEffectSource.loop = false;
                soundEffectSource.clip = clip;
                soundEffectSource.Play();
            }
        }
    }

    // 강제로 사운드 재생하기 (중복 허용 시)
    public void PlaySoundForce(AudioClip clip)
    {
        if (soundEffectSource != null && clip != null) {
            soundEffectSource.loop = false;
            soundEffectSource.clip = clip;
            soundEffectSource.Play();
        }
    }

    public void PlaySoundOneShot(AudioClip clip)
    {
        if (soundEffectSource != null && clip != null) {
            soundEffectSource.loop = false;
            soundEffectSource.PlayOneShot(clip);
        }
    }

    public void PlayLoopSound(AudioClip clip)
    {
        if (loopingEffectSource != null && clip != null) {
            loopingEffectSource.clip = clip;
            loopingEffectSource.loop = true; 
            loopingEffectSource.volume = 1f;
            loopingEffectSource.Play();
        }
    }

    public void StopLoopSound()
    {
        loopingEffectSource.loop = false;
        loopingEffectSource.Stop();
        loopingEffectSource.clip = null;   // 클립을 제거하여 다음 사운드에 영향 없도록 함
    }

    public void PauseLoopSound()
    {
        if (loopingEffectSource != null && loopingEffectSource.isPlaying) {
            loopingEffectSource.Pause();
        }
    }

    public void ResumeLoopSound()
    {
        if (loopingEffectSource != null && loopingEffectSource.clip != null) {
            loopingEffectSource.UnPause();
        }
    }

    public void RegisterBurnableObject(BurnableObject burnableObject)
    {
        if (!activeBurnableObjects.Contains(burnableObject))
        {
            activeBurnableObjects.Add(burnableObject);
        }
        // onFire 상태이고 거리가 maxDistance 이내면 소리 재생
        if (burnableObject.onFire && player != null)
        {
            float distance = Vector2.Distance(burnableObject.transform.position, player.transform.position);
            if (distance < maxDistance)
                PlayLoopSound(burnableObject.burnSound);
        }
    }

    // 비활성화된 BurnableObject 등록 해제
    public void UnregisterBurnableObject(BurnableObject burnableObject)
    {
        if (activeBurnableObjects.Contains(burnableObject)) {
            activeBurnableObjects.Remove(burnableObject);
        }
        // 모든 BurnableObject가 비활성화되면 소리 정지
        if (activeBurnableObjects.Count == 0)
        {
            StopLoopSound();
        }
    }

    // 플레이어 위치에 따라 볼륨 조절
    private void AdjustSoundVolume()
    {
        if (isGamePaused || activeBurnableObjects.Count == 0 || loopingEffectSource.clip == null) return;

        // 가장 가까운 활성화된 BurnableObject와의 거리 계산
        float closestDistance = maxDistance;
        bool soundShouldPlay = false; // 소리가 재생되어야 하는지 여부를 저장

        foreach (var burnableObject in activeBurnableObjects)
        {
            if (burnableObject.onFire)
            {
                float distance = Vector2.Distance(burnableObject.transform.position, player.transform.position);
                closestDistance = Mathf.Min(closestDistance, distance);

                if (distance < maxDistance)
                {
                    soundShouldPlay = true; //가장 가까운 오브젝트가 maxDistance 이내라면소리가 재생되어야 함
                }
            }
        }

        if (soundShouldPlay)
        {
            loopingEffectSource.volume = 1 - (closestDistance / maxDistance);
            if (!loopingEffectSource.isPlaying)
                loopingEffectSource.Play();
        }
        else
        {
            PauseLoopSound();
        }
    }

    public void SetGamePaused(bool isPaused)
    {
        isGamePaused = isPaused;
    }
}
