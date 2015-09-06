using UnityEngine;
using System.Collections;

public class DragonHeart : FighterSkill
{

    public DragonHeart ( )
    {
        OnAddingEffect();
    }

    protected override void OnAddingEffect ( )
    {
        float r = PlayerData.GetInstance().curLife / PlayerData.GetInstance().maxLife;
        PlayerData.GetInstance().maxLife += 100;
        PlayerData.GetInstance().curLife = PlayerData.GetInstance().maxLife * r;
        PlayerData.GetInstance().gameUI.lifeText.text = PlayerData.GetInstance().curLife + " / " + PlayerData.GetInstance().maxLife;
        PlayerData.GetInstance().healingRate *= 2;
        PlayerData.GetInstance().gameUI.lifeRegeText.text = "+" + PlayerData.GetInstance().healingRate;
    }

}
