using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// unlike a normal NPC, a quest man have a list of quest
// if a player want claim a quest of a quest man, he need to
// select one from this list
public class QuestMan : NPC
{

    const bool DEBUG = true;

    private List<Quest> m_allQuests = new List<Quest>();

    // avalible quest for player to select, it should be read only after create
    private List<Quest> m_avalibleQusets = new List<Quest>();

    // count of avalible quest;
    private int m_avaCount;

    // quest player attempt to claim
    private int m_attemptQuestClaim = -1;
    private fuckRPGLib.GameCode.QuestCaption m_questClaimCaption;

    // the grid(list, n * 1) UI of quest list items, its size will be changed depend on the count of avalible quest
    public RectTransform questGridUI;

    // we will create some quest list items dynamicly depend on the count of avalible quest
    // every of it will have the size h * w
    public GameObject questListItemUIPrefab;
    public float h = 100;
    public float w = 100;

    // the quest UIs we create dynamicly
    private List<GameObject> m_questUIs;

    // when select a quest, show the details
    public GameObject questDetailsUI;

    // quest name and decription
    public Text questNameText;
    public Text questDescriptionText;

    // main or side quest image
    public Sprite mainQuestButtonSprite;
    public Sprite sideQuestButtonSprite;

    void Start ( )
    {
        base.Start();
        npcType = fuckRPGLib.GameCode.NPCType.Quest;
        m_questUIs = new List<GameObject>();
        #region TEST_QUEST
        if (DEBUG)
        {
            m_avalibleQusets.Add(new CrusadeQuest("@w@", "杀死2个狼人首领", false, "snow", (byte)fuckRPGLib.GameCode.EnemyID.WolfLeader, 2));
            m_avalibleQusets.Add(new CrusadeQuest("@w@", "杀死2个狼人首领", false, "snow", (byte)fuckRPGLib.GameCode.EnemyID.WolfLeader, 2));
            m_avalibleQusets.Add(new CrusadeQuest("@w@", "杀死2个狼人首领", false, "snow", (byte)fuckRPGLib.GameCode.EnemyID.WolfLeader, 2));
            m_avalibleQusets.Add(new CrusadeQuest("@w@", "杀死2个狼人首领", false, "snow", (byte)fuckRPGLib.GameCode.EnemyID.WolfLeader, 2));
            m_avalibleQusets.Add(new CrusadeQuest("@w@", "杀死2个狼人首领", false, "snow", (byte)fuckRPGLib.GameCode.EnemyID.WolfLeader, 2));
            m_avalibleQusets.Add(new CrusadeQuest("@w@", "杀死2个狼人首领", false, "snow", (byte)fuckRPGLib.GameCode.EnemyID.WolfLeader, 2));
            m_avalibleQusets.Add(new HuntingQuest("我真是哔了狗了", "杀死地下城守护者（这个任务没有奖励）", false, "dungeon boss", (byte)fuckRPGLib.GameCode.EnemyID.Knight));
        }
        #endregion
        m_avaCount = m_avalibleQusets.Count;
        UpdateAvalibleQuestList();
    }

    // when player click on the quest man
    public override void OnClick ( )
    {
        base.OnClick();
        //UpdateAvalibleQuestList();
    }

    private Sprite GetQuestButtonSprite (bool isMain)
    {
        if (isMain)
        {
            return mainQuestButtonSprite;
        }
        else
        {
            return sideQuestButtonSprite;
        }
    }

    protected void OnSelectQuest (int index, fuckRPGLib.GameCode.QuestCaption questCaption)
    {
        // show the detail of a quest
        questDetailsUI.SetActive(true);
        questNameText.text = questCaption.name;
        questDescriptionText.text = questCaption.description;
        m_attemptQuestClaim = index;
        m_questClaimCaption = questCaption;
    }

    public void OnConfirmQuest ( )
    {
        // max amount of quest player can claim
        if (PlayerData.GetInstance().questCount == QuestManager.maxQuestCount)
            return;

        // update the avalible quest list
        // avalible quest list should not be changed
        // m_avalibleQusets.RemoveAt(m_attemptQuestClaim);
        m_avaCount--;
        //m_questUIs[m_attemptQuestClaim].GetComponent<Button>().enabled = false;
        PoolManager.GetInstance().GetPool(questListItemUIPrefab.name).GivebackObject(m_questUIs[m_attemptQuestClaim]);
        AdjustQuestListUISize();
        questDetailsUI.SetActive(false);

        // add the quest to player's quest list
        PlayerData.GetInstance().questCaptions[PlayerData.GetInstance().questCount] = m_questClaimCaption;
        PlayerData.GetInstance().questCount++;
        m_avalibleQusets[m_attemptQuestClaim].Claim();
    }

    public void OnRejectQuest ( )
    {
        questDetailsUI.SetActive(false);
    }

    private void UpdateAvalibleQuestList ( )
    {
        // adjust the size of th grid
        AdjustQuestListUISize();
        // show the avalible quest list on the quest grid UI
        for (int i = 0; i < m_avaCount; i++)
        {
            GameObject questUI = PoolManager.GetInstance().GetPool(questListItemUIPrefab).GetObject();
            questUI.GetComponent<Image>().sprite = GetQuestButtonSprite(m_avalibleQusets[i].isMain);
            questUI.GetComponentInChildren<Text>().text = m_avalibleQusets[i].name;
            questUI.transform.SetParent(questGridUI);
            questUI.transform.localScale = Vector3.one;

            // add a click event, by passing a index of avalible quest
            Button bt = questUI.GetComponent<Button>();
            bt.onClick.RemoveAllListeners();

            // a new int must be claimed used for the arg in the callback
            // we pass a value, not reference, to the listner
            fuckRPGLib.GameCode.QuestCaption questCaption = new fuckRPGLib.GameCode.QuestCaption(m_avalibleQusets[i].name, m_avalibleQusets[i].description, m_avalibleQusets[i].scene);
            int index = i;
            bt.onClick.AddListener(delegate( )
            {
                this.OnSelectQuest(index, questCaption);
            });
            m_questUIs.Add(questUI);
        }
    }

    private void AdjustQuestListUISize ( )
    {
        float height = 10 + (m_avaCount + 1) / 2 * (int)(10 + h);
        if (height < 220)
        {
            ((RectTransform)questGridUI.parent).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 10);
            ((RectTransform)questGridUI.parent).localPosition = new Vector3(0, 110 - height / 2, 0);
        }
        questGridUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 10);
        questGridUI.localPosition = new Vector3(0, -height / 2, 0);
    }



}
