using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class LeapAttack : FighterSkill
{

    static protected float m_baseAtk = 20;
    static protected float m_ctrlTime = 0.2f;
    static protected float m_ctrlDegree = 2;

    public LeapAttack()
    {
        AreaControllSkill arctrlSkill = m_action.areaControlPrefab.GetComponent<AreaControllSkill>();
        arctrlSkill.targetTag = "Enemy";
        arctrlSkill.controlType = "knock back";
        arctrlSkill.time = m_ctrlTime;
        arctrlSkill.d = m_ctrlDegree;
        arctrlSkill.atk = m_baseAtk + PlayerData.GetInstance().GetAttributeValue(GameCode.BasicAttributeName.Intelligence);
    }

    public static void OnStop()
    {
        GameObject groundShock = PoolManager.GetInstance().GetPool(m_action.groundEffectPrefab).GetObject(m_playerTF.position);
        GameObject areaCtrl = PoolManager.GetInstance().GetPool(m_action.areaControlPrefab).GetObject();
        areaCtrl.transform.SetParent(m_playerTF);
        areaCtrl.transform.localPosition = Vector3.zero;
    }

}
