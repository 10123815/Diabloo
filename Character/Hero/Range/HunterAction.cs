using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class HunterAction : RangeAction
{

    public GameObject pierceArrowPrefab;
    public GameObject beatBackAttowPrefab;

    // when at charging, if player is sliding joystick, the charging won't be discarded;
    // but if player has loosen joystick and then slide it, player will discard charging
    private bool m_discardCharge = false;

    public override void Animate (Vector3 movement, bool atking, bool[] skills)
    {

        // update the target every 10 frame
        if (Time.frameCount % 10 == 0)
            SelectTarget();

        if (skills[2])
            m_skillManager.Skill(2);

        #region AnimatorStateInfo
        m_anmSttInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (m_anmSttInfo.IsName("0.idle"))
        {
            GetSkillInput(skills);
            m_animator.SetBool("atk", atking);
            m_agent.Resume();
            m_agent.speed = PlayerData.GetInstance().moveSpeed;
            m_agent.angularSpeed = 9999999999;
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
        }
        else if (m_anmSttInfo.IsName("0.run"))
        {
            GetSkillInput(skills);
            if (atking)
                m_animator.SetBool("atk", true);
            m_discardCharge = false;
            m_agent.Resume();
            m_agent.speed = PlayerData.GetInstance().moveSpeed;
            m_agent.angularSpeed = 9999999999;
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            m_animator.speed = m_agent.speed / m_baseMoveSpeed;
        }
        else if (m_anmSttInfo.IsName("0.pre"))
        {
            m_agent.Stop();
        }
        else if (m_anmSttInfo.IsName("0.atk"))
        {
            // the hunter do not have combos
            m_animator.SetBool("atk", false);
            AtkAnimate(false, m_anmSttInfo.normalizedTime, 0);
        }
        else if (m_anmSttInfo.IsName("0.preslpit"))
        {
            m_agent.Stop();
        }
        else if (m_anmSttInfo.IsName("0.split"))
        {
            if (m_anmSttInfo.normalizedTime > 0.5f && !m_hasAtked)
            {
                m_skillManager.Skill(0);
                m_hasAtked = true;
            }
        }
        else if (m_anmSttInfo.IsName("0.precharge"))
        {
            // stop and then move to discard charging
            if (!m_discardCharge)
            {
                m_discardCharge = movement.magnitude < 0.1f;
                m_animator.SetBool("moving", false);
            }
            else
            {
                m_animator.SetBool("moving", m_anmSttInfo.normalizedTime < 0.9f && movement.magnitude > 0.1f);
            }

            if (m_animator.GetBool("charge"))
            {
                m_animator.SetBool("charge", false);
                ChargeEffect();
            }
        }
        else if (m_anmSttInfo.IsName("0.charge"))
        {
            if (m_anmSttInfo.normalizedTime > 0.5f && !m_hasAtked)
            {
                // shoot with a big arrow before charging
                m_skillManager.Skill(1);
                m_hasAtked = true;
                m_discardCharge = false;
            }
        }

        if (m_anmSttInfo.IsTag("a"))
        {
            if (m_anmSttInfo.normalizedTime > 0.65f && movement.magnitude > 0.1f)
            {
                m_animator.SetBool("moving", true);
                m_agent.enabled = true;
                m_agent.Resume();
                m_agent.speed = PlayerData.GetInstance().moveSpeed;
                m_agent.angularSpeed = 9999999999;
            }
        }

        #endregion

        #region AnimatorTransitionInfo
        m_anmTraInfo = m_animator.GetAnimatorTransitionInfo(0);

        if (m_anmTraInfo.IsUserName("2a"))
        {
            m_animator.SetBool("atk", false);
            // prepare to shoot
            m_hasAtked = false;
            m_agent.speed = 0;
            m_agent.angularSpeed = 0;
            m_agent.Stop();
            if (target != null)
                LookAtTarget(target.transform);
        }
        else if (m_anmTraInfo.IsUserName("a2i"))
        {
            m_hasAtked = false;
            m_animator.SetBool("atk", false);
            m_agent.speed = PlayerData.GetInstance().moveSpeed;
        }
        else if (m_anmTraInfo.IsName("a2"))
        {
            m_agent.enabled = true;
            m_agent.Resume();
            m_agent.speed = PlayerData.GetInstance().moveSpeed;
            m_agent.angularSpeed = 9999999999;
        }
        #endregion
    }

    // this method will only be called in one frame, because the skill will enter in CD immediatly,
    // and the button will be disabled
    private void GetSkillInput (bool[] skills)
    {
        if (skills[2])
            m_skillManager.Skill(2);
        if (skills[1])
            m_animator.SetBool("charge", m_skillManager.manaCosts[1] < PlayerData.GetInstance().curMana);
        m_animator.SetBool("split", skills[0] && m_skillManager.manaCosts[0] < PlayerData.GetInstance().curMana);

    }
}
