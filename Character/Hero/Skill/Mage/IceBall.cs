using UnityEngine;
using System.Collections;

public class IceBall : MageSkill
{
    protected static float m_baseAtk = 30;
    protected static float m_time = 3;
    protected static float m_slowRate = 0.2f;

    public IceBall ( )
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        LastingShootTarget slow = m_action.iceballPrefab.GetComponent<LastingShootTarget>();
        slow.time = m_time;
        slow.d = m_slowRate;
    }

    static public void OnLaunch ( )
    {
        m_action.Shoot(m_action.target, m_action.iceballPrefab, true, m_baseAtk);
    }
}
