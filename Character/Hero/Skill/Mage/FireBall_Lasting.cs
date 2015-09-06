using UnityEngine;
using System.Collections;

public class FireBall_Lasting : FireBall
{

    protected override void OnAddingEffect ( )
    {
        //m_fireBallIndex = 1;
        base.OnAddingEffect();
        m_action.fireballPrefab.AddComponent<SplitFireBall>();
        PoolManager.GetInstance().GetPool(m_action.fireballPrefab).AddComponent<SplitFireBall>();
    }

}
