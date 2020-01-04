using System.Collections;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public static Bonus Instance { get; set; }

    public GameObject bonusLife;
    public SpriteRenderer lifeImg;

    public GameObject bonusPoint;
    public SpriteRenderer pointImg;
    public GameObject bonusPointMask;

    public GameObject bonusShield;
    public SpriteRenderer shieldImg;
    public GameObject bonusShieldMask;

    public GameObject bonusTime;
    public SpriteRenderer timeImg;
    public GameObject bonusTimeMask;

    public GameObject hard;
    public SpriteRenderer hardImg;
    public GameObject hardMask;

    public AudioSource bonusAudioSource;

    readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    readonly WaitForFixedUpdate fixedEndOfFrame = new WaitForFixedUpdate();
    readonly Vector3 midPos = new Vector3(0.0f, 3.3f, 0.0f);
    readonly Vector3 rightPos = new Vector3(0.5f, 3.3f, 0.0f);
    readonly Vector3 leftPos = new Vector3(-0.5f, 3.3f, 0.0f);
    readonly Vector3 abovePos = new Vector3(0.0f, 4.2f, 0.0f);
    readonly Color transparent = new Color(1, 1, 1, 0);

    Coroutine cor;
    GameObject firstActive, secondActive;
    short isInProgress = -1;


    void Awake()
    {
        Instance = this;
    }

    public void BonusOn(short bonusType)
    {
        bonusAudioSource.Play();
        switch (bonusType)
        {
            case 0:
                Bob.Instance.AddLife();
                StartCoroutine(OnCor(bonusLife, lifeImg));
                break;
            case 2://Bonus Point
                Score.Instance.AddPoint();
                isInProgress = 2;
                cor = StartCoroutine(ProgressCor(bonusPoint, bonusPointMask, pointImg));
                break;
            case 4://Bonus Shield
                Bob.Instance.AddShield();
                StartCoroutine(ProgressCor(bonusShield, bonusShieldMask, shieldImg));
                break;
            case 6://Bonus Time
                ObjPooler.Instance.SlowPooling();
                isInProgress = 6;
                cor = StartCoroutine(ProgressCor(bonusTime, bonusTimeMask, timeImg));
                break;
            case 8://Danger
                ObjPooler.Instance.SlowPooling();
                isInProgress = 8;
                cor = StartCoroutine(ProgressCor(hard, hardMask, hardImg));
                break;
            //case 10://Coin 2
            //    Score.Instance.AddScore(2);
            //    break;
            //case 12://Coin 3
                //Score.Instance.AddScore(3);
                //break;
        }
    }

    public void BonusOff(short bonusType)
    {
        switch (bonusType)
        {
            case 1://Bonus Life OFF
                StartCoroutine(OffCor(bonusLife, lifeImg));
                break;
            case 3://Bonus Point OFF
                switch (isInProgress)
                {
                    case 2:
                        Score.Instance.StopPoint();
                        StartCoroutine(StopCor(bonusPoint, bonusPointMask, pointImg));
                        break;
                    case 6:
                        ObjPooler.Instance.StopSlowPooling();
                        StartCoroutine(StopCor(bonusTime, bonusTimeMask, timeImg));
                        break;
                    case 8:
                        ObjPooler.Instance.StopSlowPooling();
                        StartCoroutine(StopCor(hard, hardMask, hardImg));
                        break;
                }
                isInProgress = -1;
                break;
        }
    }

    IEnumerator StopCor(GameObject gameObj, GameObject maskObj, SpriteRenderer imgRender)
    {
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.2f)
        {
            tmp = Time.deltaTime * 5;
            gameObj.transform.localScale += new Vector3(tmp, tmp, 0.0f);
            imgRender.color -= new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }

        imgRender.color = transparent;
        gameObj.transform.position = abovePos;
        gameObj.transform.localScale = Vector3.one;
        maskObj.transform.localScale = Vector3.forward;

        if (cor != null)
            StopCoroutine(cor);

        //Place
        if (firstActive == gameObj && secondActive != null)
        {
            firstActive = secondActive;
            secondActive = null;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else if (firstActive != null && secondActive == gameObj)
        {
            secondActive = null;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else
        {
            firstActive = null;
        }
    }

    IEnumerator MoveCor(GameObject gameObj, Vector3 targetPos)
    {
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.2f)
        {
            tmp = Time.deltaTime * 25;
            gameObj.transform.position = Vector3.Lerp(gameObj.transform.position, targetPos, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
    }

    IEnumerator OnCor(GameObject gameObj, SpriteRenderer imgRender)
    {
        //Place
        if (firstActive == null)
        {
            firstActive = gameObj;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else
        {
            secondActive = gameObj;
            StartCoroutine(MoveCor(firstActive, leftPos));
            StartCoroutine(MoveCor(secondActive, rightPos));
        }

        //Animation
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.2f)
        {
            tmp = Time.deltaTime * 5;
            imgRender.color += new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        imgRender.color = Color.white;
    }

    IEnumerator OffCor(GameObject gameObj, SpriteRenderer imgRender)
    {
        //Animation
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.2f)
        {
            tmp = Time.deltaTime * 5;
            gameObj.transform.localScale += new Vector3(tmp, tmp, 0.0f);
            imgRender.color -= new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        gameObj.transform.position = abovePos;
        gameObj.transform.localScale = Vector3.one;
        imgRender.color = transparent;

        //Place
        if (firstActive == gameObj && secondActive != null)
        {
            firstActive = secondActive;
            secondActive = null;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else if (firstActive != null && secondActive == gameObj)
        {
            secondActive = null;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else
        {
            firstActive = null;
        }

        isInProgress = -1;
    }

    IEnumerator ProgressCor(GameObject gameObj, GameObject maskObj, SpriteRenderer imgRender)
    {
        //float a = Time.fixedTime;

        //Place
        if (firstActive == null)
        {
            firstActive = gameObj;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else
        {
            secondActive = gameObj;
            StartCoroutine(MoveCor(firstActive, leftPos));
            StartCoroutine(MoveCor(secondActive, rightPos));
        }

        //Animation
        float elapsedTime = 0.0f;
        float tmp;
        maskObj.transform.localScale = Vector3.forward;
        while (elapsedTime < 0.2f)
        {
            tmp = Time.deltaTime * 5;
            imgRender.color += new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        //Debug.Log(Time.fixedTime - a);
        imgRender.color = Color.white;

        elapsedTime = 0.0f;
        while (elapsedTime < 4.6f)
        {
            maskObj.transform.localScale = Vector3.MoveTowards(maskObj.transform.localScale, Vector3.one, 0.006f);
            elapsedTime += Time.fixedDeltaTime;
            yield return fixedEndOfFrame;
        }

        //Debug.Log(Time.fixedTime - a + " fin");

        elapsedTime = 0.0f;
        while (elapsedTime < 0.2f)
        {
            tmp = Time.deltaTime * 5;
            gameObj.transform.localScale += new Vector3(tmp, tmp, 0.0f);
            imgRender.color -= new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        gameObj.transform.position = abovePos;
        gameObj.transform.localScale = Vector3.one;
        maskObj.transform.localScale = Vector3.forward;
        imgRender.color = transparent;

        //Debug.Log(Time.fixedTime - a + " Bonus");

        //Place
        if (firstActive == gameObj && secondActive != null)
        {
            firstActive = secondActive;
            secondActive = null;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else if (firstActive != null && secondActive == gameObj)
        {
            secondActive = null;
            StartCoroutine(MoveCor(firstActive, midPos));
        }
        else
        {
            firstActive = null;
        }

        isInProgress = -1;
    }
}
