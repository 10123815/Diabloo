using UnityEngine;
using System.Collections;

public class Frost_Lasting : IceBall
{

    public Frost_Lasting()
    {
        m_action.iceballPrefab.GetComponent<ShootTarget>().hitEffectPrefab = m_action.frostPrefab;
        int count = PoolManager.GetInstance().GetPool(m_action.iceballPrefab).GetCount();
        for (int i = 0; i < count; i++)
        {
            PoolManager.GetInstance().GetPool(m_action.iceballPrefab).GetObject(i).GetComponent<ShootTarget>().hitEffectPrefab = m_action.frostPrefab;
        }
    }

}
