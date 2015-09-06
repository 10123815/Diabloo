using UnityEngine;
using System.Collections;

public class ChargeShot : HunterSkill
{

    static protected float m_baseDamage = 30;

    public ChargeShot ( )
    {

    }

    static public void OnLaunch ( )
    {
        m_action.Shoot(m_action.target, m_action.pierceArrowPrefab, true, m_baseDamage);
    }

    static public void OnLasting (float t)
    {
    }

    static public void OnStop ( )
    {
    }
    
}
