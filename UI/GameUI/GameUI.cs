using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

public class GameUI : MonoBehaviour
{

    public Sprite[] atkSprites = new Sprite[8];
    public ETCButton atkButton;
    public Image atkImage;

    public Sprite[] avatarSprites = new Sprite[4];
    public Image avatarImage;

    public Slider lifeSlider;
    public Text lifeText;
    public Text lifeRegeText;
    public Slider manaSlider;
    public Text manaText;
    public Text manaRegeText;

    public GameObject[] skillTrees;

    public GameObject[] menuButtons;
    public GameObject[] menus;

    public Slider expSlider;

    public ETCButton[] skillButtons = new ETCButton[3];
    public Image[] cd = new Image[3];
    public Image[] skillImages = new Image[3];
    public Sprite[] fighterSkillSprites = new Sprite[6];
    public Sprite[] mageSkillSprites = new Sprite[6];
    public Sprite[] hunterSkillSprites = new Sprite[6];
    public Sprite[] monkSkillSprites = new Sprite[6];
    private Sprite[] m_skillSprites;

    private int m_classIndex;

    // Use this for initialization
    void Awake ( )
    {
        SetInitSprite();

    }

    // Update is called once per frame
    void Update ( )
    {

    }

    private void SetInitSprite ( )
    {
        m_classIndex = (int)PlayerData.GetInstance().GetClassName();
        switch (m_classIndex)
        {
            case 0:
                m_skillSprites = fighterSkillSprites;
                break;
            case 1:
                m_skillSprites = mageSkillSprites;
                break;
            case 2:
                m_skillSprites = hunterSkillSprites;
                break;
            case 3:
                m_skillSprites = monkSkillSprites;
                break;
            default:
                break;
        }

        // relating to the player
        PlayerData.GetInstance().gameUI = this;

        // attack sprite
        atkImage.sprite = atkSprites[m_classIndex * 2];
        atkButton.normalSprite = atkSprites[m_classIndex * 2];
        atkButton.pressedSprite = atkSprites[m_classIndex * 2 + 1];

        // class avatar sprite
        avatarImage.sprite = avatarSprites[m_classIndex];

        // set active skill button
        List<int> skillIntNames = GameCode.skillsAllClasses[m_classIndex];
        for (int i = 0; i < skillIntNames.Count; i++)
        {
            if (skillIntNames[i] % 10 == 0 && PlayerData.GetInstance().hasLearned[i])
            {
                int j = skillIntNames[i] / 10;
                SetSkillActive(j);
            }
        }

        // set bar
        SetExpSlider(PlayerData.GetInstance().exp, PlayerData.GetInstance().level);
        SetLife(PlayerData.GetInstance().curLife / PlayerData.GetInstance().maxLife);
        SetMana(PlayerData.GetInstance().curMana / PlayerData.GetInstance().maxMana);

        lifeText.text = PlayerData.GetInstance().curLife + " / " + PlayerData.GetInstance().maxLife;
        lifeRegeText.text = "+" + PlayerData.GetInstance().healingRate;
        manaText.text = PlayerData.GetInstance().curMana + " / " + PlayerData.GetInstance().maxMana;
        manaRegeText.text = "+" + PlayerData.GetInstance().manaingRate;

        // set skill tree depend on class
        GameObject skillTree = PoolManager.GetInstance().GetPool(skillTrees[m_classIndex]).GetObject();
        skillTree.transform.SetParent(menus[(int)GameCode.MenuName.Skill].transform);
        skillTree.transform.localPosition = Vector3.zero;
        skillTree.transform.localScale = Vector3.one;
    }

    public void SetLife (float value)
    {
        StartCoroutine(DoSetLifeSlider(value));
        lifeText.text = PlayerData.GetInstance().curLife.ToString("0") + " / " + PlayerData.GetInstance().maxLife;
    }

    private IEnumerator DoSetLifeSlider (float value)
    {
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            yield return 0;
            lifeSlider.value = Mathf.Lerp(lifeSlider.value, value, Time.deltaTime * 4);
        }
    }

    public void SetMana (float value)
    {
        StartCoroutine(DoSetManaSlider(value));
        manaText.text = PlayerData.GetInstance().curMana.ToString("0") + " / " + PlayerData.GetInstance().maxMana;
    }

    private IEnumerator DoSetManaSlider (float value)
    {
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            yield return 0;
            manaSlider.value = Mathf.Lerp(manaSlider.value, value, Time.deltaTime * 4);
        }
    }

    public void OpenCloseMenu (int i)
    {
        if (menus[i].activeSelf)
            menus[i].SetActive(false);
        else
            menus[i].SetActive(true);

    }

    public void SetExpSlider (int exp, int level)
    {
        float curExp = exp - (level - 1) * (100 + 100 + 10 * (level - 2)) / 2;
        float curNeed = 100 + (level - 1) * 10;
        expSlider.value = curExp / curNeed;
    }

    // when player learn the ith active skill
    public void SetSkillActive (int i)
    {
        skillButtons[i].activated = true;
        skillButtons[i].normalSprite = m_skillSprites[i * 2];
        skillButtons[i].pressedSprite = m_skillSprites[i * 2 + 1];
        skillImages[i].sprite = m_skillSprites[i * 2];
    }

    public void DisplayCD(int i, float value)
    {
        if (!cd[i].gameObject.activeSelf)
        {
            cd[i].gameObject.SetActive(true);
        }
        else
        {
            cd[i].fillAmount = value;
            if (value == 0)
            {
                cd[i].gameObject.SetActive(false);
            }
        }
    }

}
