using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class RangeAction : CharacterAction
{

    [System.NonSerialized]
    public int m_targetIndex = -1;

    [System.NonSerialized]
    public GameObject target;


    public GameObject[] orbsPrefabs;
    public int[] orbsHave;
    public Transform orbsBirth;

    [System.NonSerialized]
    public GameCode.OrbsType orbsType = GameCode.OrbsType.Normal;

    protected override void RangeAtk (int stage)
    {
        m_hasAtked = true;

        // shoot to the target
        Shoot(target, orbsPrefabs[(int)orbsType], false);
    }

    protected void SelectTarget ( )
    {
        // if the current target is on the front of hero
        // 0 = out of range, 1 = front, -1 = behind
        int isFront = 0;
        float curDis = 99999;

        // targeting the nearest target
        GameObject newFrontTarget = null;
        float minFrontDis = 9999;
        int indexFront = -1;
        GameObject newBehindTarget = null;
        float minBehindDis = 9999;
        int indexBehind = -1;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].tag == "dead")
            {
                targets.RemoveAt(i);
                UndoCheckEffect(i);
                continue;
            }

            Transform enemyTransform = targets[i].transform;
            float dis = Vector3.Distance(enemyTransform.position, transform.position);
            // target the enemy on the front of hero primly,
            // because player often want to shoot the enemy they face to
            if (Vector3.Dot(enemyTransform.position - transform.position, transform.forward) > 0.5f)
            {
                // at the front of...
                if (dis < minFrontDis)
                {
                    minFrontDis = dis;
                    newFrontTarget = targets[i];
                    indexFront = i;
                }

                if (target != null && target.GetInstanceID() == targets[i].GetInstanceID())
                {
                    isFront = 1;
                    curDis = dis;
                }
            }
            else
            {
                // at the behind of...
                if (dis < minBehindDis)
                {
                    minBehindDis = dis;
                    newBehindTarget = targets[i];
                    indexBehind = i;
                }

                if (target != null && target.GetInstanceID() == targets[i].GetInstanceID())
                    isFront = -1;
            }
        }

        if (newFrontTarget != null)
        {
            if (target == null)
            {
                target = newFrontTarget;
                m_targetIndex = indexFront;
                isFront = 1;
            }
            else if (target.GetInstanceID() == newFrontTarget.GetInstanceID())
            {

            }
            // the current is on the front of the hero and new target is much more close to the hero
            // or the current is on the behind of the hero
            else if ((isFront == 1 && minFrontDis < 0.9f * curDis) || isFront == -1)
            {
                UndoCheckEffect(m_targetIndex);
                target = newFrontTarget;
                m_targetIndex = indexFront;
            }
        }
        else if (newBehindTarget != null)
        {
            if (target == null)
            {
                target = newBehindTarget;
                m_targetIndex = indexBehind;
                isFront = -1;
            }
        }

        if (target != null)
            if (isFront != 0)
            {
                CheckEffect(m_targetIndex);
            }
            else
            {
                target = null;
                m_targetIndex = -1;
            }
    }


    // shoot one target with a kind of arrow and deal physical or magic damagement
    // if is not a magic, the last parameter is 0
    public void Shoot (GameObject target, GameObject orbsPrefab, bool isMagic, float magicBaseDamge = 0)
    {
        if (target == null)
            return;

        GameObject arrow = (GameObject)PoolManager.GetInstance().GetPool(orbsPrefab).GetObject(orbsBirth.position);
        arrow.transform.LookAt(transform.forward);
        ShootTarget shoot = arrow.GetComponent<ShootTarget>();
        if (shoot != null)
        {
            // reset all hit effect attached to this orb
            shoot.Reset();
            shoot.target = target.transform;
            shoot.sizeY = target.GetComponent<Collider>().bounds.size.y / 2;
            // a physical attack
            if (!isMagic)
            {
                if (Random.value < PlayerData.GetInstance().critRate)
                {
                    shoot.atk = PlayerData.GetInstance().crit;
                    shoot.hitEffectPrefab = critAttackPrefab;
                }
                else
                {
                    shoot.atk = PlayerData.GetInstance().atk;
                    shoot.hitEffectPrefab = physicalAttackPrefab;
                }
            }
            else
            {
                shoot.atk = PlayerData.GetInstance().GetAttributeValue(GameCode.BasicAttributeName.Intelligence) + magicBaseDamge;
            }
        }
    }
}
