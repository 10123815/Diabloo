using UnityEngine;
using System.Collections;

public class NinjaController : DogFaceControllerBase
{

    protected override void Init ( )
    {
        enemyId = fuckRPGLib.GameCode.EnemyID.Ninja;
        base.Init();
    }

    public float dodgeDis = 3;

    override protected void UpdateAnimation ( )
    {

        #region AnimatorStateInfo

        base.UpdateAnimation();

        #region combat state
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
            m_agent.Stop();
            m_animator.speed = anmAtkSpeed * m_data.atkSpeed / m_baseAtkSpeed;
            if (m_animator.GetBool("atk0") && m_anmSttInfo.normalizedTime > 0.4f)
            {
                if (m_distance < m_data.atkRange * 1.5f && Random.value < m_data.hitRate)
                    PlayerData.GetInstance().Damaged(m_data.atk);
                else
                {
                    // miss
                }
                m_animator.SetBool("atk0", false);
            }
        }
        else if (m_anmSttInfo.IsName("0.atk1"))
        {
            m_agent.Stop();
            m_animator.speed = anmAtkSpeed * m_data.atkSpeed / m_baseAtkSpeed;
            if (m_animator.GetBool("atk1") && m_anmSttInfo.normalizedTime > 0.6f)
            {
                if (m_distance < m_data.atkRange * 1.5f)
                    PlayerData.GetInstance().Damaged(m_data.crit);
                else
                {
                    // miss
                }
                m_animator.SetBool("atk1", false);
            }
        }
        else if (m_anmSttInfo.IsName("0.batk"))
        {
            m_animator.speed = 3;
            if (m_animator.GetBool("roll") || m_animator.GetBool("atk0") || m_animator.GetBool("atk1"))
                return;

            m_agent.Stop();
            m_animator.SetBool("roll", Random.value < m_data.dodgeRate);
            if (m_animator.GetBool("roll"))
            {
                Vector3 normal = (transform.position - player.transform.position).normalized;
                normal = Quaternion.Euler(0, (Random.value - 1) * 90, 0) * normal;
                m_dodgePosition = transform.position + normal * dodgeDis;
                m_agent.Resume();
            }
            else if (m_data.atkRange + m_playerSize > m_distance)
            {
                m_animator.SetBool("atk0", Random.value > m_data.critRate);
                m_animator.SetBool("atk1", !m_animator.GetBool("atk0"));
            }
            else
            {
                m_animator.SetBool("fight", true);
            }

        }
        else if (m_anmSttInfo.IsName("0.roll"))
        {
            m_agent.SetDestination(m_dodgePosition);
            m_animator.SetBool("roll", false);
            m_animator.speed = 1;
        }
        #endregion

        #endregion

        #region AnimatorTransitionInfo
        if (m_anmTrsInfo.IsUserName("b2a"))
        {
            transform.LookAt(player.transform);
        }
        else if (m_anmTrsInfo.IsUserName("r2b"))
        {
            transform.LookAt(player.transform);
        }
        #endregion

    }
}
