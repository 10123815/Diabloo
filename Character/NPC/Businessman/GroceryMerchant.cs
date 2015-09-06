using UnityEngine;
using System.Collections;

public class GroceryMerchant : Merchant
{
    void Start()
    {
        npcType = fuckRPGLib.GameCode.NPCType.Grocery;
    }
}
