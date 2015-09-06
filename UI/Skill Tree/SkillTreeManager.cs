//------------------------------------------------------------
// a skill tree of a class, which player depends on to learn
// skills. One point has to be spend to learn a skill
//------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

public class SkillTreeManager : MonoBehaviour
{
    private int m_classIndex;

    // the sprites of skills player have not learned
    public Sprite[] skillSprites;
    public Sprite[] skillSpriteLearneds;
    // the skill UI put at the skill tree window
    public Image[] skillImages;
    // brieves of all skills of a class
    public string[] brieves;
    // a tip that can let player know which skill they can learn
    public Text[] arrowText;

    public struct SkillButton
    {
        public ETCButton button;
        public Sprite image;
    }
    // the position on the Game UI for skill to put on
    private SkillButton[] m_skillButton = new SkillButton[3];

    // a confirm-window for player to confirm learning a skill
    public GameObject popupBox;
    // brief on the confirm box;
    public Text briefText;
    // the index of skill player want to learn
    private int m_skillAttemptIndex;
    // if the prerequisites skill has been learned
    public delegate bool HaveLearnedPrerequisites ( );
    private HaveLearnedPrerequisites[] hlps = new HaveLearnedPrerequisites[4];

    [System.NonSerialized]
    public GameUI gameUI;

    private SkillManager m_skillManager;

    // Use this for initialization
    void Start ( )
    {
        m_classIndex = (int)PlayerData.GetInstance().GetClassName();

        gameUI = PlayerData.GetInstance().gameUI;

        for (int i = 0; i < skillImages.Length; i++)
        {
            if (PlayerData.GetInstance().hasLearned[i])
            {
                skillImages[i].sprite = skillSpriteLearneds[i];
            }
            else
                skillImages[i].sprite = skillSprites[i];
        }

        hlps[(int)GameCode.ClassName.Fighter] = DoFighterPrerequisites;
        hlps[(int)GameCode.ClassName.Mage] = DoMagePrerequisites;
        hlps[(int)GameCode.ClassName.Hunter] = DoHunterPrerequisites;
        hlps[(int)GameCode.ClassName.Monk] = DoMonkPrerequisites;

        m_skillManager = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillManager>();
    }

    // Update is called once per frame
    void Update ( )
    {

    }

    // learn the indexth skill
    public void AttemptToLearn (int index)
    {
        // the skill player want to learn
        m_skillAttemptIndex = index;

        // pop up a confirm-message box
        // this box must be opned to show the brief whenever player click a skill UI
        popupBox.transform.parent.gameObject.SetActive(true);
        // this box also display a brief of the skill player want to learn
        briefText.text = "剩余技能点：" + PlayerData.GetInstance().GetPointToSpend() + "\n" + brieves[index];

        // if this skill has been learned
        if (PlayerData.GetInstance().hasLearned[index])
            briefText.text += "\n您已学习了该技能！";
        else if (!hlps[m_classIndex]())
            briefText.text += "\n您不能学习该技能！";

    }

    public void ConfirmToLearn ( )
    {
        // if this skill has been learned
        if (PlayerData.GetInstance().hasLearned[m_skillAttemptIndex] || !hlps[m_classIndex]())
            return;

        // the point to spend must >= 1
        int point = PlayerData.GetInstance().GetPointToSpend();
        if (point == 0)
            return;
        PlayerData.GetInstance().SetPointToSpend(--point);

        // let player know he has learned this skill
        PlayerData.GetInstance().hasLearned[m_skillAttemptIndex] = true;
        // set the sprite to the learing status
        skillImages[m_skillAttemptIndex].sprite = skillSpriteLearneds[m_skillAttemptIndex];

        // set the skill UI and on the button of game UI if this skill is a active skill
        // and set the button active
        int skillIntName = GameCode.skillsAllClasses[m_classIndex][m_skillAttemptIndex];
        if (skillIntName % 10 == 0)
        {
            gameUI.SetSkillActive(skillIntName / 10);
        }
        m_skillManager.Learn(m_skillAttemptIndex);
        if (!(PlayerData.GetInstance().GetClassName() == GameCode.ClassName.Monk && m_skillAttemptIndex == 5))
            if (arrowText[m_skillAttemptIndex] != null)
                arrowText[m_skillAttemptIndex].color = new Color(255, 255, 0);
        if (PlayerData.GetInstance().GetClassName() == GameCode.ClassName.Monk && m_skillAttemptIndex == 4)
            arrowText[5].color = new Color(255, 255, 0);

        // close the window when player has finished learning
        popupBox.transform.parent.gameObject.SetActive(false);
    }

    public void CancelToLearn ( )
    {
        popupBox.transform.parent.gameObject.SetActive(false);
    }

    #region prerequisites checking
    private bool DoFighterPrerequisites ( )
    {
        if (m_skillAttemptIndex == 0 ||
            m_skillAttemptIndex == 2 ||
            m_skillAttemptIndex == 5)
        {
            return true;
        }
        else if (m_skillAttemptIndex == 1 ||
            m_skillAttemptIndex == 4 ||
            m_skillAttemptIndex == 6)
        {
            return PlayerData.GetInstance().hasLearned[m_skillAttemptIndex - 1];
        }
        else if (m_skillAttemptIndex == 3)
        {
            return PlayerData.GetInstance().hasLearned[1] ||
                PlayerData.GetInstance().hasLearned[2] ||
                PlayerData.GetInstance().hasLearned[6];
        }

        return false;
    }

    private bool DoMagePrerequisites ( )
    {
        if (m_skillAttemptIndex == 0 ||
            m_skillAttemptIndex == 2 ||
            m_skillAttemptIndex == 5)
        {
            return true;
        }
        else if (m_skillAttemptIndex == 1 ||
            m_skillAttemptIndex == 3 ||
            m_skillAttemptIndex == 6 ||
            m_skillAttemptIndex == 7)
        {
            return PlayerData.GetInstance().hasLearned[m_skillAttemptIndex - 1];
        }
        else if (m_skillAttemptIndex == 4)
        {
            return PlayerData.GetInstance().hasLearned[1] ||
                PlayerData.GetInstance().hasLearned[3] ||
                PlayerData.GetInstance().hasLearned[7];
        }

        return false;
    }

    private bool DoHunterPrerequisites ( )
    {
        if (m_skillAttemptIndex == 0 ||
            m_skillAttemptIndex == 3 ||
            m_skillAttemptIndex == 5)
        {
            return true;
        }
        else
        {
            return PlayerData.GetInstance().hasLearned[m_skillAttemptIndex - 1];
        }

    }

    private bool DoMonkPrerequisites ( )
    {
        if (m_skillAttemptIndex == 0 ||
            m_skillAttemptIndex == 3 ||
            m_skillAttemptIndex == 6)
        {
            return true;
        }
        else if (m_skillAttemptIndex == 1 ||
            m_skillAttemptIndex == 2 ||
            m_skillAttemptIndex == 4 ||
            m_skillAttemptIndex == 5)
        {
            return PlayerData.GetInstance().hasLearned[m_skillAttemptIndex - 1];
        }
        else if (m_skillAttemptIndex == 7)
        {
            return PlayerData.GetInstance().hasLearned[4]
                || PlayerData.GetInstance().hasLearned[6];
        }

        return false;
    }
    #endregion


}
