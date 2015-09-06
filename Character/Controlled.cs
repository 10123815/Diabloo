using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

public class Controlled : MonoBehaviour
{

    public bool isAntiCtrl = false;

    //[System.NonSerialized]
    public bool canMove = true;

    public delegate IEnumerator Control (float t, float d);
    [System.NonSerialized]
    public Dictionary<string, Control> controlFuncDic = new Dictionary<string, Control>();

    [System.NonSerialized]
    public GameObject controller;

    private bool m_isPlayer;
    private BaseData m_data;

    private float m_slowRate = 0;

    // Use this for initialization
    void Start ( )
    {
        m_isPlayer = tag.Equals("Player");
        if (!m_isPlayer)
        {
            m_data = GetComponent<BaseData>();
            if (m_data == null)
                Debug.LogError("BaseData is not found!");
        }
        else
            m_data = PlayerData.GetInstance();

        controlFuncDic.Add("knock up", KnockedUp);
        controlFuncDic.Add("knock back", KnockedBack);
        controlFuncDic.Add("stun", Stun);
        controlFuncDic.Add("slow", SlowDown);
        controlFuncDic.Add("firing", Firing);
    }

    // Update is called once per frame
    void Update ( )
    {

    }

    public IEnumerator KnockedUp (float time, float height)
    {
        if (isAntiCtrl)
            time = 0;

        canMove = false;
        Vector3 pp = transform.position + new Vector3(0, height, 0);
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            if (i < time / 2)
                transform.position = Vector3.Lerp(transform.position, pp, Time.deltaTime * height / (time / 2));
            else
                transform.position -= new Vector3(0, Time.deltaTime * height / (time / 2), 0);
            yield return 0;
        }
        canMove = true;
    }

    public IEnumerator KnockedBack (float time, float distance)
    {
        if (isAntiCtrl)
            time = 0;

        canMove = false;
        if (controller == null)
            Debug.LogError("No Controller");
        Vector3 dir = (transform.position - controller.transform.position).normalized;
        dir.y = 0;
        Vector3 p = dir * distance + transform.position;
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(transform.position, p, Time.deltaTime * distance / time);
            yield return 0;
        }
        canMove = true;
    }

    public IEnumerator Stun (float time, float d = 0)
    {
        if (isAntiCtrl)
            time = 0;


        canMove = false;
        for (float i = 0; i < time; i += Time.deltaTime)
            yield return 0;
        canMove = true;
    }

    public IEnumerator SlowDown (float time, float d)
    {
        if (isAntiCtrl)
            time = 0;


        m_slowRate = m_data.moveSpeed * d;
        m_data.moveSpeed -= m_slowRate;

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            yield return 0;
        }

        ResetSpeed();
    }

    public IEnumerator Firing (float time, float d)
    {
        if (isAntiCtrl)
            time = 0;


        for (float i = 0; i < time; i += Time.deltaTime)
        {
            if (m_data.curLife > 0)
            {
                m_data.Damaged(d + Time.deltaTime * PlayerData.GetInstance().GetAttributeValue(GameCode.BasicAttributeName.Intelligence) * 5);
                if (m_data.curLife <= 0)
                {
                    PlayerData.GetInstance().GainExp(m_data.valueExp);
                    PlayerData.GetInstance().gold += m_data.valueGold;
                }
            }
            yield return 0;
        }
    }

    public void ResetSpeed ( )
    {
        if (m_isPlayer)
        {
            PlayerData.GetInstance().moveSpeed += m_slowRate;
        }
        else
        {
            m_data.moveSpeed += m_slowRate;
        }
    }
}
