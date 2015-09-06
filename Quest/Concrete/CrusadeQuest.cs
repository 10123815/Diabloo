using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CrusadeQuest : Quest
{

    public byte dogID;
    public byte amount;

    public CrusadeQuest (string name, string descript, bool isMain, string scene, byte dogID, byte amount)
        : base(name, descript, isMain, scene)
    {
        this.dogID = dogID;
        this.amount = amount;
        this.questType = fuckRPGLib.GameCode.QuestType.Crusade;
    }


    public override void CheckCompletionEventHandler ( )
    {
        amount--;
        if (amount == 0)
        {
            Debug.Log("任务完成！");
            //GameObject comUI = PoolManager.GetInstance().GetPool(completionUIPrefab).GetObject(PlayerData.GetInstance().gameUI.transform);
            QuestManager.enemyDeathEvents[dogID].RemoveListener(this.CheckCompletionEventHandler);
        }
        else
            Debug.Log(dogID + " 剩余 " + amount);
    }

    public override void Claim ( )
    {
        // listen to the QuestManager to get the quest completion evet
        if (!QuestManager.enemyDeathEvents.ContainsKey(dogID))
        {
            UnityEvent checkEvent = new UnityEvent();
            QuestManager.enemyDeathEvents.Add(dogID, checkEvent);
        }
        QuestManager.enemyDeathEvents[dogID].AddListener(this.CheckCompletionEventHandler);
    }
}
