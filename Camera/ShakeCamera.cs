using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour
{

    private Vector3 m_randomAngle;

    public Vector3[] angles;

    public float speed;

    public int rate;

    // Use this for initialization
    void Start ( )
    {
        m_randomAngle = RandomEulerAngles();
        print(transform.eulerAngles);
    }

    // Update is called once per frame
    void Update ( )
    {
        if (Time.frameCount % rate != 0)
        {
            //print(Time.frameCount);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, m_randomAngle, Time.deltaTime * speed);
        }
        else
        {
            m_randomAngle = RandomEulerAngles();
        }
    }

    private Vector3 RandomEulerAngles ( )
    {
        rate = (int)(Random.value * 10) + 100;
        return angles[(int)(Random.value * (angles.Length - 0.01f))];
    }
}
