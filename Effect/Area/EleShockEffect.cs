using UnityEngine;
using System.Collections;

public class EleShockEffect : MonoBehaviour
{

    public float baseAtk;

    // Use this for initialization
    void Start ( )
    {

    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Enemy"))
        {
            BaseData data = collider.GetComponent<BaseData>();
            if (data != null)
            {
                data.Damaged(PlayerData.GetInstance().GetAttributeValue(fuckRPGLib.GameCode.BasicAttributeName.Intelligence) * 2 + baseAtk);
                if (data.curLife < 0)
                {
                    PlayerData.GetInstance().GainExp(data.valueExp);
                    PlayerData.GetInstance().gold += data.valueGold;
                }
            }
        }
    }
}
