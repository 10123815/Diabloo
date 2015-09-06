using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class FighterAction : CharacterAction
{

    private bool m_hasAdjusted;

    public bool hasLearnDash = false;

    public GameObject defensePrefab;
    public GameObject ragePrefab;
    public GameObject innerVitalityPrefab;

    // every attacking action of a Fighter will engage a area of enemies, not a single enemy
    override protected void MeleeAtk (int stage)
    {
        m_hasAtked = true;

        if (targets.Count == 0)
            return;

        float atkRange = PlayerData.GetInstance().atkRange;
        float atk = PlayerData.GetInstance().atk;
        GameCode.PhysicalAttackType atkType = (GameCode.PhysicalAttackType)stage;
        switch (atkType)
        {
            case GameCode.PhysicalAttackType.Crit:
                atkRange *= 1.5f;
                atk = PlayerData.GetInstance().crit;
                StartCoroutine(CameraShake(0.3f));
                break;
            case GameCode.PhysicalAttackType.HeavyAtk:
                atk *= 1.1f;
                break;
            case GameCode.PhysicalAttackType.LightAtk:
                atk *= 0.9f;
                break;
            case GameCode.PhysicalAttackType.None:
                return;
            default:
                break;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            
            // look at the target
            LookAtTarget(targets[0].transform);
            // add a "checking" effect to this enemy
            CheckEffect(i);

            if (stage == 2)
            {
                ControlEnemy(i, atk, physicalAttackPrefab, "knock up", 1, 2);
            }
            else
            {
                if (true /*Random.value < PlayerData.GetInstance().hitRate*/)
                { 
                    Damage(i, atk, physicalAttackPrefab);
                }
                else
                {
                    // miss text effect
                }
            }

        }
    }

    public override void Animate (Vector3 movement, bool atking, bool[] skills)
    {
        base.Animate(movement, atking, skills);

        #region skill input

        if (skills[0] && m_skillManager.manaCosts[0] <= PlayerData.GetInstance().curMana)
        {
            m_animator.SetBool("leap", true);
        }
        else if (skills[1] && m_skillManager.manaCosts[0] <= PlayerData.GetInstance().curMana)
        {
            m_animator.SetBool("def", true);
        }

        #endregion

        #region AnimatorStateInfo

        m_animator.SetBool("moving", movement.magnitude > 0.1f);
        if (m_anmSttInfo.IsName("0.leap"))
        {
            if (m_animator.GetBool("leap"))
            {
                m_agent.speed = 0;
                m_agent.angularSpeed = 0;
                m_animator.SetBool("leap", false);
                m_skillManager.Skill(0);
                StartCoroutine(JumpAndDrop(0.3f, 3));
            }
            if (atking)
                m_animator.SetBool("atk", hasLearnDash);
            if (m_anmSttInfo.normalizedTime > 0.6f)
                m_animator.SetBool("moving", !m_animator.GetBool("atk") && movement.magnitude > 0.1f);
        }
        else if (m_anmSttInfo.IsName("0.dash"))
        {
            if (m_anmSttInfo.normalizedTime > 0.4f && m_anmSttInfo.normalizedTime < 0.5f && m_animator.GetBool("atk"))
            {
                m_animator.SetBool("atk", false);
                for (int i = 0; i < targets.Count; i++)
                {
                    ControlEnemy(i, PlayerData.GetInstance().atk, critAttackPrefab, "knock up", 0.5f);
                }
            }
            if (m_anmSttInfo.normalizedTime > 0.4f && m_anmSttInfo.normalizedTime < 0.7f)
                transform.Translate(Vector3.forward * Time.deltaTime * 40, Space.Self);
            if (m_anmSttInfo.normalizedTime > 0.8f)
                m_animator.SetBool("moving", movement.magnitude > 0.1f);
        }
        else if (m_anmSttInfo.IsName("0.atk0") || m_anmSttInfo.IsName("0.atk2"))
        {
            if (skills[2] && m_skillManager.manaCosts[0] <= PlayerData.GetInstance().curMana)
                m_animator.SetBool("dev", true);
        }
        else if (m_anmSttInfo.IsName("0.dev0") || m_anmSttInfo.IsName("0.dev1"))
        {

            if (m_anmSttInfo.normalizedTime < 0.2f)
                m_animator.SetBool("dev", false);

            m_meleeWeaponTrail.Emit = true;
            if (m_anmSttInfo.normalizedTime > 0.8f)
                m_meleeWeaponTrail.Emit = false;

            if (m_anmSttInfo.normalizedTime > 0.7f && m_anmSttInfo.normalizedTime < 0.9f && !m_hasAtked)
            {
                MeleeAtk(1);
            }
            if (skills[2] && m_skillManager.manaCosts[0] <= PlayerData.GetInstance().curMana)
                m_animator.SetBool("dev", true);
        }
        else if (m_anmSttInfo.IsName("0.dev2"))
        {
            m_meleeWeaponTrail.Emit = true;
            if (m_anmSttInfo.normalizedTime > 0.8f)
                m_meleeWeaponTrail.Emit = false;

            if (m_anmSttInfo.normalizedTime > 0.7f && m_anmSttInfo.normalizedTime < 0.9f && !m_hasAtked)
            {
                MeleeAtk(1);
                m_skillManager.Skill(2);
                m_animator.SetBool("dev", false);
            }

            if (atking)
                m_animator.SetBool("atk", true);
        }

        else if (m_anmSttInfo.IsName("0.defense"))
        {
            if (m_animator.GetBool("def"))
            {
                m_animator.SetBool("def", false);
                m_skillManager.Skill(1);
            }

            if (m_anmSttInfo.normalizedTime > 0.6f)
                m_animator.SetBool("moving", movement.magnitude > 0.1f);
        }
        #endregion

        #region AnimatorTransitionInfo

        if (m_anmTraInfo.IsUserName("2d"))
        {
            if (!m_hasAdjusted)
            {
                m_hasAdjusted = true;
                m_hasAtked = false;
                m_agent.Stop();
                m_agent.speed = 0;
                m_agent.angularSpeed = 0;
            }
        }
        else if (m_anmTraInfo.IsUserName("d2i") || m_anmTraInfo.IsUserName("d2r"))
        {
            if (m_hasAdjusted)
            {
                m_hasAdjusted = false;
                m_animator.SetBool("dev", false);
                m_agent.angularSpeed = 9999999999;
                m_agent.Resume();
                m_skillManager.Skill(2);
            }
        }

        #endregion

    }



}
