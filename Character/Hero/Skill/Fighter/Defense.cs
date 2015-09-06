using UnityEngine;
using System.Collections;

public class Defense : FighterSkill
{

    static protected GameObject m_defEffect;
    //static protected float m_defAugRate = 0.4f;
    static protected float m_defAug = 200;

    public Defense ( )
    {

    }

    static public void OnLaunch ( )
    {
        PlayerData.GetInstance().def += m_defAug;
        m_defEffect = PoolManager.GetInstance().GetPool(m_action.defensePrefab).GetObject();
        m_defEffect.transform.SetParent(m_playerTF);
        m_defEffect.transform.localPosition = new Vector3(0, 1, 0);
    }

    static public void OnStop ( )
    {
        PoolManager.GetInstance().GetPool(m_defEffect.name).GivebackObject(m_defEffect);
        PlayerData.GetInstance().def -= m_defAug;
    }

}
