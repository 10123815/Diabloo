//-------------------------------------------------------
// get the input from ETCInput to controll the character
// 
//-------------------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterAction))]
[RequireComponent(typeof(Controlled))]
public class CharacterInput : MonoBehaviour
{
    private CharacterAction m_action;
    private NavMeshAgent m_agent;
    private Animator m_animator;
    private Transform m_Cam;
    private Controlled m_ctrled;
    [System.NonSerialized]
    public Vector3 movement;
    [System.NonSerialized]
    public bool atk;
    [System.NonSerialized]
    public bool[] skills = new bool[3];

    // Use this for initialization
    void Start ( )
    {
        if (Camera.main == null)
        {
            Debug.LogError("Error: no main camera found");
        }
        else
        {
            m_Cam = Camera.main.transform;
        }
        m_action = GetComponent<CharacterAction>();
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_ctrled = GetComponent<Controlled>();
    }

    // Update is called once per frame
    void Update ( )
    {
        // death
        if (PlayerData.GetInstance().curLife <= 0)
        {
            m_animator.SetBool("death", true);
            m_agent.enabled = false;
            return;
        }

        // stun
        m_animator.SetBool("stun", !m_ctrled.canMove);
        // m_agent.enabled = m_ctrled.canMove;
        if (!m_ctrled.canMove)
        {
            m_action.Stunned();
            return;
        }
        else
        {
            m_agent.enabled = !m_action.isJump;
            m_animator.SetBool("stun", false);
        }

        atk = ETCInput.GetButtonDown("atk");
        skills[0] = ETCInput.GetButtonDown("0");
        skills[1] = ETCInput.GetButtonDown("1");
        skills[2] = ETCInput.GetButtonDown("2");
        float h = ETCInput.GetAxis("move h");
        float v = ETCInput.GetAxis("move v");
        Vector3 m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
        movement = v * m_CamForward + h * m_Cam.right;
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        if (movement.magnitude > 0.1f)
        {
            m_action.Move(movement);
        }
        m_action.Animate(movement, atk, skills);


    }
}
