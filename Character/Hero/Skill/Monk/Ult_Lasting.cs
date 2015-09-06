using UnityEngine;
using System.Collections;

public class Ult_Lasting : Ultrasonic
{

    static protected float m_augRate = 2.0f;
    static protected float m_desDef = 0.9f;
    static protected GameObject m_buffPrefab;
    static private GameObject m_buff;

    public Ult_Lasting ( )
    {
        m_buffPrefab = m_playerTF.gameObject.GetComponent<MonkAction>().madnessPrefab;
    }

    static public void OnLaunch ( )
    {
        PlayerData.GetInstance().atkSpeed *= m_augRate;
        PlayerData.GetInstance().def *= m_desDef;
        m_buff = PoolManager.GetInstance().GetPool(m_buffPrefab).GetObject();
        m_buff.transform.SetParent(m_playerTF);
        m_buff.transform.localPosition = Vector3.zero;
    }

    static public void OnStop ( )
    {
        PlayerData.GetInstance().atkSpeed /= m_augRate;
        PlayerData.GetInstance().def /= m_desDef;
        PoolManager.GetInstance().GetPool(m_buff.name).GivebackObject(m_buff);
    }
}
