using UnityEngine;
using System.Collections;

// a lasting effect will launch at target when shooted at it
public class LastingShootTarget : ShootTarget
{
    // lasting effect duration time
    public float time;

    // slowing down degree/firing damage....
    public float d;

    // Use this for initialization
    void Start ( )
    {

    }

    public override void OnTriggerEnter (Collider collider)
    {
        if (collider.transform.GetInstanceID() == target.GetInstanceID())
        {
            // a lasting effect
            GameObject lastingHit = PoolManager.GetInstance().GetPool(hitEffectPrefab).GetObject();
            lastingHit.transform.SetParent(target);
            lastingHit.transform.localPosition = Vector3.zero;
            lastingHit.transform.localScale = Vector3.one * 2 * sizeY;
            ParticleSystem[] ps = lastingHit.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].startSize = sizeY * 2;
            }

            m_time = 0;
            Damage(target, PlayerData.GetInstance().hitRate);

            // slow down/firing... enemy
            lastingHit.GetComponent<LastingEffect>().Lasting(target, time, d);

            // effect when hit a enemy, that will be destroyed by it self
            PoolManager.GetInstance().GetPool(gameObject.name).GivebackObject(gameObject);

        }
    }
}
