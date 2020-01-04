using System.Collections;
using UnityEngine;

public class ObsOnCollide : MonoBehaviour
{

    public SpriteRenderer theRender;
    readonly Color collideColor = Color.white;
    bool once;
    readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    void OnEnable()
    {
        once = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Bob") && once && Bob.Instance.canMove && Bob.Instance.canDie)
        {
            once = false;
            StartCoroutine(CollisionCor());
        }
    }

    IEnumerator CollisionCor()
    {
        float elapsedTime = 0.0f;
        float tmp;
        while (elapsedTime < 0.5f)
        {
            tmp = Time.deltaTime * 10;
            theRender.color = Color.Lerp(theRender.color, collideColor, tmp);
            elapsedTime += Time.deltaTime;
            yield return endOfFrame;
        }
        theRender.color = collideColor;
    }
}
