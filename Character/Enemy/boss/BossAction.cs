using UnityEngine;
using System.Collections;

public class BossAction : EnemyController
{
    protected bool m_hasAtked;

    // when curlife < 0.3 max, rage
    protected bool isRage = false;

    protected float m_baseMoveSpeed;

    // Use this for initialization
    virtual public void Start ( )
    {

        playerCtrl = player.GetComponent<Controlled>();

        m_animator = GetComponent<Animator>();
        m_ctrled = GetComponent<Controlled>();

        m_baseMoveSpeed = m_data.moveSpeed;

        Init();

    }

    virtual protected void Init ( )
    {

    }

    // Update is called once per frame
    void Update ( )
    {

        m_anmSttInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        m_anmTrsInfo = m_animator.GetAnimatorTransitionInfo(0);

        // death
        Dead();
        // set hp UI
        hpSlider.value = Mathf.Lerp(hpSlider.value, m_data.curLife / m_data.maxLife, Time.deltaTime * 10);
        if (tag == "dead")
        {
            return;
        }

        // stunned
        BeControlled();

        // heavy hurt and the rage
        Rage();
        if (m_data.curLife < m_data.maxLife * 0.2f && !isRage)
            return;

        if (m_ctrled.canMove && m_data.curLife > 0)
        {
            m_animator.SetFloat("dis", Vector3.Distance(transform.position, player.transform.position));
            Animate();
        }
    }

    virtual protected void Rage ( )
    {

    }

    virtual protected void Animate ( )
    {
    }

}
