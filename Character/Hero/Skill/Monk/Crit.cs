using UnityEngine;
using System.Collections;

public class Crit : Ultrasonic
{

    static public float augRate = 1.3f;

    public Crit ( )
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        base.OnAddingEffect();
        PlayerData.GetInstance().critRate *= augRate;
    }
}
