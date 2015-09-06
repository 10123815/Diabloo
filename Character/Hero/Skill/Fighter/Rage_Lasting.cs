using UnityEngine;
using System.Collections;

public class Rage_Lasting : FighterSkill
{

    static protected GameObject m_rageEffect;
    static protected float m_atkAug;

    public Rage_Lasting()
    {
    
    }

    static public void OnLaunch()
    {
        PlayerData.GetInstance().atk += m_atkAug;
        m_rageEffect = PoolManager.GetInstance().GetPool(m_action.ragePrefab).GetObject();
        m_rageEffect.transform.SetParent(m_playerTF);
        m_rageEffect.transform.localPosition = Vector3.zero;
    }

    static public void OnStop()
    {
        PlayerData.GetInstance().atk -= m_atkAug;
        PoolManager.GetInstance().GetPool(m_rageEffect.name).GivebackObject(m_rageEffect);
    }
}
