using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class SplitShot : HunterSkill
{
    static protected GameCode.OrbsType orbsType = GameCode.OrbsType.Normal;

    public SplitShot ( )
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        base.OnAddingEffect();
    }

    public static void OnLaunch ( )
    {
        for (int i = 0; i < m_action.targets.Count; i++)
        {
            Vector3 dir = m_action.targets[i].transform.position - m_action.transform.position;
            if (Vector3.Dot(dir.normalized, m_action.transform.forward) > 0)
                m_action.Shoot(m_action.targets[i], m_action.orbsPrefabs[(int)orbsType], true);
        }
    }

    public static void OnLasting (float t)
    {
    }

    public static void OnStop ( )
    {
    }
}
