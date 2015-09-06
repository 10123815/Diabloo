using UnityEngine;
using System.Collections;

public class Blizzard_Lasting : MageSkill
{

    static protected float m_duration = 2;

    public Blizzard_Lasting ( )
    {
        AreaFrozenEffect frost = m_action.iceRainPrefab.GetComponent<AreaFrozenEffect>();
        frost.duration = m_duration;
        frost.atk = (PlayerData.GetInstance().GetAttributeValue(fuckRPGLib.GameCode.BasicAttributeName.Intelligence) + 10) * Time.deltaTime;
        m_action.GetComponent<Animator>().SetBool("frost", true);
    }

    public static void OnLaunch ( )
    {
        PoolManager.GetInstance().GetPool(m_action.iceRainPrefab).GetObject(m_action.target.transform.position);
    }

}
