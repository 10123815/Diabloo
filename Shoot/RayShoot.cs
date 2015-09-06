using UnityEngine;
using System.Collections;

// shoot along a ray until collied with a enemy
public class RayShoot : MonoBehaviour
{

    public float deathTime = 5;
    private float m_lifeTime = 0;
    public float dmg;
    public float speed;

    [System.NonSerialized]
    public string shooterTag;

    public GameObject hitEffectPrefab;

    // Use this for initialization
    void Start ( )
    {

    }

    // Update is called once per frame
    void Update ( )
    {
        if (m_lifeTime >= deathTime)
        {
            m_lifeTime = 0;
            OnGiveBack();
            // destroyed when time out
            PoolManager.GetInstance().GetPool(name).GivebackObject(gameObject);
            return;
        }

        m_lifeTime += Time.deltaTime;
        if (m_lifeTime > 0.05f)
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    public void OnTriggerEnter (Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            PlayerData.GetInstance().Damaged(dmg);
        }
        if (collider.tag.Equals("Player") || collider.tag.Equals("Ground"))
        {
            m_lifeTime = 0;
            OnGiveBack();
            PoolManager.GetInstance().GetPool(hitEffectPrefab).GetObject(transform.position);
            // destroyed when collide with a gameobject
            PoolManager.GetInstance().GetPool(name).GivebackObject(gameObject);
        }
    }

    virtual protected void OnGiveBack ( )
    {
    }
}
