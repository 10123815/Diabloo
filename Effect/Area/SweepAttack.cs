using UnityEngine;
using System.Collections;

public class SweepAttack : MonoBehaviour
{
    public float dmg;
    private GameObject m_player;
    private Controlled m_playCtrled;
    
    // Use this for initialization
    void Start ( )
    {
        dmg = GetComponentInParent<NashorData>().atk;
        m_player = PlayerData.GetInstance().action.gameObject;
        m_playCtrled = PlayerData.GetInstance().action.GetComponent<Controlled>();
        if (m_playCtrled == null)
        {
            Debug.LogError("Controlled has not be found at player!!");
        }
    }

    public void OnTriggerEnter (Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            m_playCtrled.controller = gameObject;
            PlayerData.GetInstance().Damaged(dmg);
            StartCoroutine(m_playCtrled.controlFuncDic["knock back"](0.5f, 6));
        }
    }

}
