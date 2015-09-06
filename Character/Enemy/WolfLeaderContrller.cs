using UnityEngine;
using System.Collections;

public class WolfLeaderContrller : DogFaceControllerBase
{

    protected override void Init ( )
    {
        enemyId = fuckRPGLib.GameCode.EnemyID.WolfLeader;
        base.Init();
    }

    public GameObject clawPrefab;

    protected override void UpdateAnimation ( )
    {
        #region AnimatorStateInfo

        base.UpdateAnimation();

        #region combat
        if (m_anmSttInfo.IsName("0.fight"))
        {
            m_agent.Resume();
            m_agent.SetDestination(player.transform.position);
            m_animator.SetBool("atk0", m_data.atkRange + m_playerSize > m_distance);
            m_animator.SetBool("fight", false);
            m_animator.speed = anmMoveSpeed * m_data.moveSpeed / m_baseSpeed;
        }
        else if (m_anmSttInfo.IsName("0.atk0"))
        {
            if (m_agent.isOnNavMesh)
                m_agent.Stop();
            m_animator.speed = anmAtkSpeed * m_data.atkSpeed / m_baseAtkSpeed;
            if (m_animator.GetBool("atk0") && m_anmSttInfo.normalizedTime > 0.4f)
            {
                if (m_distance < m_data.atkRange * 2 && Random.value < m_data.hitRate)
                    PlayerData.GetInstance().Damaged(m_data.atk);
                else
                {
                    // miss
                }
                m_animator.SetBool("atk0", false);
                transform.LookAt(player.transform);
            }
        }
        else if (m_anmSttInfo.IsName("0.atk1"))
        {
            if (m_agent.isOnNavMesh)
                m_agent.Stop();
            m_animator.speed = anmAtkSpeed * m_data.atkSpeed / m_baseAtkSpeed;
            if (m_animator.GetBool("atk1") && m_anmSttInfo.normalizedTime > 0.4f)
            {
                if (m_distance < m_data.atkRange * 2 && Random.value < m_data.hitRate)
                {
                    PlayerData.GetInstance().Damaged(m_data.atk);
                    GameObject claw = PoolManager.GetInstance().GetPool(clawPrefab).GetObject();
                    claw.transform.SetParent(transform);
                    claw.transform.localPosition = new Vector3(0, 1.5f, 2);
                }
                else
                {
                    // miss
                }
                m_animator.SetBool("atk1", false);
                transform.LookAt(player.transform);
            }
        }
        else if (m_anmSttInfo.IsName("0.crit"))
        {
            if (m_agent.isOnNavMesh)
                m_agent.Stop();
            m_animator.speed = anmCritSpeed * m_data.atkSpeed / m_baseAtkSpeed;
            if (m_animator.GetBool("crit") && m_anmSttInfo.normalizedTime > 0.6f)
            {
                if (m_distance < m_data.atkRange * 2)
                {
                    PlayerData.GetInstance().Damaged(m_data.crit);
                }
                m_animator.SetBool("crit", false);
                transform.LookAt(player.transform);
            }
        }
        else if (m_anmSttInfo.IsName("0.batk"))
        {
            m_animator.speed = .5f;
            if (m_animator.GetBool("crit") || m_animator.GetBool("atk0") || m_animator.GetBool("atk1"))
                return;

            if (m_agent.isOnNavMesh)
                m_agent.Stop();
            if (m_data.atkRange + m_playerSize > m_distance)
            {
                m_animator.SetBool("crit", Random.value < m_data.critRate);
                if (!m_animator.GetBool("crit"))
                {
                    m_animator.SetBool("atk0", Random.value > 0.5f);
                    m_animator.SetBool("atk1", !m_animator.GetBool("atk0"));
                }
            }
            else
            {
                m_animator.SetBool("fight", true);
            }
        }
        #endregion

        #endregion

        #region AnimatorTransitionInfo
        if (m_anmTrsInfo.IsUserName("b2a"))
        {
            transform.LookAt(player.transform);
        }
        #endregion
    }

}
