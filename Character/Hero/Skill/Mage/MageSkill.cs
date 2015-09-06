using UnityEngine;
using System.Collections;

public class MageSkill : Skill {

    static protected MageAction m_action;

    public MageSkill ( )
    {
        m_action = (MageAction)PlayerData.GetInstance().action;
    }

}
