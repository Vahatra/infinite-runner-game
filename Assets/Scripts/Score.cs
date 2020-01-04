using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    public static Score Instance { get; set; }
    public bool canUseRevive;
    public float Speed;

    public Text h_src;
    public Text r_src;
    public Text r_bst_src;

    public Image r_img;
    public Image h_src_img;
    public Image r_rec_img;

    public Color[] colorTab;

    readonly WaitForSeconds delayScore = new WaitForSeconds(0.5f);
    readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    Color currentColor = Color.white;
    int highScore, currentScore, reviveScore, realScore, i, f;
    bool canCount;
    Coroutine cor0, cor1;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore");
    }

    public void PlayScore()
    {
        cor0 = StartCoroutine(ScoreCor());
    }

    IEnumerator ScoreCor()
    {
        Speed = 4.5f;
        currentScore = 0;
        reviveScore = 0;
        realScore = 0;
        ObjPooler.Instance.min = 0;
        ObjPooler.Instance.max = 9;
        canCount = true;
        canUseRevive = false;
        int k = 0;
        f = 4;

        while (canCount)
        {
            k = 0;
            //float x = Time.fixedTime;
            while (k < f)
            {
                k++;
                yield return delayScore;
            }
            //Debug.Log(Time.fixedTime - x);
            if (UiManager.Instance.usedRevive)
            {
                reviveScore++;
                h_src.text = reviveScore.ToString();
            }
            else
            {
                currentScore++;
                h_src.text = currentScore.ToString();
            }

            if (Speed < 6.2f)
                Speed += 0.1f;

            realScore++;
            switch (realScore)
            {
                case 5:
                    ObjPooler.Instance.max = 13;
                    canUseRevive = true;
                    break;
                case 10:
                    ObjPooler.Instance.max = 17;
                    break;
                case 15:
                    ObjPooler.Instance.min = 14;
                    ObjPooler.Instance.max = 25;
                    break;
                case 20:
                    ObjPooler.Instance.max = 29;
                    break;
                case 40:
                    ObjPooler.Instance.ToggleToHardMode();
                    break;
            }
        }
        h_src.text = "0";
        yield return null;
    }

    public void YesRevive()
    {
        reviveScore = currentScore;
        currentColor = colorTab[5];
        StartCoroutine(ToColorCor(currentColor));
    }

    public void StopScore()
    {
        StartCoroutine(StopScoreCor());
    }

    IEnumerator StopScoreCor()
    {
        canCount = false;
        if (cor0 != null)
            StopCoroutine(cor0);

        SettingUi(currentScore);

        yield return new WaitForSeconds(1.0f);
        currentColor = Color.white;
        h_src_img.color = currentColor;
        h_src.text = "0";
    }

    public void IsRewarded()
    {
        SettingUi(reviveScore);
    }

    void SettingUi(int score)
    {
        if (score > 79)
        {
            r_img.color = colorTab[3];
        }
        else if (score > 39)
        {
            r_img.color = colorTab[2];
        }
        else if (score > 19)
        {
            r_img.color = colorTab[1];
        }
        else
        {
            r_img.color = colorTab[0];
        }

        r_rec_img.color = colorTab[0];
        if (score > highScore)
        {
            highScore = score;
            r_rec_img.color = Color.white;
            PlayerPrefs.SetInt("highScore", highScore);
        }
        r_src.text = score.ToString();
        r_bst_src.text = "Best : " + highScore.ToString();
    }

    public void PauseScore()
    {
        f = 100;
    }

    public void ResumeScore()
    {
        f = 4;
    }

    //Bonus Effect
    //Point
    public void AddPoint()
    {
        cor1 = StartCoroutine(ChangeColorCor());
    }

    IEnumerator ChangeColorCor()
    {
        //float a = Time.fixedTime;
        f = 1;

        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.5f)
        {
            tmp = Time.deltaTime * 10;
            h_src_img.color = Color.Lerp(h_src_img.color, colorTab[4], tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        h_src_img.color = colorTab[4];

        yield return new WaitForSeconds(4.2f);
        f = 4;

        elapsedTime = 0.0f;
        while (elapsedTime < 0.5f)
        {
            tmp = Time.deltaTime * 10;
            h_src_img.color = Color.Lerp(h_src_img.color, currentColor, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        h_src_img.color = currentColor;

        //Debug.Log(Time.fixedTime - a + " Point");
    }

    public void StopPoint()
    {
        if (cor1 != null)
        {
            StopCoroutine(cor1);
            StartCoroutine(ToColorCor(currentColor));
        }
    }

    IEnumerator ToColorCor(Color targetColor)
    {
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.5f)
        {
            tmp = Time.deltaTime * 10;
            h_src_img.color = Color.Lerp(h_src_img.color, targetColor, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        h_src_img.color = targetColor;
    }

    ////Coin
    //public void AddScore(int p)
    //{
    //    if (UiManager.Instance.usedRevive)
    //    {
    //        reviveScore += p;
    //        h_src.text = reviveScore.ToString();
    //    }
    //    else
    //    {
    //        currentScore += p;
    //        h_src.text = currentScore.ToString();
    //    }
    //}
}
