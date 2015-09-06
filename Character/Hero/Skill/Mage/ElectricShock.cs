using UnityEngine;
using System.Collections;

public class ElectricShock : MageSkill
{

    protected static float m_baseAtk;

    protected static float m_cdDecrease = 1;
    protected static float m_duration = 3;

    static protected SkillManager m_skillMng;

    public ElectricShock ( )
    {
        m_skillMng = m_action.GetComponent<SkillManager>();
        m_skillMng.last[1] = m_duration;
    }

    static public void OnLaunch ( )
    {
        for (int i = 0; i < m_skillMng.cds.Length; i++)
        {
            m_skillMng.cds[i] -= m_cdDecrease;
        }
        GameObject light = PoolManager.GetInstance().GetPool(m_action.lightPrefab).GetObject(m_action.orbsBirth.position);
        light.transform.SetParent(m_playerTF);
        light.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    static public void OnStop ( )
    {
        for (int i = 0; i < m_skillMng.cds.Length; i++)
        {
            m_skillMng.cds[i] += 1;
        }
    }

}
