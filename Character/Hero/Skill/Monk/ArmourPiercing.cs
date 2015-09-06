using UnityEngine;
using System.Collections;

public class ArmourPiercing : Raptor
{

    public ArmourPiercing ( )
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        base.OnAddingEffect();
        PlayerData.GetInstance().armPieRate += 30;
    }

    public override void OnRemovingEffect ( )
    {
        base.OnRemovingEffect();
    }

}
