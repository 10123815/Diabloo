//----------------------------------------------------
// Moving the camera smoothly around the player, and 
// avoiding obstacels auto, such as walls, buildings
//----------------------------------------------------

using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{

    public bool isOccluded = true;
    public bool isFixed = false;

    public float rotateSpeed = 5;
    private float m_maxRotateSpeed = 10000;
    public float smooth = 20;
    private float m_smooth;
    // the player
    private Transform target;
    public float distance = 8;
    private float m_distance;
    public float minDistance = 2;
    private Vector3 m_direction;
    private Vector3 m_origin;

    // Use this for initialization
    void Start ( )
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        m_direction = (transform.position - target.position).normalized;
    }

    // Update is called once per frame
    void Update ( )
    {
        RaycastHit hit = new RaycastHit();
        m_origin = new Vector3(target.position.x, target.position.y + 1, target.position.z);
        Vector3 offset = Vector3.zero;
        if (Physics.Raycast(m_origin, m_direction, out hit, distance * 1.2f) && isOccluded)
        {
            
            Debug.DrawLine(target.position, hit.point, Color.yellow);
            m_distance = Vector3.Distance(m_origin, hit.point) * 0.85f;
            m_smooth = smooth / 2;
            offset = hit.normal;
        }
        else
        {
            m_distance = distance;
            m_smooth = Mathf.Lerp(m_smooth, smooth, Time.deltaTime);
        }

        transform.position = Vector3.Lerp(transform.position, m_direction * m_distance + m_origin + offset, Time.deltaTime * m_smooth);
        transform.LookAt(m_origin);
    }

    public void Move (Vector2 axis)
    {
        if (isFixed)
            return;

        float ma = m_origin.y + distance * 0.8f;
        float mi = m_origin.y;
        if ((transform.position.y > ma && axis.y > 0) || (transform.position.y < mi && axis.y < 0))
            return;

        if (axis.magnitude > 0.15f)
        {
            m_direction = Quaternion.Euler(0, axis.x * rotateSpeed / 50, 0) * m_direction;
            m_direction.y += axis.y * rotateSpeed / m_maxRotateSpeed;
            m_direction = m_direction.normalized;
        }
    }
}
