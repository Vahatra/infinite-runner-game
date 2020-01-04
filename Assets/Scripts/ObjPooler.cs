using System.Collections;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    public static ObjPooler Instance { get; set; }
    public GameObject[] allPrefabs;
    public short objPooledType;
    public int min = 0, max = 9;
    public Color hardColor;

    //1 - Small
    readonly GameObject[] s = new GameObject[6];
    short s_it;

    //2 - Medium
    readonly GameObject[] m = new GameObject[6];
    short m_it;

    //3 - Big
    readonly GameObject[] b = new GameObject[6];
    short b_it;

    //4 - Big X Big-Medium-Small
    readonly GameObject[] bx = new GameObject[6];
    short bx_it;
    short[] bx_tab = { 0, 1, 2, 3, 4, 5 };

    //0 - Bonus
    readonly GameObject[] bonus = new GameObject[4];

    ////0 - Coin
    //readonly GameObject[] coin = new GameObject[6];
    //short coin_it;

    GameObject tmpObj;
    bool canPool, hardMode;
    int k, f, index, nonObsTimer;
    short left, right;

    Coroutine cor1, cor2, cor3;

    WaitForSeconds delayObs0;
    WaitForSeconds delayObs1;

    readonly Vector2 origin = new Vector2(0.0f, -6.0f);

    #region Waves
    short[] wave;
    readonly short[][] waveTab = {

		//B-NxN-B
        new short [] { 1, 7, 3, 1 },//0
        new short [] { 2, 7, 3, 1 },
        new short [] { 3, 7, 1, 1 },
        new short [] { 3, 7, 2, 1 },
        new short [] { 4, 7, 0, 0 },//4
        new short [] { 1, 8, 3, 1 },
        new short [] { 2, 8, 3, 1 },
        new short [] { 3, 8, 1, 1 },
        new short [] { 3, 8, 2, 1 },
        new short [] { 4, 8, 0, 0 },//9

		//S-NxN-S
        new short [] { 4, 3, 0, 0 }, new short [] { 4, 3, 0, 0 },//10
        new short [] { 4, 4, 0, 0 }, new short [] { 4, 4, 0, 0 },//13

        //S-S
        new short [] { 1, 3, 3, 4 },//14
        new short [] { 2, 3, 3, 4 },
        new short [] { 3, 3, 1, 4 },
        new short [] { 3, 3, 2, 4 },//17

        //S-DxD-S
        new short [] { 4, 10, 1, 3 },//18
        new short [] { 1, 10, 4, 3 },

        new short [] { 4, 10, 2, 3 },
        new short [] { 2, 10, 4, 3 },

        new short [] { 4, 9, 2, 4 },
        new short [] { 2, 9, 4, 4 },

        new short [] { 4, 9, 1, 4 },
        new short [] { 1, 9, 4, 4 },//25

		//B-DxD-B
        new short [] { 4, 10, 1, 7 },//26
////new short [] { 1, 10, 4, 7 },

        new short [] { 4, 10, 2, 7 },
////new short [] { 2, 10, 4, 7 },

        new short [] { 4, 9, 2, 8 },
////new short [] { 2, 9, 4, 8 },

        new short [] { 4, 9, 1, 8 },//29
////new short [] { 1, 9, 4, 8 }
        //*****
    };
    #endregion

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //Debug.Log(waveTab.Length);
        for (int i = 0; i < allPrefabs.Length; i++)
        {
            switch (i)
            {
                case 0:
                    for (int j = 0; j < 6; j++)
                    {
                        tmpObj = Instantiate(allPrefabs[i]);
                        tmpObj.SetActive(false);
                        b[j] = tmpObj;
                    }
                    break;
                case 1:
                    for (int j = 0; j < 6; j++)
                    {
                        tmpObj = Instantiate(allPrefabs[i]);
                        tmpObj.SetActive(false);
                        m[j] = tmpObj;
                    }
                    break;
                case 2:
                    for (int j = 0; j < 6; j++)
                    {
                        tmpObj = Instantiate(allPrefabs[i]);
                        tmpObj.SetActive(false);
                        s[j] = tmpObj;
                    }
                    break;
                case 3:
                    for (int j = 0; j < 2; j++)
                    {
                        tmpObj = Instantiate(allPrefabs[i]);
                        tmpObj.SetActive(false);
                        bx[j] = tmpObj;
                    }
                    break;
                case 4:
                    for (int j = 2; j < 4; j++)
                    {
                        tmpObj = Instantiate(allPrefabs[i]);
                        tmpObj.SetActive(false);
                        bx[j] = tmpObj;
                    }
                    break;
                case 5:
                    for (int j = 4; j < 6; j++)
                    {
                        tmpObj = Instantiate(allPrefabs[i]);
                        tmpObj.SetActive(false);
                        bx[j] = tmpObj;
                    }
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    tmpObj = Instantiate(allPrefabs[i]);
                    tmpObj.SetActive(false);
                    bonus[i - 6] = tmpObj;
                    break;
                    //case 10:
                    //    for (int j = 0; j < 3; j++)
                    //    {
                    //        tmpObj = Instantiate(allPrefabs[i]);
                    //        tmpObj.SetActive(false);
                    //        coin[j] = tmpObj;
                    //    }
                    //    break;
                    //case 11:
                    //    for (int j = 3; j < 5; j++)
                    //    {
                    //        tmpObj = Instantiate(allPrefabs[i]);
                    //        tmpObj.SetActive(false);
                    //        coin[j] = tmpObj;
                    //    }
                    //    break;
                    //case 12:
                    //tmpObj = Instantiate(allPrefabs[i]);
                    //tmpObj.SetActive(false);
                    //coin[5] = tmpObj;
                    //break;
            }
        }
    }

    public void StartPool()
    {
        cor1 = StartCoroutine(PoolObsCor());
    }

    void Shuffle(short[] tab)
    {

        int n = tab.Length;
        //string test ="";
        //for (int i = 0; i < n; i++)
        //{
        //    test += tab[i].ToString() + " ";
        //}
        //Debug.Log(test);
        for (int i = 0; i < n; i++)
        {
            int r = Random.Range(i, n);
            short t = tab[r];
            tab[r] = tab[i];
            tab[i] = t;
        }
        //test = "";
        //for (int i = 0; i < n; i++)
        //{
        //    test += tab[i].ToString() + " ";
        //}
        //Debug.Log(test);
    }

    public void PausePool()
    {
        f = 20;
        for (int i = 0; i < 4; i++)
        {
            if (bonus[i].activeSelf)
                bonus[i].SetActive(false);
        }
    }

    public void ResumePool()
    {
        k = 1;
        f = 1;
    }

    IEnumerator PoolObsCor()
    {
        //Init Pool
        Shuffle(bx_tab);
        canPool = true;
        left = 0;
        right = 0;
        hardMode = false;
        k = 0;
        f = 1;
        index = 0;
        nonObsTimer = 0;
        bx_it = 0;
        min = 0;
        max = 9;
        wave = waveTab[0];
        delayObs0 = new WaitForSeconds(0.8f);
        delayObs1 = new WaitForSeconds(0.75f);

        while (canPool)
        {
            k = 0;
            //float x = Time.fixedTime;
            while (k < f)
            {
                k++;
                if (wave[1] > 8)
                    yield return delayObs0;
                else
                    yield return delayObs1;
            }
            //Debug.Log(Time.fixedTime - x);

            index = Random.Range(min, max);

            if (index > -1 && index < 5)
            {
                if (left == 2)
                {
                    left = 0;
                    index = 9;
                    right++;
                }
                else
                {
                    left++;
                    right = 0;
                }
            }
            else if (index > 4 && index < 10)
            {
                if (right == 2)
                {
                    right = 0;
                    index = 4;
                    left++;
                }
                else
                {
                    right++;
                    left = 0;
                }
            }
            wave = waveTab[index];

            for (int i = 0; i < 4;)
            {

                objPooledType = wave[i + 1];

                switch (wave[i])
                {
                    case 1:
                        s[s_it].SetActive(true);
                        s_it++;
                        if (s_it == 6)
                            s_it = 0;
                        break;
                    case 2:
                        m[m_it].SetActive(true);
                        m_it++;
                        if (m_it == 6)
                            m_it = 0;
                        break;
                    case 3:
                        b[b_it].SetActive(true);
                        b_it++;
                        if (b_it == 6)
                            b_it = 0;
                        break;
                    case 4:
                        bx[bx_tab[bx_it]].SetActive(true);
                        bx_it++;
                        if (bx_it == 6)
                            bx_it = 0;
                        break;
                }
                i += 2;
            }

            nonObsTimer++;
            if (nonObsTimer == 15)
            {
                nonObsTimer = 0;
                if (hardMode)
                {
                    cor3 = StartCoroutine(ToggleToHardModeCor());
                    hardMode = false;
                }
                else
                {
                    if (Bob.Instance.neverHadLife)
                    {
                        bonus[Random.Range(0, 4)].SetActive(true);
                    }
                    else
                    {
                        bonus[Random.Range(1, 4)].SetActive(true);
                    }
                }
            }
            //else if (nonObsTimer == 5 || nonObsTimer == 10)
            //{
            //    coin[coin_it].SetActive(true);
            //    coin_it++;
            //    if (coin_it == 6)
            //        coin_it = 0;
            //}
        }
        yield return null;
    }

    public void ResetPool()
    {
        canPool = false;
        StopCoroutine(cor1);
        if (cor3 != null)
            StopCoroutine(cor3);
        StartCoroutine(ResetPoolCor());
    }

    IEnumerator ResetPoolCor()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        for (int i = 0; i < 6; i++)
        {
            s[i].SetActive(false);
            s[i].transform.position = origin;

            m[i].SetActive(false);
            m[i].transform.position = origin;

            b[i].SetActive(false);
            b[i].transform.position = origin;

            bx[i].SetActive(false);
            bx[i].transform.position = origin;
        }

        for (int i = 0; i < 4; i++)
        {
            bonus[i].SetActive(false);
            bonus[i].transform.position = origin;
        }
    }

    //Bonus Effets
    //Time
    public void SlowPooling()
    {
        cor2 = StartCoroutine(SlowPoolingCor());
    }

    IEnumerator SlowPoolingCor()
    {
        f = 25;
        yield return new WaitForSeconds(4.5f);
        if (f == 25)
        {
            ResumePool();
        }
    }

    public void StopSlowPooling()
    {
        if (cor2 != null)
            StopCoroutine(cor2);
    }

    //Hard Mode
    public void ToggleToHardMode()
    {
        hardMode = true;
    }

    IEnumerator ToggleToHardModeCor()
    {
        Bonus.Instance.BonusOn(8);
        yield return new WaitForSeconds(2.0f);
        delayObs0 = new WaitForSeconds(0.75f);
        delayObs1 = new WaitForSeconds(0.5f);
        Score.Instance.Speed = 6.9f;
        Weather.Instance.lightColor = hardColor * 1.17f;
        Weather.Instance.darkColor = hardColor;
    }
}