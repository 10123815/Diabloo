using UnityEngine;
using System.Collections;

public class DownShoot : RayShoot
{
    [System.NonSerialized]
    public GameObject check;

    protected override void OnGiveBack ( )
    {
        PoolManager.GetInstance().GetPool(check.name).GivebackObject(check);
    }

}
