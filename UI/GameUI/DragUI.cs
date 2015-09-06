using UnityEngine;
using System.Collections;

public class DragUI : MonoBehaviour
{

    // the offset from drag point to center of UI
#if UNITY_EDITOR
    private Vector3 m_dragOffset;
#else
    private Vector2 m_dragOffset;
#endif

    // Use this for initialization
    void Start ( )
    {

    }


    public void OnDragBegin ( )
    {
        
#if UNITY_EDITOR
        Vector3 pos = transform.position;
        m_dragOffset = pos - Input.mousePosition;
#else
        Vector2 pos = transform.position;
        m_dragOffset = pos - Input.GetTouch(0).position;
#endif
    }

    public void OnDrag ( )
    {
#if UNITY_EDITOR
        Vector3 pos = Input.mousePosition + m_dragOffset;
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10);
#else
        Vector3 pos = Input.GetTouch(0).position + m_dragOffset;
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10);
#endif
    }
}
