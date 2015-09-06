using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using fuckRPGLib;

abstract public class Quest
{

    public string name;
    public string description;
    public string scene;
    public bool isMain;
    public bool hasCompleted = false;
    public GameCode.QuestType questType;

    public GameObject completionUIPrefab;

    public Reward reward;

    public Quest (string name, string descript, bool isMain, string scene)
    {
        this.name = name;
        this.description = descript;
        this.isMain = isMain;
        this.scene = scene;
    }

    // this method will be invoked when check the quest is completed or not
    abstract public void CheckCompletionEventHandler ( );

    // this method will be invoked when quest is claimed
    abstract public void Claim ( );

}
