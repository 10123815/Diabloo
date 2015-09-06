using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class MonkAction : CharacterAction
{
    public TrailRenderer trail;
    public Vector2[] ariAtktimes;

    private bool m_jumped;
    private Coroutine jump;

    // monk only attack one enemy at one time
    protected override void MeleeAtk (int stage)
    {
        m_hasAtked = true;
        if (targets.Count == 0)
        {
            return;
        }

        LookAtTarget(targets[0].transform);
        CheckEffect(0);

        switch ((GameCode.PhysicalAttackType)stage)
        {
            case GameCode.PhysicalAttackType.Crit:
                Damage(0, PlayerData.GetInstance().crit, critAttackPrefab);
                break;
            case GameCode.PhysicalAttackType.HeavyAtk:
                if (Random.value < PlayerData.GetInstance().hitRate)
                {
                    Damage(0, PlayerData.GetInstance().atk * 1.1f, physicalAttackPrefab);
                }
                break;
            case GameCode.PhysicalAttackType.LightAtk:
                if (Random.value < PlayerData.GetInstance().hitRate)
                {
                    Damage(0, PlayerData.GetInstance().atk * 0.9f, physicalAttackPrefab);
                }
                break;
            case GameCode.PhysicalAttackType.None:
                return;
            default:
                break;
        }

    }

    public override void Animate (Vector3 movement, bool atking, bool[] skills)
    {
        base.Animate(movement, atking, skills);

        #region skill input
        // use one of the three active skills
        if (skills[0])
        {
            m_animator.SetBool("dash", true);
        }
        else if (skills[1])
        {
            m_animator.SetBool("ultra", true);
        }
        else if (skills[2])
        {
            // use raptor directly will entering charging state!!
            if (!m_anmSttInfo.IsName("0.atk3") && !m_anmSttInfo.IsName("0.raptor") && !m_anmSttInfo.IsName("0.charge"))
                m_animator.SetBool("raptord", true);
        }
        #endregion

        #region AnimatorStateInfo
        if (m_anmSttInfo.IsName("0.dash"))
        {
            m_animator.SetBool("moving", movement.magnitude > 0.1f);

            // dash trail
            if (trail != null)
            {
                if (!trail.enabled && m_anmSttInfo.normalizedTime < 0.5f)
                {
                    trail.enabled = true;
                }
                if (trail.enabled && m_anmSttInfo.normalizedTime > 0.5f)
                {
                    trail.enabled = false;
                }
            }

            if (m_animator.GetBool("dash"))
            {
                m_agent.angularSpeed = 0;
                m_agent.speed = 0;
                m_agent.Stop();
                m_animator.SetBool("dash", false);
                m_skillManager.Skill(0);
            }
            if (atking)
                m_animator.SetBool("atk", true);
            transform.Translate(Vector3.forward * Time.deltaTime * 30, Space.Self);
        }
        else if (m_anmSttInfo.IsName("0.crit"))
        {
            if (!m_hasAtked && m_anmSttInfo.normalizedTime < 0.2f)
            {
                m_hasAtked = true;
                if (targets.Count > 0)
                {
                    // target index, damage, attack effect, controlling type, controlling time, controlling degree
                    ControlEnemy(0, PlayerData.GetInstance().crit, critAttackPrefab, "knock up", 0.3f, 1.8f);
                }
                StartCoroutine(JumpAndDrop(0.3f, 1.8f));
            }

            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            if (m_agent.enabled && atking)
                m_animator.SetBool("atk", true);
        }
        else if (m_anmSttInfo.IsName("0.ultrasonic"))
        {
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            if (m_animator.GetBool("ultra"))
            {
                m_agent.angularSpeed = 9999999999;
                m_agent.speed = 0;
                m_agent.Stop();
                m_animator.SetBool("ultra", false);
                m_skillManager.Skill(1);
            }
            if (!m_hasAtked && targets.Count > 0 && m_anmSttInfo.normalizedTime > 0.2f && m_anmSttInfo.normalizedTime < 0.3f)
            {
                m_hasAtked = true;
                float damage = Ultrasonic.baseDamage + PlayerData.GetInstance().GetAttributeValue(GameCode.BasicAttributeName.Intelligence) * Ultrasonic.intAugRate;
                Damage(0, damage, skillAttackPrefab);
            }
            // this skill can be sequenced by pkysical attack
            if (atking)
                m_animator.SetBool("atk", true);
        }
        else if (m_anmSttInfo.IsName("0.atk3"))
        {
            if (skills[2])
            {
                m_animator.SetBool("raptor", true);
            }
        }
        else if (m_anmSttInfo.IsName("0.charge"))
        {
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            if (m_animator.GetBool("raptord"))
            {
                ChargeEffect();
                m_animator.SetBool("raptord", false);
                m_animator.SetBool("charged", true);
            }
        }
        else if (m_anmSttInfo.IsName("0.raptor"))
        {
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            // launch directly
            if (m_anmSttInfo.normalizedTime > 0.2f && m_animator.GetBool("charged"))
            {
                m_animator.SetBool("charged", false);
                m_skillManager.Skill(2);
                CameraShake(0.1f);
                if (targets.Count > 0)
                {
                    float damage = Raptor.baseDamage + PlayerData.GetInstance().GetAttributeValue(GameCode.BasicAttributeName.Intelligence) * Raptor.intAugRate;
                    ControlEnemy(0, damage, skillAttackPrefab, "knock back", 0.5f, 8);
                }
            }

            // launch before combo
            if (m_anmSttInfo.normalizedTime > 0.2f && m_animator.GetBool("raptor"))
            {
                m_animator.SetBool("raptor", false);
                m_skillManager.Skill(2);
                CameraShake(0.1f);
                if (targets.Count > 0)
                {
                    float damage = Raptor.baseDamage + PlayerData.GetInstance().GetAttributeValue(GameCode.BasicAttributeName.Intelligence) * Raptor.intAugRate;
                    ControlEnemy(0, damage, skillAttackPrefab, "knock back", 0.5f, 8);
                }
            }
        }
        else if (m_anmTraInfo.IsName("0.air atk2"))
        {
            StartCoroutine(JumpAndDrop(0.2f, 1));
        }
        #endregion

        #region AnimatorTransitionInfo
        if (m_anmTraInfo.IsUserName("2dash"))
        {
            // look at axis direction before monk dash
            transform.LookAt(transform.position + movement);
        }
        else if (m_anmTraInfo.IsUserName("2a"))
        {
            m_jumped = false;
        }
        else if (m_anmTraInfo.IsUserName("a2i"))
        {
            m_jumped = false;
        }
        #endregion
    }

    protected void AirAtkAnimate (bool atking, float time, int stage)
    {
        m_animator.speed = PlayerData.GetInstance().atkSpeed;

        // attacking detection
        if (time > ariAtktimes[stage].x && time < ariAtktimes[stage].y && !m_animator.GetBool("atk"))
        {

            // atk action will be called only once when atk animation is playing
            if (!m_hasAtked)
            {
                MeleeAtk(stage);
                StopCoroutine(jump);
                jump = StartCoroutine(JumpAndDrop(0.1f, 1));
            }

            // to the next stage of combo
            if (atking)
                m_animator.SetBool("atk", true);
        }

    }
}
