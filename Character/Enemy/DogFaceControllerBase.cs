using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using fuckRPGLib;

public class DogFaceControllerBase : EnemyController
{

    public float detectRange = 8;

    [System.NonSerialized]
    public bool isFighting = false;
    protected Vector3 m_dodgePosition;

    public float anmMoveSpeed;
    public float anmAtkSpeed;
    public float anmCritSpeed;
    protected float m_baseSpeed;
    protected float m_baseAtkSpeed;

    // the next destinate position in random way point
    public float movementRange;
    protected Vector3 m_birthPosition;
    protected Vector3 m_nextDesPos;
    public float maxPauseTime = 4;
    protected float m_pauseTime = 4;

    // hp bar
    protected Vector3 hpUIPosition;
    protected Vector3 m_size;

    // Use this for initialization
    void Start ( )
    {
        enemyType = GameCode.EnemyType.Dogface;

        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_data = GetComponent<BaseData>();
        m_baseSpeed = m_data.moveSpeed;
        m_baseAtkSpeed = m_data.atkSpeed;
        m_agent.speed = m_baseSpeed;
        m_size = GetComponent<Collider>().bounds.size;
        m_ctrled = GetComponent<Controlled>();

        Init();
    }

    virtual protected void Init ( )
    {
        m_birthPosition = transform.position;
        m_data.curLife = m_data.maxLife;
        tag = "Enemy";
        m_hasInited = true;
    }

    // Update is called once per frame
    void Update ( )
    {
        if (!m_hasInited)
        {
            Init();
        }

        m_anmSttInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        m_anmTrsInfo = m_animator.GetAnimatorTransitionInfo(0);

        // death
        Dead();

        // stunned
        BeControlled();

        SetUI();

        m_distance = Vector3.Distance(player.transform.position, transform.position);
        m_animator.SetFloat("dis", m_distance);
        isFighting = m_distance < detectRange;
        m_agent.speed = m_data.moveSpeed;

        if (!isFighting)
        {
            RandomWayPoint();
        }
        else
        {
            m_pauseTime = maxPauseTime;
        }

        UpdateAnimation();
    }

    // the movement when idle is a random way point model
    private void RandomWayPoint ( )
    {
        if (m_pauseTime > 0)
        {
            // stop moving
            m_pauseTime -= Time.deltaTime;
        }
        else if (m_pauseTime < 0)
        {
            // start to move
            m_pauseTime = 0;
            Vector3 rdDir = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
            rdDir = rdDir.normalized;
            float rdDis = Mathf.Max(1, Random.value * movementRange);
            m_nextDesPos = rdDir * rdDis + m_birthPosition;
            m_agent.Resume();
        }
        else
        {
            // moving
            m_agent.SetDestination(m_nextDesPos);
            if (!m_agent.hasPath)
            {
                m_pauseTime = Mathf.Max(Random.value * maxPauseTime, 1);
            }
        }
    }

    virtual protected void UpdateAnimation ( )
    {

        #region normal state
        if (m_anmSttInfo.IsName("0.idle"))
        {
            m_animator.SetBool("run", m_agent.hasPath);
            m_animator.speed = 1;
        }
        else if (m_anmSttInfo.IsName("0.run"))
        {
            m_animator.SetBool("run", m_agent.hasPath);
            m_animator.speed = m_data.moveSpeed / m_baseSpeed;
            m_agent.speed = m_data.moveSpeed;
        }
        #endregion
    }


    // set life bar on its head
    virtual protected void SetUI ( )
    {
        if (hpBar && hpSlider)
        {
            hpSlider.value = m_data.curLife / m_data.maxLife;
            if (m_distance < 30)
            {
                if (!hpBar.activeSelf)
                    hpBar.SetActive(true);
                else
                {
                    hpUIPosition = new Vector3(transform.position.x, transform.position.y + m_size.y, transform.position.z);
                    hpBar.transform.position = Camera.main.WorldToScreenPoint(hpUIPosition);
                }
            }
            else
            {
                if (hpBar.activeSelf)
                    hpBar.SetActive(false);
            }
        }
    }
}
