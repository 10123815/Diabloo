using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class Split_Lasting : SplitShot
{
       
    static protected float m_duration = 2;
    static protected float m_slowDownRate = 0.2f;

    public Split_Lasting()
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        orbsType = fuckRPGLib.GameCode.OrbsType.SlowDown;
        LastingShootTarget sst = m_action.orbsPrefabs[(int)GameCode.OrbsType.SlowDown].GetComponent<LastingShootTarget>();
        sst.time = m_duration;
        sst.d = m_slowDownRate;
    }

}
