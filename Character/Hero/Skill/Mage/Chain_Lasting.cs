using UnityEngine;
using System.Collections;

public class Chain_Lasting : ElectricShock
{

    static public void OnLaunch()
    {
        for (int i = 0; i < m_skillMng.cds.Length; i++)
        {
            m_skillMng.cds[i] -= m_cdDecrease;
        }
        for (int i = 0; i < m_action.targets.Count; i++)
        {
            GameObject thunder = PoolManager.GetInstance().GetPool(m_action.thunderPrefab).GetObject(m_action.targets[i].transform.position);
            thunder.transform.SetParent(m_action.targets[i].transform);
            BaseData data = m_action.targets[i].GetComponent<BaseData>();
            if (data != null)
            {
                data.Damaged(m_baseAtk + PlayerData.GetInstance().GetAttributeValue(fuckRPGLib.GameCode.BasicAttributeName.Intelligence) * 2);
                if (data.curLife < 0)
                {
                    PlayerData.GetInstance().GainExp(data.valueExp);
                    PlayerData.GetInstance().gold += data.valueGold;
                }
            }
        }
    }

}
