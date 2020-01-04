using UnityEngine;

public class Obs_bx : MonoBehaviour
{

    public GameObject movingPart;
    public SpriteRenderer firstRender;
    public SpriteRenderer secondRender;
    public float disNone, disShr, targetHeight;

    float mx, my, fx, fy, mSpeed;
    bool leftRight;
    Vector3 fTarget;
    Vector3 direction;
    Vector2 mTarget = Vector2.zero;

    void OnEnable()
    {
        if (Random.Range(0, 2) == 0)
            my = targetHeight;
        else
            my = 0.0f;

        fy = -6.0f;

        firstRender.sortingOrder = 0;
        secondRender.sortingOrder = 0;

        switch (ObjPooler.Instance.objPooledType)
        {
            case 1:
                fx = 0.15f;
                mx = disNone - 1.75f - fx;
                leftRight = false;

                firstRender.color = Weather.Instance.lightColor;
                secondRender.color = Weather.Instance.lightColor * 1.07f;
                secondRender.sortingOrder = 2;
                break;
            case 2:
                fx = -0.15f;
                mx = 1.75f - disNone - fx;
                leftRight = true;

                firstRender.color = Weather.Instance.lightColor;
                secondRender.color = Weather.Instance.lightColor * 1.07f;
                secondRender.sortingOrder = 2;
                break;
            case 3:
                fx = 0.15f;
                mx = disShr - 1.75f - fx;
                leftRight = false;

                firstRender.color = Weather.Instance.lightColor;
                //secondRender.color = Weather.Instance.darkColor * 1.1f;
                secondRender.color = Weather.Instance.darkColor * 1.1f;
                firstRender.sortingOrder = 2;
                break;
            case 4:
                fx = -0.15f;
                mx = 1.75f - disShr - fx;
                leftRight = true;

                firstRender.color = Weather.Instance.lightColor;
                //secondRender.color = Weather.Instance.darkColor * 1.1f;
                secondRender.color = Weather.Instance.darkColor * 1.1f;
                firstRender.sortingOrder = 2;
                break;
            case 7:
                mx = Random.Range(-0.75f, -1.0f);
                fx = -1.3f - mx;
                leftRight = false;

                firstRender.color = Weather.Instance.lightColor;
                secondRender.color = Weather.Instance.lightColor * 1.07f;
                secondRender.sortingOrder = 2;
                break;
            case 8:
                mx = Random.Range(0.75f, 1.0f);
                fx = 1.3f - mx;
                leftRight = true;

                firstRender.color = Weather.Instance.lightColor;
                secondRender.color = Weather.Instance.lightColor * 1.07f;
                secondRender.sortingOrder = 2;
                break;
            case 9:
                mx = Random.Range(-0.75f, -1.0f);
                fx = -1.3f - mx;
                fy = -8.4f;
                leftRight = false;

                firstRender.color = Weather.Instance.lightColor;
                secondRender.color = Weather.Instance.lightColor * 1.07f;
                secondRender.sortingOrder = 2;
                break;
            case 10:
                mx = Random.Range(0.75f, 1.0f);
                fx = 1.3f - mx;
                fy = -8.4f;
                leftRight = true;

                firstRender.color = Weather.Instance.lightColor;
                secondRender.color = Weather.Instance.lightColor * 1.07f;
                secondRender.sortingOrder = 2;
                break;
        }

        mSpeed = (Score.Instance.Speed * 0.8f) + 0.2f;

        if (Random.Range(0, 2) == 0)
            transform.position = new Vector2(fx + Random.Range(0.5f, 0.75f), fy);
        else
            transform.position = new Vector2(fx - Random.Range(0.5f, 0.75f), fy);

        fTarget = new Vector3(fx, 2.0f, 0.0f);
        direction = (fTarget - transform.position).normalized;

        mTarget = new Vector2(mx, my);
        if (leftRight)
            mx += Random.Range(2.0f, 3.0f);
        else
            mx -= Random.Range(2.0f, 3.0f);
        movingPart.transform.localPosition = new Vector2(mx, my);


        //firstRender.sortingOrder = 0;
        //secondRender.sortingOrder = 0;
        //if (Random.Range(0, 3) == 0)
        //{
        //    firstRender.color = Weather.Instance.lightColor;
        //    secondRender.color = Weather.Instance.lightColor * 1.07f;
        //    secondRender.sortingOrder = 2;
        //}
        //else
        //{
        //    firstRender.color = Weather.Instance.darkColor;
        //    secondRender.color = Weather.Instance.darkColor * 1.1f;
        //    secondRender.sortingOrder = 2;
        //}
    }

    void Update()
    {
        if (transform.position.y > 6)
            gameObject.SetActive(false);
        movingPart.transform.localPosition = Vector3.Lerp(movingPart.transform.localPosition, mTarget, Time.deltaTime * mSpeed);
        transform.position += direction * Time.deltaTime * Score.Instance.Speed;
    }
}

