using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireSprayEffect : MonoBehaviour
{

    public float fireAtk = 10;

    private Dictionary<int, BaseData> m_targetDatas = new Dictionary<int, BaseData>();

    // Use this for initialization
    void Start ( )
    {

    }

    // Update is called once per frame
    void Update ( )
    {

    }

    public void OnTriggerEnter (Collider collider)
    {
        if (collider.tag.Equals("Enemy") && tag.Equals("Player Atk"))
        {
            if (!m_targetDatas.ContainsKey(collider.GetInstanceID()))
                m_targetDatas.Add(collider.GetInstanceID(), collider.GetComponent<BaseData>());
        }
        else if (collider.tag.Equals("Player") && tag.Equals("Enemy Atk"))
        {
            if (!m_targetDatas.ContainsKey(collider.GetInstanceID()))
                m_targetDatas.Add(collider.GetInstanceID(), PlayerData.GetInstance());
        }

    }

    public void OnTriggerStay (Collider collider)
    {
        int id = collider.GetInstanceID();
        if (m_targetDatas.ContainsKey(id))
        {
            m_targetDatas[id].Damaged(fireAtk * Time.deltaTime);
        }
    }

    public void OnTriggerExit (Collider collider)
    {
        int id = collider.GetInstanceID();
        if (m_targetDatas.ContainsKey(id))
        {
            m_targetDatas.Remove(id);
        }
    }
}
