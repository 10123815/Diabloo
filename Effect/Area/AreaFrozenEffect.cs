using UnityEngine;
using System.Collections;

public class AreaFrozenEffect : BaseAOE
{

    public float duration;
    public float d;
    public GameObject frozenPrefab;
    
    // Use this for initialization
    void Start ( )
    {
    }

    // Update is called once per frame
    void Update ( )
    {
        StartCoroutine(DoReturn(duration + 0.1f));
    }

    public void OnTriggerEnter (Collider collider)
    {
        print(collider.name);
        if (collider.tag.Equals("Enemy"))
        {
            GameObject frozen = PoolManager.GetInstance().GetPool(frozenPrefab).GetObject();
            frozen.transform.SetParent(collider.transform);
            frozen.transform.localPosition = Vector3.zero;
            Vector3 targetSize = collider.GetComponent<Collider>().bounds.size;
            frozen.transform.localScale = targetSize;
            ParticleSystem[] pses = frozen.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < pses.Length; i++)
            {
                pses[i].startSize = targetSize.y;
            }

            LastingEffect frost = frozen.GetComponent<LastingEffect>();
            frost.Lasting(collider.transform, duration, d);
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (collider.tag.Equals("Enemy"))
        {
            AOE(collider);
        }
    }

    private IEnumerator DoReturn(float time)
    {
        yield return new WaitForSeconds(time);
        PoolManager.GetInstance().GetPool(name).GivebackObject(gameObject);
    }

}
