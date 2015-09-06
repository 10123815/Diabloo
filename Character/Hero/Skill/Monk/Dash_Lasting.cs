using UnityEngine;
using System.Collections;

public class Dash_Lasting : Skill
{

    static protected float m_augRate = 2.0f;

    public Dash_Lasting ( )
    {

    }

    // a active skill must both have a "Launch", a "Lasting" and a "Stop"
    // an instantaneous effect when ues this skill
    // only called one time
    static public void OnLaunch ( )
    {
        PlayerData.GetInstance().moveSpeed *= m_augRate;
    }

    // a lasting effect, called every frame at duration
    // a normalized time t
    static public void OnLasting (float normalizedTime)
    {

    }

    // a skill has already stopped
    static public void OnStop ( )
    {
        PlayerData.GetInstance().moveSpeed /= m_augRate;
    }

}
