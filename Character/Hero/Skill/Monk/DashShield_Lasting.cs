using UnityEngine;
using System.Collections;

public class DashShield_Lasting : Dash_Lasting
{

    static protected GameObject m_shieldPrefab;
    static protected GameObject m_shield;
    static protected float m_originDEF;
    static protected float m_augRate = 2;

    public DashShield_Lasting ( )
    {
        m_shieldPrefab = m_playerTF.gameObject.GetComponent<SkillManager>().shieldPrefab;
        m_originDEF = PlayerData.GetInstance().def;
    }

    static void OnLaunch ( )
    {
        Dash_Lasting.OnLaunch();
        PlayerData.GetInstance().def *= m_augRate;
        m_shield = PoolManager.GetInstance().GetPool(m_shieldPrefab).GetObject();
        m_shield.transform.SetParent(m_playerTF);
        m_shield.transform.localPosition = Vector3.zero;
    }

    static void OnStop()
    {
        Dash_Lasting.OnStop();
        PlayerData.GetInstance().def = m_originDEF;
        PoolManager.GetInstance().GetPool(m_shieldPrefab.name).GivebackObject(m_shield);
    }

}
