using UnityEngine;
using System.Collections;

public interface IBuySell
{

    // sell a item to a businessman
    // return the selling price of the item
    float Sell (Item item, Merchant businessman);

    // buy a item from a businessman
    void Buy (Item item, Merchant businessman);

}
