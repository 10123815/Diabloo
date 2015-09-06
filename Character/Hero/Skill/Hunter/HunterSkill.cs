using UnityEngine;
using System.Collections;

public class HunterSkill : Skill
{

    static protected HunterAction m_action;

    public HunterSkill()
    {
        m_action = (HunterAction)PlayerData.GetInstance().action;
    }
}
