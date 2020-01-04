using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; set; }

    public Animator uiAnimator;
    public Image uiPanel;

    public Image p_snd_img;

    public Sprite soundOn;
    public Sprite soundOff;

    public Text revive_txt;
    public SpriteRenderer heartRender;
    public GameObject heartMask;

    public float startVolume = 0.75f;
    public float deadVolume = 1.0f;
    bool isPlaying;
    int mute = 0, firstEver = 0, noAdsFor = 0;
    public AudioSource audioSource;
    //public int soundClipIndex;
    //public AudioClip[] soundClip;

    readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    readonly WaitForFixedUpdate fixedEndOfFrame = new WaitForFixedUpdate();

    readonly Color transparent = new Color(1, 1, 1, 0);

    //Advertisement
    public bool RewardBasedVideoReady, InterstitialReady, usedRevive;
    bool alreadyRunning;
    Coroutine cor;
    int bannerCount;

    //Share
    bool isProcessing, isFocus;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        Time.timeScale = 1.0f;

        firstEver = PlayerPrefs.GetInt("firstEver");
        if (firstEver == 0)
        {
            noAdsFor = 4;
            PlayerPrefs.SetInt("firstEver", 1);
        }
        else
        {
            noAdsFor = 0;
#if UNITY_EDITOR
            Debug.Log("Nope.");
#else
            AdManager.Instance.LoadAllAds();
#endif
        }

        mute = PlayerPrefs.GetInt("mute");
        if (mute == 0)
        {
            audioSource.mute = false;
            p_snd_img.sprite = soundOn;
        }
        else
        {
            audioSource.mute = true;
            p_snd_img.sprite = soundOff;
        }
        audioSource.Play();
        StartCoroutine(Play_onCor());
    }

    IEnumerator Play_onCor()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        uiAnimator.Play("play_on");
    }

    public void Mute()
    {
        if (mute == 1)
        {
            audioSource.mute = false;
            PlayerPrefs.SetInt("mute", 0);
            mute = 0;
            p_snd_img.sprite = soundOn;
        }
        else
        {
            audioSource.mute = true;
            PlayerPrefs.SetInt("mute", 1);
            mute = 1;
            p_snd_img.sprite = soundOff;
        }
    }

    public void Play()
    {
        StartCoroutine(Play_offCor());
    }

    IEnumerator Play_offCor()
    {
        uiAnimator.Play("play_off");
        Bob.Instance.canMove = true;
        Weather.Instance.Play();
        yield return new WaitForSecondsRealtime(2.0f);
        ObjPooler.Instance.StartPool();
        Score.Instance.PlayScore();
        isPlaying = true;
    }

    public void Pause()
    {
        if (Bob.Instance.canMove)
        {
            isPlaying = false;
            uiAnimator.Play("pause_on");
            Bob.Instance.canMove = false;
            Time.timeScale = 0;
            uiPanel.color = Weather.Instance.backColor;
#if UNITY_EDITOR
            Debug.Log("Nope.");
#else
            if (noAdsFor == 0)
                AdManager.Instance.ShowBanner();
#endif
        }
    }

    public void Resume()
    {
        StartCoroutine(Pause_offCor());
    }

    IEnumerator Pause_offCor()
    {
#if UNITY_EDITOR
        Debug.Log("Nope.");
#else
        if (noAdsFor == 0)
            AdManager.Instance.HideBanner();
#endif
        uiAnimator.Play("pause_off");
        yield return new WaitForSecondsRealtime(1.333f);
        Bob.Instance.canMove = true;
        Time.timeScale = 1;
        isPlaying = true;
    }

    public void AskRevive()
    {
        isPlaying = false;
        alreadyRunning = false;
        usedRevive = false;
        uiAnimator.Play("revive_on");
        cor = StartCoroutine(AskReviveCor());
    }

    public void YesRevive()
    {
        alreadyRunning = true;
        usedRevive = true;
        StartCoroutine(Revive_offCor());
        Bob.Instance.ReviveBob();
        Score.Instance.YesRevive();
    }

    public void NoRevive()
    {
        alreadyRunning = true;
        usedRevive = false;
        StartCoroutine(Revive_offCor());
        Bob.Instance.IsDead();
    }

    IEnumerator AskReviveCor()
    {
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.5f)
        {
            tmp = Time.deltaTime * 3;
            heartRender.color += new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        heartRender.color = Color.white;

        heartMask.transform.localScale = Vector3.forward;
        elapsedTime = 0.0f;
        while (elapsedTime < 4.7f)
        {
            heartMask.transform.localScale = Vector3.MoveTowards(heartMask.transform.localScale,
                                                                 Vector3.one,
                                                                 0.006f);
            elapsedTime += Time.fixedDeltaTime;
            yield return fixedEndOfFrame;
        }

        if (!alreadyRunning)
            NoRevive();
        //Debug.Log(Time.fixedTime - a + " fin");

    }

    IEnumerator Revive_offCor()
    {
        isPlaying = true;
        if (cor != null)
            StopCoroutine(cor);
        uiAnimator.Play("revive_off");
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.5f)
        {
            tmp = Time.deltaTime * 3;
            heartRender.color -= new Color(0, 0, 0, tmp);
            heartMask.transform.localScale = Vector3.MoveTowards(heartMask.transform.localScale,
                                                                 Vector3.forward,
                                                                 0.2f);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        heartMask.transform.localScale = Vector3.forward;
        heartRender.color = transparent;
    }

    public void BobDead()
    {
        isPlaying = false;
        Score.Instance.StopScore();
        StartCoroutine(BobDeadCor());
        ObjPooler.Instance.ResetPool();
    }

    IEnumerator BobDeadCor()
    {
#if UNITY_EDITOR
        Debug.Log("Nope.");
#else
        if (noAdsFor == 0)
        {
            noAdsFor = 0;
            AdManager.Instance.IfAnyFailed();
            if (usedRevive && RewardBasedVideoReady)
            {
                StartCoroutine(FadeOut(audioSource, 0.4f));
                yield return new WaitForSecondsRealtime(0.5f);
                usedRevive = false;
                bannerCount = 0;
                AdManager.Instance.ShowRewardBasedVideo();
            }
            else if (bannerCount > 1 && InterstitialReady)
            {
                StartCoroutine(FadeOut(audioSource, 0.4f));
                yield return new WaitForSecondsRealtime(0.5f);
                bannerCount = 0;
                AdManager.Instance.ShowInterstitial();
            }
            else
            {
                StartCoroutine(FadeIn(audioSource, deadVolume, 0.5f));
                yield return new WaitForSecondsRealtime(0.5f);
                bannerCount++;
                AdManager.Instance.ShowBanner();
            }
        }
        else if (noAdsFor == 1)
        {
            AdManager.Instance.LoadAllAds();
            noAdsFor = 0;
        }
        else
        {
            noAdsFor--;
        }
#endif
        uiPanel.color = Weather.Instance.backColor;
        uiAnimator.Play("retry_on");
        yield return new WaitForSecondsRealtime(0.5f);
        Weather.Instance.Stop();
        uiAnimator.Play("share");
    }

    public void Retry()
    {
        StartCoroutine(Retry_offCor());
    }

    IEnumerator Retry_offCor()
    {
        Time.timeScale = 1.0f;
        Weather.Instance.Retry();
        uiAnimator.Play("retry_off");

#if UNITY_EDITOR
        Debug.Log("Nope.");
#else
        if (noAdsFor == 0)
            AdManager.Instance.HideBanner();
#endif

        //Sound Manager
        StartCoroutine(FadeOut(audioSource, 0.7f));
        yield return new WaitForSecondsRealtime(1.5f);
        audioSource.Play();
        StartCoroutine(FadeIn(audioSource, startVolume, 0.5f));

        Bob.Instance.canMove = true;
        ObjPooler.Instance.StartPool();
        Score.Instance.PlayScore();
        isPlaying = true;
    }

    IEnumerator FadeOut(AudioSource audioSrc, float FadeTime)
    {
        while (audioSrc.volume > 0)
        {
            audioSrc.volume -= deadVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    IEnumerator FadeIn(AudioSource audioSrc, float targetVolume, float FadeTime)
    {
        while (audioSrc.volume < targetVolume)
        {
            audioSrc.volume += targetVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    //Share Button
    public void ShareScreenshot()
    {
        if (!isProcessing)
        {
            StartCoroutine(DelayedShareCor());
        }
    }

    IEnumerator DelayedShareCor()
    {
#if UNITY_EDITOR
        Debug.Log("Nope");
        yield return null;
#else
        string filePath = Application.persistentDataPath + "/" + "screenshot.png";
        if (File.Exists(filePath)) File.Delete(filePath);

        ScreenCapture.CaptureScreenshot("screenshot.png");
        while (!File.Exists(filePath))
        {
            yield return new WaitForSeconds(.05f);
        }

        NativeShare.Share(filePath);

        yield return new WaitUntil(() => isFocus);
        isProcessing = false;
#endif
    }

    void OnApplicationFocus(bool focus)
    {
#if UNITY_EDITOR
        Debug.Log("Nope.");
#else
        isFocus = focus;
#endif
    }

    //Rate
    public void RateOnStore()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.eno.bobble");
    }

    //Like
    public void LikeOnFacebook()
    {
        Application.OpenURL("https://www.facebook.com/eno.bobble/");
    }

    //OnPause manager
    void OnApplicationPause(bool pause)
    {
#if UNITY_EDITOR
        Debug.Log("Nope.");
#else
        if (isPlaying && pause)
            Pause();
#endif
    }
}
