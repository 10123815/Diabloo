using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class KnightAction : BossAction
{

    private float m_fightRate = 0.05f;

    // effect
    public GameObject rageEffectPrefab;
    public GameObject shieldEffectPrefab;
    public MeleeWeaponTrail mwTrail;

    public override void Start ( )
    {
        m_data = GetComponent<KnightData>();
        base.Start();

    }

    protected override void Init ( )
    {
        enemyId = fuckRPGLib.GameCode.EnemyID.Knight;
        base.Init();
    }

    override protected void Animate ( )
    {

        #region AnimatorStateInfo
        if (m_anmSttInfo.IsName("0.idle"))
        {
            m_hasAtked = false;
            m_animator.SetBool("l", Random.value < 0.5f);
            m_animator.SetBool("r", !m_animator.GetBool("l"));
            if (Time.frameCount % 10 == 0 && !m_animator.GetBool("fight"))
            {
                m_animator.SetBool("fight", Random.value < m_fightRate);
            }
        }
        if (m_anmSttInfo.IsName("0.l"))
        {
            if (m_animator.GetBool("l"))
            {
                m_animator.SetBool("l", false);
                m_animator.SetBool("r", Random.value < 0.5f);
            }
            LatoralMove(-1);
            if (Time.frameCount % 10 == 0 && !m_animator.GetBool("fight"))
                m_animator.SetBool("fight", Random.value < m_fightRate);
        }
        else if (m_anmSttInfo.IsName("0.r"))
        {
            if (m_animator.GetBool("r"))
            {
                m_animator.SetBool("r", false);
                m_animator.SetBool("l", Random.value < 0.5f);
            }
            LatoralMove(1);
            if (Time.frameCount % 10 == 0 && !m_animator.GetBool("fight"))
                m_animator.SetBool("fight", Random.value < m_fightRate);
        }
        else if (m_anmSttInfo.IsName("0.run"))
        {
            transform.Translate(Vector3.forward * m_data.moveSpeed * Time.deltaTime);
            if (m_animator.GetFloat("dis") > 8 && Time.frameCount % 10 == 0 && !m_animator.GetBool("dash"))
            {
                m_animator.SetBool("dash", Random.value < 0.1);
            }
        }
        else if (m_anmSttInfo.IsName("0.back"))
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            transform.Translate(-Vector3.forward * 20 * Time.deltaTime);
        }
        else if (m_anmSttInfo.IsName("0.atk0"))
        {
            if (m_anmSttInfo.normalizedTime > 0.5f && m_anmSttInfo.normalizedTime < 0.64f)
            {
                m_hasAtked = false;
            }
            MeleeAtkTrailEffect();
            if (m_anmSttInfo.normalizedTime > 0.65f && !m_hasAtked && m_anmSttInfo.normalizedTime < 0.75f)
            {
                KnightAttack();
            }
        }
        else if (m_anmSttInfo.IsName("0.atk1"))
        {
            MeleeAtkTrailEffect();
            if (m_anmSttInfo.normalizedTime > 0.65f && !m_hasAtked && m_anmSttInfo.normalizedTime < 0.75f)
            {
                KnightAttack();
            }
        }
        else if (m_anmSttInfo.IsName("0.atk2"))
        {
            if (m_anmSttInfo.normalizedTime > 0.45f)
            {
                mwTrail.Emit = true;
            }
        }
        else if (m_anmSttInfo.IsName("0.atk fin"))
        {
            if (!m_hasAtked && m_anmSttInfo.normalizedTime < 0.2f)
            {
                KnightAttack();
                ControlPlayer("knock back", 0.1f, 1);
            }
            if (m_anmSttInfo.normalizedTime > 0.45f)
            {
                mwTrail.Emit = false;
            }
        }
        else if (m_anmSttInfo.IsName("dash"))
        {
            if (m_anmSttInfo.normalizedTime > 0.2f && m_animator.GetBool("dash"))
            {
                m_animator.SetBool("dash", false);
                StartCoroutine(Dash(1, m_animator.GetFloat("dis") - 1f, transform.forward));
            }

            if (!m_hasAtked && m_animator.GetFloat("dis") < m_data.atkRange)
            {
                KnightAttack();
                ControlPlayer("stun", 0.05f);
            }
        }

        if (m_anmSttInfo.IsTag("r"))
        {
            transform.LookAt(player.transform);
        }
        #endregion

        #region AnimatorTransitionInfo
        if (m_anmTrsInfo.IsUserName("2a"))
        {
            m_hasAtked = false;
            transform.LookAt(player.transform);
            m_animator.SetBool("fight", false);
        }
        else if (m_anmTrsInfo.IsUserName("2i"))
        {
            m_hasAtked = false;
            transform.LookAt(player.transform);
        }
        #endregion

    }

    // if dir is equals to -1, latoral move left,  else right
    protected void LatoralMove (float dir)
    {
        float angle = Mathf.Acos(m_data.moveSpeed * 8 * Time.deltaTime / m_animator.GetFloat("dis"));
        // min is 30
        angle = Mathf.Max(angle * 180 / Mathf.PI, 30);
        Vector3 newDir = Quaternion.Euler(0, dir * angle, 0) * (player.transform.position - transform.position);
        Vector3 newPos = newDir.normalized * m_data.moveSpeed / 16 * Time.deltaTime + transform.position;
        transform.position = newPos;
    }

    protected void MeleeAtkTrailEffect (float start = 0.45f, float end = 0.75f)
    {
        if (m_anmSttInfo.normalizedTime > start && m_anmSttInfo.normalizedTime < end)
        {
            mwTrail.Emit = true;
        }
        else
        {
            mwTrail.Emit = false;
        }
    }

    private void KnightAttack ( )
    {
        m_hasAtked = true;
        if (m_animator.GetFloat("dis") < m_data.atkRange &&
            Vector3.Dot(transform.forward, (player.transform.position - transform.position).normalized) > 0.5)
            PlayerData.GetInstance().Damaged(m_data.atk);
    }

    protected void ControlPlayer (string type, float time, float d = 0)
    {
        if (m_animator.GetFloat("dis") < m_data.atkRange)
        {
            playerCtrl.controller = gameObject;
            StartCoroutine(playerCtrl.controlFuncDic[type](time, d));
        }
    }

    // dash dis meters along dir with time seconds
    protected IEnumerator Dash (float time, float dis, Vector3 dir)
    {
        shieldEffectPrefab.SetActive(true);
        dir.y = 0;
        Vector3 pos = transform.position + dis * dir;
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * dis / time);
            yield return 0;
        }
        shieldEffectPrefab.SetActive(false);
    }

    override protected void Rage ( )
    {
        if (!isRage && m_data.curLife < m_data.maxLife * 0.2f)
        {
            if (m_anmSttInfo.IsName("rage"))
            {
                if (m_anmSttInfo.normalizedTime > 0.9f)
                {
                    isRage = true;
                    m_data.def = m_data.maxDef / 4;
                    m_data.atk *= 1.5f;
                    m_fightRate *= 2;
                    // a rage effect
                    GameObject rageEffect = PoolManager.GetInstance().GetPool(rageEffectPrefab).GetObject(transform.position);
                    rageEffect.transform.SetParent(transform);
                }
            }
            else if (m_anmSttInfo.IsName("tried"))
            {
                m_animator.SetBool("tried", false);
            }
            else
            {
                m_animator.SetBool("tried", true);
                m_ctrled.isAntiCtrl = true;
                m_data.def = m_data.maxDef;
            }

        }
    }

}
