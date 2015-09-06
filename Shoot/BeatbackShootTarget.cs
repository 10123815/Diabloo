using UnityEngine;
using System.Collections;

// pierce and bear back all enemy along its movement
public class BeatbackShootTarget : ShootTarget
{

    // Use this for initialization
    void Start ( )
    {

    }

    public override void OnTriggerEnter (Collider collider)
    {
        // damage all enemy it 
        if (collider.tag.Equals("Enemy"))
        {
            // a magic has a 100% hit rate
            Damage(collider.gameObject.transform, 1);
            // knock back
            Controlled contrlled = collider.gameObject.GetComponent<Controlled>();
            if (contrlled != null)
            {
                if (contrlled.controller == null)
                    contrlled.controller = PlayerData.GetInstance().action.gameObject;
                StartCoroutine(contrlled.KnockedBack(0.3f, 2));
            }
        }
    }

}
