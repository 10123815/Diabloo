using UnityEngine;
using System.Collections;

public class Shield : MageSkill
{

    static protected GameObject m_shield;
    static protected float m_defAug = 50;
    protected float m_baseAtk = 5;

    public Shield()
    {
        SkillManager skillMng = m_action.GetComponent<SkillManager>();
        if (skillMng == null)
            Debug.LogError("A hero must have a SkillManager");
        for (int i = 0; i < skillMng.launchFuncs.Length; i++)
        {
            if (skillMng.launchFuncs[i] != null)
            {
                skillMng.launchFuncs[i] += ExtraOnLaunch;
                skillMng.stopFuncs[i] += ExtraOnStop;
            }
        }
    }

    public void ExtraOnLaunch( )
    {
        PlayerData.GetInstance().def += m_defAug;
        m_shield = PoolManager.GetInstance().GetPool(m_action.shieldPrefab).GetObject(m_playerTF.position + new Vector3(0, 1, 0));
        m_shield.transform.SetParent(m_playerTF);
        m_shield.GetComponent<EleShieldEffect>().atk = Time.deltaTime * (PlayerData.GetInstance().GetAttributeValue(fuckRPGLib.GameCode.BasicAttributeName.Intelligence) * 0.5f + m_baseAtk);
    }

    public void ExtraOnStop()
    {
        PoolManager.GetInstance().GetPool(m_action.shieldPrefab.name).GivebackObject(m_shield);
        PlayerData.GetInstance().def -= m_defAug;
    }

}
