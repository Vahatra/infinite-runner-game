using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bob : MonoBehaviour
{
    public static Bob Instance { get; set; }
    public bool canMove, canDie, hasLife, neverHadLife;
    public Animator bobAnim;

    public SpriteRenderer theRender;
    public GameObject shield;
    public SpriteRenderer shieldRender;

    public AudioSource bobAudioSource;
    public AudioClip[] soundClip;

    readonly Vector2 leftPos = new Vector2(-1.75f, 2.0f);
    readonly Vector2 rightPos = new Vector2(1.75f, 2.0f);
    readonly Vector2 midPos = new Vector2(0.0f, 2.0f);
    readonly Vector2 asidePos = new Vector2(8.0f, 2.0f);

    Vector2 touchPosition;
    Touch touch;
    bool isBusy, isShr, canShr, canAdRevive;

    //*****Test: var test
    //bool test = true;
    //*****

    WaitForSeconds delayShr = new WaitForSeconds(0.161f);

    readonly Color transparent = new Color(1, 1, 1, 0);
    float tmp, elapsedTime;
    readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(InitCor());
    }

    IEnumerator InitCor()
    {
        canDie = true;
        canAdRevive = true;
        neverHadLife = true;

        yield return new WaitForSecondsRealtime(1.0f);

        bobAudioSource.clip = soundClip[0];
        bobAudioSource.Play();
        bobAnim.Play("bob_tele");
        elapsedTime = 0.0f;
        while (elapsedTime < 0.1f)
        {
            tmp = Time.deltaTime * 5;
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, tmp);
            theRender.color -= new Color(0, 0, 0, tmp * 2);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        //Debug.Log(theRender.color);
        transform.position = midPos;

        elapsedTime = 0.0f;
        while (elapsedTime < 0.1f)
        {
            tmp = Time.deltaTime * 5;
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, tmp);
            theRender.color += new Color(0, 0, 0, tmp * 2);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        //Debug.Log(theRender.color);
        transform.localScale = Vector3.one;
        theRender.color = Color.white;
    }

    void Update()
    {
        if (canMove)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    touchPosition = Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (!isBusy)
                            {
                                if (touchPosition.x < 0.0f)
                                {
                                    if (transform.position.x > -1.0f)
                                    {
                                        canShr = false;
                                        StartCoroutine(TeleportCor(leftPos));
                                    }
                                    else
                                    {
                                        canShr = false;
                                        StartCoroutine(ShrinkCor());
                                    }
                                }
                                else
                                {
                                    if (transform.position.x < 1.0f)
                                    {
                                        canShr = false;
                                        StartCoroutine(TeleportCor(rightPos));
                                    }
                                    else
                                    {
                                        canShr = false;
                                        StartCoroutine(ShrinkCor());
                                    }
                                }
                            }
                            break;
                        case TouchPhase.Stationary:
                            if (!isBusy)
                            {
                                if (touchPosition.x < 0.0f)
                                {
                                    if (transform.position.x > -1.0f)
                                    {
                                        canShr = false;
                                        StartCoroutine(TeleportCor(leftPos));
                                    }
                                    else if (canShr)
                                    {
                                        canShr = false;
                                        StartCoroutine(ShrinkCor());
                                    }
                                }
                                else
                                {
                                    if (transform.position.x < 1.0f)
                                    {
                                        canShr = false;
                                        StartCoroutine(TeleportCor(rightPos));
                                    }
                                    else if (canShr)
                                    {
                                        canShr = false;
                                        StartCoroutine(ShrinkCor());
                                    }
                                }
                            }
                            break;
                        case TouchPhase.Ended:
                            canShr = true;
                            break;
                    }
                }
            }

            if (isShr)
                transform.position = Vector3.Lerp(transform.position,
                                                   new Vector3(transform.position.x, 2.25f, 0.0f),
                                                   Time.deltaTime * 9.0f);
            else
                transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, 2.0f, 0.0f),
                Time.deltaTime * 9.0f);
        }
    }

    IEnumerator ShrinkCor()
    {
        isBusy = true;

        isShr = true;
        bobAudioSource.clip = soundClip[1];
        bobAudioSource.Play();
        bobAnim.Play("bob_shr");
        yield return delayShr;
        isShr = false;
        yield return delayShr;

        isBusy = false;
    }

    IEnumerator TeleportCor(Vector3 targetPos)
    {
        isBusy = true;

        bobAudioSource.clip = soundClip[0];
        bobAudioSource.Play();
        bobAnim.Play("bob_tele");
        elapsedTime = 0.0f;
        while (elapsedTime < 0.1f)
        {
            tmp = Time.deltaTime * 5;
            //transform.localScale -= new Vector3(tmp, tmp, 0.0f);
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, tmp);
            theRender.color -= new Color(0, 0, 0, tmp * 2);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        if (canMove)
            transform.position = targetPos;
        else
            yield return null;

        elapsedTime = 0.0f;
        while (elapsedTime < 0.1f)
        {
            tmp = Time.deltaTime * 5;
            //transform.localScale += new Vector3(tmp, tmp, 0.0f);
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, tmp);
            theRender.color += new Color(0, 0, 0, tmp * 2);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        transform.localScale = Vector3.one;
        theRender.color = Color.white;

        isBusy = false;
    }

    //*****Test: Toggle UI, toggle mety maty & ts mety maty
    //public void TestToggleFn()
    //{
    //    test = !test;
    //}
    //*****

    IEnumerator ResetBobCor()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        bobAnim.Play("bob_wind");
        bobAnim.Play("bob_idle");
        transform.position = midPos;
    }

    //Bonus Effects*************************************
    //Shield
    public void AddShield()
    {
        canDie = false;
        StartCoroutine(ShieldOnCor());
    }

    IEnumerator ShieldOnCor()
    {
        //float a = Time.fixedTime;

        shield.transform.localScale = Vector3.one;
        float localElapsedTime = 0.0f;
        float localTmp;
        while (localElapsedTime < 0.2f)
        {
            localTmp = Time.deltaTime * 5;
            shieldRender.color += new Color(0, 0, 0, localTmp);
            localElapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        shieldRender.color = Color.white;

        yield return new WaitForSeconds(4.7f);

        while (localElapsedTime < 0.3f)
        {
            localTmp = Time.deltaTime * 5;
            shieldRender.color -= new Color(0, 0, 0, localTmp);
            shield.transform.localScale += new Vector3(localTmp, localTmp, 0.0f);
            localElapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        shieldRender.color = transparent;
        canDie = true;

        //Debug.Log(Time.fixedTime - a + " bob");
    }

    //Life
    public void AddLife()
    {
        hasLife = true;
        neverHadLife = false;
    }

    //*************************************

    void OnTriggerEnter2D(Collider2D col)
    {
        //*****Test: ts mety maty
        //if (test)
            //return;
        //*****
        if (!col.transform.CompareTag("Bonus") && canMove && canDie)
        {
            canMove = false;
            bobAudioSource.clip = soundClip[2];
            bobAudioSource.Play();
            bobAnim.Play("bob_dead");
            if (hasLife)//If Bob has a life are Ready
            {
                hasLife = false;
                StartCoroutine(HadLifeReviveCor());
            }
            else if (Score.Instance.canUseRevive && UiManager.Instance.RewardBasedVideoReady && canAdRevive)
            {
                AdsRevive();
            }
            else
            {
                IsDead();
            }
        }
    }

    public void IsDead()
    {
        canAdRevive = true;
        neverHadLife = true;
        UiManager.Instance.BobDead();
        StartCoroutine(ResetBobCor());
        Bonus.Instance.BonusOff(3);
    }

    IEnumerator HadLifeReviveCor()
    {
        Score.Instance.PauseScore();
        Bonus.Instance.BonusOff(3);
        ObjPooler.Instance.PausePool();
        yield return new WaitForSeconds(2.0f);
        transform.position = asidePos;
        bobAnim.Play("bob_wind");
        bobAnim.Play("bob_idle");
        canMove = true;
        StartCoroutine(TeleportCor(midPos));
        Bonus.Instance.BonusOff(1);
        yield return new WaitForSeconds(2.0f);
        ObjPooler.Instance.ResumePool();
        Score.Instance.ResumeScore();
    }

    void AdsRevive()
    {
        canAdRevive = false;
        UiManager.Instance.AskRevive();
        Score.Instance.PauseScore();
        Bonus.Instance.BonusOff(3);
        ObjPooler.Instance.PausePool();
    }

    public void ReviveBob()
    {
        StartCoroutine(ReviveBobCor());
    }

    IEnumerator ReviveBobCor()
    {
        yield return new WaitForSeconds(1.0f);
        transform.position = asidePos;
        bobAnim.Play("bob_wind");
        bobAnim.Play("bob_idle");
        canMove = true;
        StartCoroutine(TeleportCor(midPos));
        yield return new WaitForSeconds(1.0f);
        ObjPooler.Instance.ResumePool();
        Score.Instance.ResumeScore();
    }
}
