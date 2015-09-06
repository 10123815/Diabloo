using UnityEngine;
using System.Collections;

public class ChargeShot_Lasting : ChargeShot
{

    public ChargeShot_Lasting()
    {
        m_baseDamage = 40;
    }

    static public void OnLaunch ( )
    {
        m_action.Shoot(m_action.target, m_action.beatBackAttowPrefab, true, m_baseDamage);
    }

}
