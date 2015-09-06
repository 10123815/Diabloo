using UnityEngine;
using System.Collections;

public class ArrowScale : MonoBehaviour
{

    private bool isMoving;

    // Use this for initialization
    void Start ( )
    {

    }

    // Update is called once per frame
    void Update ( )
    {
        if (!isMoving)
            StartCoroutine(Scale(1, 0.5f));
    }

    private IEnumerator Scale (float dis, float time)
    {
        isMoving = true;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            transform.localScale *= 1.01f;
            yield return 0;
        }
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            transform.localScale /= 1.01f;
            yield return 0;
        }
        isMoving = false;
    }
}
