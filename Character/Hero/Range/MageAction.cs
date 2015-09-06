using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class MageAction : RangeAction
{
    public GameObject fireballPrefab;
    public GameObject iceballPrefab;
    public GameObject lightPrefab;
    public GameObject thunderPrefab;
    public GameObject frostPrefab;
    public GameObject shieldPrefab;
    public GameObject iceRainPrefab;

    public override void Animate (Vector3 movement, bool atking, bool[] skills)
    {

        // update the target every 10 frame
        if (Time.frameCount % 10 == 0)
            SelectTarget();

        #region AnimatorStateInfo
        m_anmSttInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        if (m_anmSttInfo.IsName("0.idle"))
        {
            m_agent.Resume();
            GetSkillInput(skills);
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            m_animator.SetBool("atk", atking);
        }
        else if (m_anmSttInfo.IsName("0.run"))
        {
            m_agent.Resume();
            GetSkillInput(skills);
            m_agent.speed = PlayerData.GetInstance().moveSpeed;
            m_agent.angularSpeed = 9999999999;
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            m_animator.SetBool("atk", atking);
            m_animator.speed = m_agent.speed / m_baseMoveSpeed;
        }
        else if (m_anmSttInfo.IsName("0.atk0"))
        {
            m_agent.Stop();
            AtkAnimate(atking, m_anmSttInfo.normalizedTime, 0);
        }
        else if (m_anmSttInfo.IsName("0.atk1"))
        {
            m_agent.Stop();
            AtkAnimate(atking, m_anmSttInfo.normalizedTime, 0);
        }
        else if (m_anmSttInfo.IsName("0.fire"))
        {
            m_agent.Stop();
            if (m_anmSttInfo.normalizedTime > 0.1f && m_animator.GetBool("fire"))
            {
                m_animator.SetBool("fire", false);
                m_skillManager.Skill(0);
            }
        }
        else if (m_anmSttInfo.IsName("0.ice"))
        {
            m_agent.Stop();
            if (m_anmSttInfo.normalizedTime > 0.1f && m_animator.GetBool("ice"))
            {
                m_animator.SetBool("ice", false);
                m_skillManager.Skill(2);
            }
        }
        else if (m_anmSttInfo.IsName("0.charge"))
        {
            if (m_animator.GetBool("ele") && m_anmSttInfo.normalizedTime < 0.5f)
            {
                m_animator.SetBool("ele", false);
                ChargeEffect(orbsBirth.position);
            }

            if (m_anmSttInfo.normalizedTime > 0.5f)
            {
                m_animator.SetBool("ele", true);
            }
        }
        else if (m_anmSttInfo.IsName("0.ele"))
        {
            if (m_animator.GetBool("ele"))
            {
                m_animator.SetBool("ele", false);
                m_skillManager.Skill(1);
            }
        }
        else if (m_anmSttInfo.IsName("0.ice rain"))
        {
            if (m_anmSttInfo.normalizedTime > 0.1f && m_animator.GetBool("ice"))
            {
                m_animator.SetBool("ice", false);
                m_skillManager.Skill(2);
            }
        }

        #endregion

        #region AnimatorTransitionInfo
        m_anmTraInfo = m_animator.GetAnimatorTransitionInfo(0);

        if (m_anmTraInfo.IsUserName("2a"))
        {
            m_animator.SetBool("atk", false);
            m_hasAtked = false;
            m_agent.speed = 0;
            m_agent.angularSpeed = 0;
            m_agent.Stop();
            if (target != null)
                LookAtTarget(target.transform);
        }
        if (m_anmTraInfo.IsUserName("a2i"))
        {
            m_animator.SetBool("atk", false);
        }
        #endregion
    }

    private void GetSkillInput (bool[] skills)
    {
        if (skills[0] && PlayerData.GetInstance().curMana >= m_skillManager.manaCosts[0])
        {
            m_animator.SetBool("fire", true);
        }
        else if (skills[2] && PlayerData.GetInstance().curMana >= m_skillManager.manaCosts[2])
        {
            m_animator.SetBool("ice", true);
        }
        else if (skills[1] && PlayerData.GetInstance().curMana >= m_skillManager.manaCosts[1])
        {
            m_animator.SetBool("ele", true);
        }
    }
}
