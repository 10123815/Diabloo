using UnityEngine;
using System.Collections;

public class EleShieldEffect : BaseAOE
{

    public void OnTriggerStay (Collider collider)
    {
        if (collider.tag.Equals("Enemy"))
        {
            AOE(collider);
        }
    }

}
