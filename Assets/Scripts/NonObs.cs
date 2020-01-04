using System.Collections;
using UnityEngine;

public class NonObs : MonoBehaviour
{
    public short bonusType;
    public GameObject bonusObject;
    public SpriteRenderer bonusRender;

    public GameObject bar;
    public SpriteRenderer barRender;

    public Color theColor;

    readonly Vector3 targetPos = new Vector3(0.0f, 3.5f);
    readonly Vector3 initPos = new Vector3(0.0f, -6.0f);
    readonly Vector3 leftPos = new Vector3(-2.0f, 0.0f);
    readonly Vector3 rightPos = new Vector3(2.0f, 0.0f);
    float elapsedTime, tmp;
    bool once0, once1;
    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    void OnEnable()
    {
        once0 = true;
        once1 = true;

        bonusObject.transform.localScale = Vector3.one;
        bonusRender.color = Color.white;
        barRender.color = theColor;
        transform.localPosition = initPos;

        if (Random.Range(0, 2) == 0)
        {
            bar.transform.localPosition = rightPos;
        }
        else
        {
            bar.transform.localPosition = leftPos;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Bob") && once0 && Bob.Instance.canMove)
        {
            once0 = false;
            Bonus.Instance.BonusOn(bonusType);
            StartCoroutine(CollisionCor());
        }
    }

    IEnumerator CollisionCor()
    {
        elapsedTime = 0.0f;
        while (elapsedTime < 0.2f)
        {
            tmp = Time.deltaTime * 5;
            bonusObject.transform.localScale += new Vector3(tmp, tmp, 0.0f);
            bonusRender.color -= new Color(0, 0, 0, tmp);
            barRender.color -= new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        gameObject.SetActive(false);
    }

    void BeforeDisable()
    {
        if (once1)
        {
            once1 = false;
            StartCoroutine(BeforeDisableCor());
        }
    }

    IEnumerator BeforeDisableCor()
    {
        elapsedTime = 0.0f;
        while (elapsedTime < 0.5f)
        {
            tmp = Time.deltaTime * 2;
            bonusRender.color -= new Color(0, 0, 0, tmp);
            barRender.color -= new Color(0, 0, 0, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (transform.position.y > 2.9f)
            BeforeDisable();
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 0.75f);
    }
}
