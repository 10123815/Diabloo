using UnityEngine;
using System.Collections;

public class Ultrasonic : Skill
{

    static public float baseDamage = 30;
    static public float intAugRate = 0.3f;

    public Ultrasonic ()
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        // Debug.Log("PLayer learn Ultrasonic!!!!");
    }

    public override void OnRemovingEffect ( )
    {
        // Debug.Log("Player remove Ultrasonic!!!!");
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
