using UnityEngine;
using System.Collections;

public class Defense_Lasting : Defense
{

    static protected GameObject m_innerVitalityEffect;
    static protected float m_aug;
    static protected float m_augRate = 0.2f;

    public Defense_Lasting ( )
    {
    }

    static public void OnLaunch ( )
    {
        Defense.OnLaunch();

        // healing effect
        m_innerVitalityEffect = PoolManager.GetInstance().GetPool(m_action.innerVitalityPrefab).GetObject();
        m_innerVitalityEffect.transform.SetParent(m_playerTF);
        m_innerVitalityEffect.transform.localPosition = Vector3.zero;
    
        m_aug = (PlayerData.GetInstance().maxLife - PlayerData.GetInstance().curLife) * m_augRate;
        PlayerData.GetInstance().healingRate += m_aug;
        PlayerData.GetInstance().gameUI.lifeRegeText.text = "+" + PlayerData.GetInstance().healingRate;
    }

    static public void OnLasting(float t)
    {
        PlayerData.GetInstance().healingRate -= m_aug;
        m_aug = (PlayerData.GetInstance().maxLife - PlayerData.GetInstance().curLife) * m_augRate;
        PlayerData.GetInstance().healingRate += m_aug;
        PlayerData.GetInstance().gameUI.lifeRegeText.text = "+" + PlayerData.GetInstance().healingRate;
    }

    static public void OnStop()
    {
        Defense.OnStop();
        PoolManager.GetInstance().GetPool(m_innerVitalityEffect.name).GivebackObject(m_innerVitalityEffect);
        PlayerData.GetInstance().healingRate -= m_aug;
        PlayerData.GetInstance().gameUI.lifeRegeText.text = "+" + PlayerData.GetInstance().healingRate;
    }
}
