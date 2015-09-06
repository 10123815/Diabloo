using UnityEngine;
using System.Collections;

public class Raptor : Skill
{

    static public float baseDamage = 50;
    static public float intAugRate = 0.3f;

    public Raptor ( )
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        //Debug.Log("PLayer learn Raptor!!!!");
    }

    public override void OnRemovingEffect ( )
    {
        //Debug.Log("Player remove Raptor!!!!");
    }


    // a active skill must both have a "Launch", a "Lasting" and a "Stop"
    // an instantaneous effect when ues this skill
    // only called one time
    static public void OnLaunch ( )
    {
        // Debug.Log("Launch Dash!!!");
    }

    // a lasting effect, called every frame at duration
    // a normalized time t
    static public void OnLasting (float t)
    {
        // Debug.Log("Dashing!!!");
    }

    // a skill has already stopped
    static public void OnStop ( )
    {
        // Debug.Log("Dash stop!");
    }
}
