using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ReturnToPool))]
public class LastingEffect : MonoBehaviour
{
    protected Controlled m_ctrl;
    private ReturnToPool m_rtp;

    public string type;

    // Use this for initialization
    void Start ( )
    {
    }

    public void Lasting (Transform target, float time, float d, bool isOverlay = false)
    {
        if (m_rtp == null)
            m_rtp = GetComponent<ReturnToPool>();
        m_rtp.deathTime = 0.1f + Mathf.Max(m_rtp.deathTime, time);
        GetControlled(target);
        if (m_ctrl.controlFuncDic.ContainsKey(type))
            StartCoroutine(m_ctrl.controlFuncDic[type](time, d));
    }

    protected void GetControlled (Transform tf)
    {
        m_ctrl = tf.GetComponent<Controlled>();
        if (m_ctrl == null)
            Debug.LogError("Controlled is not found!!");
    }

}
