using UnityEngine;
using System.Collections;
public class Madness : HunterSkill
{

    static protected GameObject m_madnessEffect;
    static protected float m_speedAugRate = .5f;
    static protected float m_atkSpeedAugRate = .5f;
    static protected float m_sa;
    static protected float m_aa;

    public Madness()
    {
        m_sa = PlayerData.GetInstance().moveSpeed * m_speedAugRate;
        m_aa = PlayerData.GetInstance().atkSpeed * m_atkSpeedAugRate;
    }

    static public void OnLaunch()
    {
        m_madnessEffect = PoolManager.GetInstance().GetPool(m_action.madnessPrefab).GetObject();
        m_madnessEffect.transform.SetParent(m_playerTF);
        m_madnessEffect.transform.localPosition = Vector3.zero;
        PlayerData.GetInstance().moveSpeed += m_sa;
        PlayerData.GetInstance().atkSpeed += m_aa;
    }

    static public void OnStop()
    {
        PoolManager.GetInstance().GetPool(m_madnessEffect.name).GivebackObject(m_madnessEffect);
        PlayerData.GetInstance().moveSpeed -= m_sa;
        PlayerData.GetInstance().atkSpeed -= m_aa;
    }

}
