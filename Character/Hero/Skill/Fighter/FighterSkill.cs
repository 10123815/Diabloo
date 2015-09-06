using UnityEngine;
using System.Collections;

public class FighterSkill : Skill
{

    static protected FighterAction m_action;

    public FighterSkill()
    {
        m_action = (FighterAction)PlayerData.GetInstance().action;
    }

}
