using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class HuntingQuest : Quest
{

    public byte bossID;

    public HuntingQuest (string name, string descript, bool isMain, string scene, byte bossID)
        : base(name, descript, isMain, scene)
    {
        this.bossID = bossID;
        questType = fuckRPGLib.GameCode.QuestType.Hunting;
    }

    public override void CheckCompletionEventHandler ( )
    {
        Debug.Log(bossID + "  die");

    }

    public override void Claim ( )
    {
        // listen to the QuestManager to get the quest completion evet
        if (!QuestManager.enemyDeathEvents.ContainsKey(bossID))
        {
            UnityEvent checkEvent = new UnityEvent();
            QuestManager.enemyDeathEvents.Add(bossID, checkEvent);
        }
        QuestManager.enemyDeathEvents[bossID].AddListener(this.CheckCompletionEventHandler);
    }
}
