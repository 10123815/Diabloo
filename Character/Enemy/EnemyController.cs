using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using fuckRPGLib;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BaseData))]
[RequireComponent(typeof(Controlled))]
public class EnemyController : MonoBehaviour
{

    public GameCode.EnemyID enemyId;

    // once we put thie enemy in game scene, we init it
    protected bool m_hasInited = false;

    public GameCode.EnemyType enemyType;

    [System.NonSerialized]
    public GameObject player;
    [System.NonSerialized]
    public Controlled playerCtrl;
    protected float m_playerSize = 1;
    protected float m_distance;

    protected Animator m_animator;
    protected AnimatorStateInfo m_anmSttInfo;
    protected AnimatorTransitionInfo m_anmTrsInfo;
    protected NavMeshAgent m_agent;
    protected BaseData m_data;
    protected Controlled m_ctrled;

    // hp bar
    [System.NonSerialized]
    public Slider hpSlider;
    [System.NonSerialized]
    public GameObject hpBar;

    public class EnemyDeathEvent : UnityEvent<byte>
    {
    }
    public EnemyDeathEvent enemyDeathEvent = new EnemyDeathEvent();

    protected void BeControlled ( )
    {
        // stun
        if (!m_ctrled.canMove)
        {
            if (!m_anmSttInfo.IsName("0.stun"))
            {
                m_animator.SetBool("stun", true);
                if (m_agent != null)
                    m_agent.enabled = false;
            }
            return;
        }
        else
        {
            if (m_agent != null)
                m_agent.enabled = true;
            m_animator.SetBool("stun", false);
        }
    }

    protected void Dead ( )
    {
        m_animator.SetBool("death", m_data.curLife <= 0);
        if (m_anmSttInfo.IsName("0.death"))
        {
            if (tag == "Enemy")
            {
                tag = "dead";
            }
            if (m_anmSttInfo.normalizedTime > 0.9f)
            {
                transform.position = Vector3.zero;
                m_hasInited = false;
                if (hpBar)
                    PoolManager.GetInstance().GetPool(hpBar.name).GivebackObject(hpBar);
                for (int i = 1; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        PoolManager.GetInstance().GetPool(transform.GetChild(i).name).GivebackObject(transform.GetChild(i));
                    }
                    else
                        transform.GetChild(i).parent = null;
                }
                PoolManager.GetInstance().GetPool(gameObject.name).GivebackObject(gameObject);

                // some quests will listen this enemy death event
                // now we sent a event to these listener
                enemyDeathEvent.Invoke((byte)enemyId);

            }
        }
        if (tag == "dead")
        {
            return;
        }
    }
}
