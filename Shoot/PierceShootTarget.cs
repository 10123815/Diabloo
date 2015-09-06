using UnityEngine;
using System.Collections;

// this kind of arrow will pierce its target and continue moving forward,
// unless the duration has run out
public class PierceShootTarget : ShootTarget
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
        }
    }
}
