using UnityEngine;
using System.Collections;

public class ReturnToPool : MonoBehaviour 
{

    public bool autoDeath = false;

    // destroy gameobject after this time
    public float deathTime;

	// Use this for initialization
	void Awake () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (autoDeath)
        {
            StartCoroutine(Return(deathTime));
        }
	}

    private IEnumerator Return(float _t)
    {
        yield return new WaitForSeconds(_t);
        PoolManager.GetInstance().GetPool(gameObject.name).GivebackObject(gameObject);
    }
}
