using UnityEngine;
using System.Collections;

public class MovementPath : MonoBehaviour
{
    // the count of key position and key direction must be same
    // the camera from one key position with a key direction smooth,
    // move to the next key position with the next key direction along a curve
    public Vector3[] keyPositions;
    public Vector3[] keyEulers;
    public float[] speeds;

    private float m_speed;
    private float m_argular;
    [System.NonSerialized]
    public int nextIndex = 0;
    [System.NonSerialized]
    public int keyCount;
    //private float distanceToNext;

    [System.NonSerialized]
    public bool reached = false;

    public float distanceThreshold = 0.2f;

    public delegate void OnReachDL ( );
    public OnReachDL OnReach;

    private struct Circle
    {
        public float r;
        public Vector3 o;
    }

    // Use this for initialization
    void Start ( )
    {
        keyCount = keyPositions.Length;
        if (keyCount == 0)
            return;

        m_speed = speeds[nextIndex];
        m_argular = Quaternion.Angle(transform.rotation, Quaternion.Euler(keyEulers[0])) 
            / Vector3.Distance(transform.position, keyPositions[0]) * m_speed;
    }

    // Update is called once per frame
    void Update ( )
    {
        if (nextIndex == keyCount)
            return;

        if (!HasReached())
        {
            MoveToNextPositionLine();
        }
        else
        {
            nextIndex++;
            if (nextIndex == keyCount)
                OnReach();
            else
            {
                m_speed = speeds[nextIndex];
                m_argular = Quaternion.Angle(transform.rotation, Quaternion.Euler(keyEulers[nextIndex])) 
                    / Vector3.Distance(transform.position, keyPositions[nextIndex]) * m_speed;
            }
        }
    }

    private Vector3 GetDirection (Vector3 euler)
    {
        Vector3 oldEuler = transform.eulerAngles;
        transform.eulerAngles = euler;
        Vector3 dir = transform.forward;
        transform.eulerAngles = oldEuler;
        return dir;
    }

    private Circle GetCirleFormNextKeyPos (Vector3 nextPos, Vector3 nextEuler)
    {
        Circle c = new Circle();
        Vector3 nextDir = GetDirection(nextEuler);
        bool atFront = DirectionAtRight(nextDir) == PositionAtRight(nextPos);
        return c;
    }

    private bool DirectionAtRight (Vector3 dir)
    {
        return Vector3.Dot(transform.right, dir) > 0;
    }

    private bool PositionAtRight (Vector3 pos)
    {
        return Vector3.Dot(transform.right, pos - transform.position) > 0;
    }

    private bool HasReached ( )
    {
        return Vector3.Distance(transform.position, keyPositions[nextIndex]) < distanceThreshold;
    }

    private void MoveToNextPositionLine ( )
    {
        // move along a line
        transform.position = Vector3.MoveTowards(transform.position, keyPositions[nextIndex], m_speed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(keyEulers[nextIndex]), m_argular * Time.deltaTime);
    }

}
