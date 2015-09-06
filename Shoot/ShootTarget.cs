using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootTarget : MonoBehaviour
{

    public GameObject hitEffectPrefab;

    public float flySpeed = 10;

    [System.NonSerialized]
    public float atk;
    [System.NonSerialized]
    public Transform target;
    [System.NonSerialized]
    public float sizeY = 0;

    public float duration = 5;
    protected float m_time = 0;

    public bool isPierce = false;

    public delegate void ResetDL ( );
    public Dictionary<string, ResetDL> resetFuncDic = new Dictionary<string, ResetDL>();

    // Use this for initialization
    void Start ( )
    {

    }

    // Update is called once per frame
    public void Update ( )
    {
        if (target == null)
            return;

        m_time += Time.deltaTime;
        if (m_time > duration)
        {
            m_time = 0;
            PoolManager.GetInstance().GetPool(gameObject.name).GivebackObject(gameObject);
        }

        if (!isPierce)
        {
            transform.LookAt(target.position + new Vector3(0, sizeY, 0));
            transform.Translate(Vector3.forward * Time.deltaTime * flySpeed, Space.Self);
        }
        else
        {
            if (m_time < 0.1f)
                transform.LookAt(target.position + new Vector3(0, (sizeY + 1) / 2, 0));
            transform.Translate(Vector3.forward * Time.deltaTime * flySpeed, Space.Self);
        }

    }

    virtual public void OnTriggerEnter (Collider collider)
    {
        if (collider.transform.GetInstanceID() == target.GetInstanceID())
        {
            // effect when hit a enemy, that will be destroyed by it self
            GameObject hitEffect = PoolManager.GetInstance().GetPool(hitEffectPrefab).GetObject(transform.position);
            hitEffect.transform.SetParent(target);
            hitEffect.transform.localScale *= sizeY * 3;
            ParticleSystem[] particleSystems = hitEffect.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].startSize = sizeY * 2;
            }
            PoolManager.GetInstance().GetPool(gameObject.name).GivebackObject(gameObject);
            m_time = 0;
            Damage(target, PlayerData.GetInstance().hitRate);
        }
    }

    protected void Damage (Transform target, float hitRate)
    {
        BaseData data = target.GetComponent<BaseData>();
        if (data != null)
        {
            if (Random.value < hitRate)
            {
                data.Damaged(atk);
                // the shooting may launch some adnormal stat on enemy
                if (data.curLife <= 0)
                {
                    PlayerData.GetInstance().GainExp(data.valueExp);
                    PlayerData.GetInstance().gold += data.valueGold;
                }
            }
        }
    }

    public void Reset ( )
    {
        foreach (string key in resetFuncDic.Keys)
        {
            resetFuncDic[key]();
        }
    }

}
