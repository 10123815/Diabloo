using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SkillManager))]
public class CharacterAction : MonoBehaviour
{

    public static CharacterAction character;

    protected float m_baseMoveSpeed;

    [System.NonSerialized]
    public bool isJump = false;

    public Dictionary<int, GameObject> enemyChecked = new Dictionary<int, GameObject>();

    #region effect pregab
    public GameObject physicalAttackPrefab;
    public GameObject critAttackPrefab;
    public GameObject skillAttackPrefab;
    public GameObject redCheckingPrefab;
    public GameObject greenCheckPrefab;
    public GameObject lvupEffectPrefab;
    public GameObject chargePrefab;
    public GameObject madnessPrefab;
    public GameObject areaControlPrefab;
    public GameObject groundEffectPrefab;
    #endregion

    // has the attack decision triggered
    protected bool m_hasAtked = false;
    // time range of attack decision
    public Vector2[] atkTimes = new Vector2[3];
    public bool isMelee = true;
    protected MeleeWeaponTrail m_meleeWeaponTrail;

    protected Animator m_animator;
    protected AnimatorStateInfo m_anmSttInfo;
    protected AnimatorTransitionInfo m_anmTraInfo;
    protected NavMeshAgent m_agent;
    protected SkillManager m_skillManager;
    protected Controlled m_ctrled;

    // all enemies in atk or skill area
    //[System.NonSerialized]
    public List<GameObject> targets = new List<GameObject>();
    
    // Use this for initialization
    void Start ( )
    {
        m_skillManager = GetComponent<SkillManager>();
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_ctrled = GetComponent<Controlled>();
        m_baseMoveSpeed = m_agent.speed;

        if (isMelee)
        {
            m_meleeWeaponTrail = GetComponentInChildren<MeleeWeaponTrail>();
        }

        PlayerData.GetInstance().action = this;
    }

    public void Move (Vector3 movement)
    {
        if (m_ctrled.canMove && m_agent.enabled)
            m_agent.SetDestination(transform.position + movement);
    }

    public virtual void Animate (Vector3 movement, bool atking, bool[] skills)
    {

        #region AnimatorStateInfo
        m_anmSttInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        if (m_anmSttInfo.IsName("0.idle"))
        {
            m_agent.Stop();
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            m_animator.SetBool("atk", atking);
            m_animator.speed = 1;
        }
        else if (m_anmSttInfo.IsName("0.run"))
        {
            m_agent.Resume();
            m_animator.SetBool("moving", movement.magnitude > 0.1f);
            m_animator.SetBool("atk", atking);
            m_animator.speed = PlayerData.GetInstance().moveSpeed / m_baseMoveSpeed;
            m_agent.speed = PlayerData.GetInstance().moveSpeed;
            m_agent.angularSpeed = 99999999;
        }
        else if (m_anmSttInfo.IsName("0.atk0"))
        {
            AtkAnimate(atking, m_anmSttInfo.normalizedTime, 0);
        }
        else if (m_anmSttInfo.IsName("0.atk1"))
        {
            AtkAnimate(atking, m_anmSttInfo.normalizedTime, 0);
        }
        else if (m_anmSttInfo.IsName("0.atk2"))
        {
            AtkAnimate(atking, m_anmSttInfo.normalizedTime, 1);
        }
        else if (m_anmSttInfo.IsName("0.atk3"))
        {
            AtkAnimate(atking, m_anmSttInfo.normalizedTime, 2);
            if (m_anmSttInfo.normalizedTime > 0.5f)
                m_animator.SetBool("moving", movement.magnitude > 0.1f);
        }
        #endregion

        #region AnimatorTransitionInfo
        m_anmTraInfo = m_animator.GetAnimatorTransitionInfo(0);

        if (m_anmTraInfo.IsUserName("2a"))
        {
            m_animator.SetBool("atk", false);
            m_hasAtked = false;
            m_agent.angularSpeed = 0;
            m_agent.speed = 0;
            Vector3 direction = Vector3.Lerp(transform.position + transform.forward, transform.position + movement, Time.deltaTime * 10);
            transform.LookAt(direction);
            m_agent.Stop();
        }
        else if (m_anmTraInfo.IsUserName("a2i") && m_anmTraInfo.IsUserName("a2r"))
        {
            m_animator.SetBool("atk", false);
            m_agent.angularSpeed = 9999999999;
            m_agent.Resume();
        }
        #endregion

    }


    protected void AtkAnimate (bool atking, float time, int stage)
    {
        m_agent.Stop();
        m_agent.speed = 0;
        m_agent.angularSpeed = 0;

        m_animator.speed = PlayerData.GetInstance().atkSpeed;

        if (m_meleeWeaponTrail != null)
        {
            m_meleeWeaponTrail.Emit = true;
        }
        // turn off melee atk effect
        if (time > atkTimes[stage].y / 2 && m_meleeWeaponTrail != null)
        {
            m_meleeWeaponTrail.Emit = false;
        }

        // attacking detection
        if (time > atkTimes[stage].x && time < atkTimes[stage].y && !m_animator.GetBool("atk"))
        {

            // atk action will be called only once when atk animation is playing
            if (!m_hasAtked)
            {
                if (isMelee)
                {
                    MeleeAtk(stage);
                }
                else
                {
                    RangeAtk(stage);
                }
            }
            // to the next stage of combo
            if (atking)
                m_animator.SetBool("atk", true);
        }

    }


    protected virtual void MeleeAtk (int stage)
    {
    }

    protected virtual void RangeAtk (int stage)
    {
    }

    // display a effect of "hit" to enemy under a physical attack
    // four different classes may have their own effect
    // this effect will die by itself
    protected void DoHitEffect (int indexOfEnemy, GameObject prefab)
    {
        if (prefab == null)
        {
            return;
        }

        Transform tf = targets[indexOfEnemy].transform;
        Vector3 enemySize = targets[indexOfEnemy].GetComponent<Collider>().bounds.size;
        Vector3 position = new Vector3(tf.position.x, tf.position.y + enemySize.z / 2, tf.position.z);
        GameObject phyhit = (GameObject)PoolManager.GetInstance().GetPool(prefab).GetObject(position);
        ParticleSystem ps = phyhit.GetComponentInChildren<ParticleSystem>();
        if (ps)
            ps.startSize = enemySize.y;
    }

    // display a effect of "checking" to enemy attacked and do not have this effect
    public void CheckEffect (int indexOfEnemy)
    {
        if (indexOfEnemy < targets.Count && !enemyChecked.ContainsKey(indexOfEnemy))
        {
            // this effect should be put at the enemy's foot root
            GameObject redCheck = (GameObject)PoolManager.GetInstance().GetPool(redCheckingPrefab).GetObject(targets[indexOfEnemy].transform.position);
            redCheck.transform.parent = targets[indexOfEnemy].transform;
            redCheck.GetComponentInChildren<ParticleSystem>().startSize = targets[indexOfEnemy].GetComponent<Collider>().bounds.size.y * 2;
            enemyChecked.Add(indexOfEnemy, redCheck);
        }
    }

    // disable "cheking" effect if this enemy has it
    public void UndoCheckEffect (int indexOfEnemy)
    {
        if (enemyChecked.ContainsKey(indexOfEnemy))
        {
            enemyChecked[indexOfEnemy].transform.parent = null;
            // return it back to the object pool
            PoolManager.GetInstance().GetPool("RedCheck").GivebackObject(enemyChecked[indexOfEnemy]);
            enemyChecked.Remove(indexOfEnemy);
            //MeshRenderer[] renderers = targets[indexOfEnemy].transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
            //for (int i = 0; i < renderers.Length; i++)
            //{
            //    renderers[i].material.shader = Shader.Find("Mobile/Diffuse");
            //}
        }
    }

    protected void ChargeEffect ( )
    {
        GameObject charge = PoolManager.GetInstance().GetPool(chargePrefab).GetObject();
        charge.transform.SetParent(transform);
        charge.transform.localPosition = new Vector3(0, 1, 0);
    }

    protected void ChargeEffect(Vector3 position)
    {
        GameObject charge = PoolManager.GetInstance().GetPool(chargePrefab).GetObject(position);
        charge.transform.SetParent(transform);
    }

    // shake the camera at some time such as a crit attack or a very DIAO skill
    protected IEnumerator CameraShake (float duration)
    {
        //Vector3 origin = Camera.main.transform.position;
        Vector3 position = Camera.main.transform.position + new Vector3(1, 0.5f, 0.5f);
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            yield return 0;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, position, Time.deltaTime * 50);
        }
    }

    protected void LookAtTarget (Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
        transform.LookAt(dir + transform.position);
    }

    protected void ControlEnemy (int indexOfEnemy, float atk, GameObject hitEffect, string type, float time, float height = 2.0f)
    {
        if (indexOfEnemy > targets.Count)
            return;

        LookAtTarget(targets[indexOfEnemy].transform);
        BaseData data = targets[indexOfEnemy].GetComponent<BaseData>();
        Controlled ctrled = targets[indexOfEnemy].GetComponent<Controlled>();
        if (data != null)
        {
            // add a "get hit" effect to this enemy
            DoHitEffect(indexOfEnemy, hitEffect);
            m_hasAtked = true;
            // may be the character only control enemy but not attack it
            if (atk > 0)
            {
                data.Damaged(atk, PlayerData.GetInstance().armPieRate);
                if (ctrled != null)
                {
                    if (ctrled.controller == null)
                        ctrled.controller = gameObject;
                    StartCoroutine(ctrled.controlFuncDic[type](time, height));
                }
                if (data.curLife <= 0)
                {
                    DoDefeatEnemyByPhyatk(indexOfEnemy, data.valueExp, data.valueGold);
                }
            }
        }
    }

    protected void Damage (int indexOfEnemy, float atk, GameObject hitEffect)
    {
        if (indexOfEnemy > targets.Count)
            return;

        LookAtTarget(targets[indexOfEnemy].transform);
        BaseData data = targets[indexOfEnemy].GetComponent<BaseData>();
        if (data != null)
        {
            // add a "get hit" effect to this enemy
            DoHitEffect(indexOfEnemy, hitEffect);
            data.Damaged(atk, PlayerData.GetInstance().armPieRate);
            m_hasAtked = true;
            if (data.curLife <= 0)
            {
                DoDefeatEnemyByPhyatk(indexOfEnemy, data.valueExp, data.valueGold);
            }
        }
    }

    // defeat by physical attack, can not be called by another script
    protected void DoDefeatEnemyByPhyatk (int indexOfEnemy, int exp, int gold)
    {
        targets.RemoveAt(indexOfEnemy);
        UndoCheckEffect(indexOfEnemy);
        // get exp and gold
        PlayerData.GetInstance().GainExp(exp);
        PlayerData.GetInstance().gold += gold;
    }

    protected void DoDefeatEnemyByPhyatk(GameObject enemy, int exp, int gold)
    {
        int indexOfEnemy = targets.IndexOf(enemy);
        DoDefeatEnemyByPhyatk(indexOfEnemy, exp, gold);
    }

    public void Stunned ( )
    {
        if (!m_anmSttInfo.IsName("0.stun"))
        {
            m_animator.SetBool("stun", true);
            m_agent.enabled = false;
        }
        else
        {
            // this time this go is at one of several stun state
            // we do not care what the state is, but we know this go can not move by it self
        }
    }

    public void LevelUpEffect ( )
    {
        GameObject lvupEffect = PoolManager.GetInstance().GetPool(lvupEffectPrefab).GetObject(transform.position);
        lvupEffect.transform.parent = transform;
        lvupEffect.transform.localPosition = Vector3.zero;
    }

    // character jump only when he is at some atking state but can not jump actively
    protected IEnumerator JumpAndDrop (float time, float height)
    {
        // player can not move when jumping
        isJump = true;
        Vector3 desP = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            // go up at the first second of "time"
            if (i < time / 2)
                transform.position = Vector3.Lerp(transform.position, desP, height / (time / 2) * Time.deltaTime);
            else
                transform.position -= new Vector3(0, height / (time / 2) * Time.deltaTime, 0);
            yield return 0;
        }
        isJump = false;
    }


}
