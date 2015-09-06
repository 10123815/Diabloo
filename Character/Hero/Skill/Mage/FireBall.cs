using UnityEngine;
using System.Collections;

public class FireBall : MageSkill
{
    protected static float m_baseAtk = 30;
    protected static float m_time = 4f;
    protected static float m_atk = 10 * Time.deltaTime;

    protected static int m_fireBallIndex = 0;

    public FireBall()
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        LastingShootTarget firingEffect = m_action.fireballPrefab.GetComponent<LastingShootTarget>();
        firingEffect.time = m_time;
        firingEffect.d = m_atk;
    }

    static public void OnLaunch()
    {
        m_action.Shoot(m_action.target, m_action.fireballPrefab, true, m_baseAtk);
    }
}
