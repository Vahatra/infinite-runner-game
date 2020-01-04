using UnityEngine;

public class Obs_x : MonoBehaviour
{
    public SpriteRenderer theRender;
    public float disShr, targetHeight;
    public short size;

    float x, y;
    Vector3 targetPosition;
    Vector3 direction;

    void OnEnable()
    {
        if (Random.Range(0, 2) == 0)
            y = targetHeight;
        else
            y = -6.0f;

        //once = true;

        switch (ObjPooler.Instance.objPooledType)
        {
            case 1:
                x = 0;

                if (size == 0)
                    theRender.color = Weather.Instance.lightColor;
                else
                    theRender.color = Weather.Instance.lightColor * 1.07f;
                break;
            case 3:
                x = -1.75f + disShr;

                if (size == 0)
                    theRender.color = Weather.Instance.darkColor;
                else
                    theRender.color = Weather.Instance.darkColor * 1.1f;
                break;
            case 4:
                x = 1.75f - disShr;

                if (size == 0)
                    theRender.color = Weather.Instance.darkColor;
                else
                    theRender.color = Weather.Instance.darkColor * 1.1f;
                break;
            case 7:
                x = -1.3f;

                if (size == 0)
                    theRender.color = Weather.Instance.lightColor;
                else
                    theRender.color = Weather.Instance.lightColor * 1.07f;
                break;
            case 8:
                x = 1.3f;

                if (size == 0)
                    theRender.color = Weather.Instance.lightColor;
                else
                    theRender.color = Weather.Instance.lightColor * 1.07f;
                break;
            case 9:
                x = -1.3f;
                y = -8.4f;

                if (size == 0)
                    theRender.color = Weather.Instance.lightColor;
                else
                    theRender.color = Weather.Instance.lightColor * 1.07f;
                break;
            case 10:
                x = 1.3f;
                y = -8.4f;

                if (size == 0)
                    theRender.color = Weather.Instance.lightColor;
                else
                    theRender.color = Weather.Instance.lightColor * 1.07f;
                break;
        }
        targetPosition = new Vector3(x, 2.0f, 0.0f);

        if (Random.Range(0, 2) == 0)
            x += 0.5f;
        else
            x -= 0.5f;
        transform.position = new Vector2(x, y);

        direction = (targetPosition - transform.position).normalized;
    }

    void Update()
    {
        if (transform.position.y > 6)
            gameObject.SetActive(false);
        transform.position += direction * Time.deltaTime * Score.Instance.Speed;
    }
}
