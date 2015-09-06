using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

// Listen some quest-relative event, such as enemy death, 
// item collected... when a quest-relative event come, 
// publish this event to relative quests the player has claim
public class QuestManager
{

    // max amount of quest the player can claim
    static public byte maxQuestCount = 5;

    static public Dictionary<byte, UnityEvent> enemyDeathEvents = new Dictionary<byte, UnityEvent>();

    // a singleton
    public static QuestManager GetInstance ( )
    {
        return Singleton<QuestManager>.GetInstance();
    }

    private QuestManager ( )
    {
    }

    static public void EnemyDeathHandler (byte id)
    {
        //Debug.Log("Enemy id : " + id);
        enemyDeathEvents[id].Invoke();
    }

    static public void ItemCollectionHandler (byte id)
    {
        //Debug.Log("Item id : " + id);
    }

}
