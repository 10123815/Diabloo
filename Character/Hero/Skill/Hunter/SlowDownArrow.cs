using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class SlowDownArrow : HunterSkill 
{

    public SlowDownArrow()
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        m_action.orbsType = GameCode.OrbsType.SlowDown;
        m_action.physicalAttackPrefab = m_action.orbsPrefabs[(int)GameCode.OrbsType.SlowDown].GetComponent<ShootTarget>().hitEffectPrefab;
    }
	
}
