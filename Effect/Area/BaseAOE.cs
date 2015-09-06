using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseAOE : MonoBehaviour
{

    //[System.NonSerialized]
    public float atk;

    protected Dictionary<int, BaseData> m_targets = new Dictionary<int, BaseData>();

    protected void AOE (Collider collider)
    {
        int id = collider.GetInstanceID();
        if (!m_targets.ContainsKey(id))
        {
            BaseData data = collider.GetComponent<BaseData>();
            if (data != null)
                m_targets.Add(collider.GetInstanceID(), data);
            else
            {
                Debug.LogError("BaseData is not found!");
            }
        }
        if (m_targets[id].curLife > 0)
        {
            m_targets[id].Damaged((atk + PlayerData.GetInstance().GetAttributeValue(fuckRPGLib.GameCode.BasicAttributeName.Intelligence) * 5) * Time.deltaTime);
            if (m_targets[id].curLife <= 0)
            {
                PlayerData.GetInstance().GainExp(m_targets[id].valueExp);
                PlayerData.GetInstance().gold += m_targets[id].valueGold;
            }
        }
    }
}
