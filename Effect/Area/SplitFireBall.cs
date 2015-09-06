using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

public class SplitFireBall : MonoBehaviour
{

    //[System.NonSerialized]
    public bool isFirst = true;

    [System.NonSerialized]
    public List<GameObject> targets;

    private ShootTarget m_shoot;

    // Use this for initialization
    void Start ( )
    {
        m_shoot = GetComponent<ShootTarget>();
        // add "reset" at the first time Instaniate it
        if (!m_shoot.resetFuncDic.ContainsKey("reset split"))
            m_shoot.resetFuncDic.Add("reset split", Reset);
    }

    // Update is called once per frame
    void Update ( )
    {

    }

    public void OnTriggerEnter (Collider collider)
    {
        if (isFirst && collider.transform.GetInstanceID() == m_shoot.target.GetInstanceID())
        {
            // reset it for next usage
            isFirst = false;
            targets = PlayerData.GetInstance().action.targets;
            for (int i = 0; i < targets.Count; i++)
            {
                // a enemy that has been attacked by a fire ball will not be attacked by a splited fire ball
                if (targets[i].GetInstanceID() == collider.gameObject.GetInstanceID())
                    continue;

                // split this orbs
                GameObject orb = PoolManager.GetInstance().GetPool(name).GetObject(transform.position);
                LastingShootTarget shoot = orb.GetComponent<LastingShootTarget>();
                shoot.target = targets[i].transform;
                shoot.sizeY = targets[i].GetComponent<Collider>().bounds.size.y / 2;
                shoot.atk = PlayerData.GetInstance().GetAttributeValue(GameCode.BasicAttributeName.Intelligence) + m_shoot.atk / targets.Count;

                SplitFireBall split = orb.GetComponent<SplitFireBall>();
                split.isFirst = false;
            }

        }
    }

    public void Reset ( )
    {
        isFirst = true;
    }
}
