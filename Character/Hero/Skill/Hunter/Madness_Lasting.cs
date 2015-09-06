using UnityEngine;
using System.Collections;

public class Madness_Lasting : Madness
{

    static private float m_damagedRate = 5 * Time.deltaTime ;

    public Madness_Lasting ( )
    {
        m_atkSpeedAugRate = 1;
        m_speedAugRate = 1;
        m_sa = PlayerData.GetInstance().moveSpeed * m_speedAugRate;
        m_aa = PlayerData.GetInstance().atkSpeed * m_atkSpeedAugRate;
    }

    static public void OnLasting (float t)
    {
        PlayerData.GetInstance().Damaged(m_damagedRate);
    }

}
