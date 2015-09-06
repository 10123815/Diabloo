using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

public class PlayerData : BaseData
{

    // a singleton
    public static PlayerData GetInstance ( )
    {
        return Singleton<PlayerData>.GetInstance();
    }
    private PlayerData ( )
    {
        // init extra 5 points for player to deploy them themselves
        m_pointsToSpend = 5;
        // initiated with "Fighter" class
        SetClass(GameCode.ClassName.Fighter);
    }

    // player can claim at most 5 quest at the same time
    // these quest should be saved when player save the game
    [System.NonSerialized]
    public byte questCount = 0;
    [System.NonSerialized]
    public GameCode.QuestCaption[] questCaptions = new GameCode.QuestCaption[QuestManager.maxQuestCount];

    // the character action of player, we need it to add effect...
    [System.NonSerialized]
    public CharacterAction action;

    // the main UI in the game, which display the player's life, mana, skill.....
    [System.NonSerialized]
    public GameUI gameUI;

    [System.NonSerialized]
    public GameObject levelUpEffect;

    // some extra attributes the only heros have
    // init with 500 golds
    public int gold = 500;
    // the exp we need to level up to k
    private int m_needExp = 100;
    public int level = 1;
    private int m_maxLevel = 50;
    public int exp = 0;
    // the default username
    private string m_playerName = "fuckRPG";
    // regeneration rate of life and mana
    public float healingRate;
    public float manaingRate;
    // armour piercing
    public float armPieRate = 0;

    // unallocated point of basic attributes
    // initiated with 5 points
    // one new point will gain every time you levelup
    private int m_pointsToSpend = 5;

    private GameCode.ClassName m_className;

    private int[] m_basicAttributes = new int[GameCode.basicAttributeCount];

    // the equipment player has purchased
    public bool[] hasPurchased;

    // if the player have learned one skill with the index of it
    // we confirm its length when player selects class and initiate it with false
    // when we load game, we also new it, and set the value with data in hd
    public bool[] hasLearned;

    private GameCode.BirthAndGiftName m_birthName;

    public void SetPlayerName (string _name)
    {
        m_playerName = _name;
    }

    public string GetPlayerName ( )
    {
        return m_playerName;
    }

    public void SetClass (GameCode.ClassName _name)
    {
        m_className = _name;
        int classIndex = (int)m_className;
        for (int i = 0; i < 6; i++)
        {
            SetAtttributeValue((GameCode.BasicAttributeName)i, GameCode.InitBasicAttributes[classIndex, i]);
        }
        hasLearned = new bool[GameCode.skillsAllClasses[classIndex].Count];
        if (classIndex == 0)
            atkRange = 3;
        else if (classIndex == 1)
            atkRange = 8;
        else if (classIndex == 2)
            atkRange = 10;
        else
            atkRange = 2;
    }

    public GameCode.ClassName GetClassName ( )
    {
        return m_className;
    }

    public void SetAtttributeValue (GameCode.BasicAttributeName _name, int _value)
    {
        m_basicAttributes[(int)_name] = _value;

        // set the general and extra attributes depend on the basic attributs
        switch (_name)
        {
            case GameCode.BasicAttributeName.Agility:
                hitRate = _value / GameCode.maxAttributeValue * 0.2f + 0.8f;
                dodgeRate = _value / GameCode.maxAttributeValue;
                break;
            case GameCode.BasicAttributeName.Consitution:
                maxLife = _value * 25 + 100;
                curLife = maxLife;
                healingRate = _value * 0.01f + 0.1f;
                break;
            case GameCode.BasicAttributeName.Dexterity:
                atkSpeed = _value / GameCode.maxAttributeValue * 2.0f + 1.0f;
                moveSpeed = _value / GameCode.maxAttributeValue * 6 + 6;
                break;
            case GameCode.BasicAttributeName.Intelligence:
                break;
            case GameCode.BasicAttributeName.Mentality:
                maxMana = _value * 15 + 50;
                curMana = maxMana;
                manaingRate = _value * 0.01f + 0.1f;
                break;
            case GameCode.BasicAttributeName.Strength:
                atk = _value + 30;
                crit = atk;
                critRate = 0;
                break;
            default:
                break;
        }
    }

    public int GetAttributeValue (GameCode.BasicAttributeName _name)
    {
        return m_basicAttributes[(int)_name];
    }

    public void SetPointToSpend (int _point)
    {
        m_pointsToSpend = _point;
    }

    public int GetPointToSpend ( )
    {
        return m_pointsToSpend;
    }

    public void SetBirth (GameCode.BirthAndGiftName _birth)
    {
        m_birthName = _birth;
        switch (m_birthName)
        {
            // extra 300 init gold
            case GameCode.BirthAndGiftName.Businessman:
                break;
            // only 200 init gold, but faster growness
            case GameCode.BirthAndGiftName.Genius:
                break;
            // small littel cost when buy something in a city
            case GameCode.BirthAndGiftName.Nobility:
                break;
            // nothing can we gain at first
            case GameCode.BirthAndGiftName.Nothing:
                break;
            // a unkown stone, will generate extra award when player completes a quest
            case GameCode.BirthAndGiftName.Protagonist:
                break;
            // tp with cd
            case GameCode.BirthAndGiftName.Risker:
                break;
            // unlocking skill
            case GameCode.BirthAndGiftName.Rogue:
                break;
            default:
                break;
        }
    }

    public GameCode.BirthAndGiftName GetBrith ( )
    {
        return m_birthName;
    }

    // save all members into hard disk
    public void SaveAll ( )
    {
        PlayerPrefs.SetInt("gold", gold);
        PlayerPrefs.SetString("player name", m_playerName);
        PlayerPrefs.SetInt("class index", (int)m_className);
        PlayerPrefs.SetInt("birth index", (int)m_birthName);
        PlayerPrefs.SetInt("points to spend", m_pointsToSpend);
        PlayerPrefs.SetInt("exp", exp);
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetFloat("armour peircing", armPieRate);
        // six basic attributes
        for (int i = 0; i < 6; i++)
        {
            PlayerPrefs.SetInt(((GameCode.BasicAttributeName)i).ToString(), m_basicAttributes[i]);
        }
        // the skills learning stat
        int classIndex = (int)m_className;
        for (int i = 0; i < hasLearned.Length; i++)
        {
            PlayerPrefs.SetInt(GameCode.skillsAllClasses[classIndex][i].ToString(), (hasLearned[i]) ? 1 : 0);
        }
        #region general attributes
        PlayerPrefs.SetFloat("curLife", curLife);
        PlayerPrefs.SetFloat("maxLife", maxLife);
        PlayerPrefs.SetFloat("curMana", curMana);
        PlayerPrefs.SetFloat("maxMana", maxMana);
        PlayerPrefs.SetFloat("atk", atk);
        PlayerPrefs.SetFloat("maxAtk", maxAtk);
        PlayerPrefs.SetFloat("def", def);
        PlayerPrefs.SetFloat("maxDef", maxDef);
        PlayerPrefs.SetFloat("crit", crit);
        PlayerPrefs.SetFloat("critRate", critRate);
        PlayerPrefs.SetFloat("hitRate", hitRate);
        PlayerPrefs.SetFloat("atkSpeed", atkSpeed);
        PlayerPrefs.SetFloat("moveSpeed", moveSpeed);
        PlayerPrefs.SetFloat("healingRate", healingRate);
        PlayerPrefs.SetFloat("manaingRate", manaingRate);
        PlayerPrefs.SetFloat("dodgeRate", dodgeRate);
        #endregion
    }

    // load all members from hard disk
    public void LoadAll ( )
    {
        gold = PlayerPrefs.GetInt("gold");
        m_playerName = PlayerPrefs.GetString("player name");
        m_className = (GameCode.ClassName)PlayerPrefs.GetInt("class index");
        m_birthName = (GameCode.BirthAndGiftName)PlayerPrefs.GetInt("birth index");
        m_pointsToSpend = PlayerPrefs.GetInt("points to spend");
        exp = PlayerPrefs.GetInt("exp");
        level = PlayerPrefs.GetInt("level");
        armPieRate = PlayerPrefs.GetFloat("armour piercing");
        for (int i = 0; i < 6; i++)
        {
            m_basicAttributes[i] = PlayerPrefs.GetInt(((GameCode.BasicAttributeName)i).ToString());
        }
        int classIndex = (int)m_className;
        hasLearned = new bool[GameCode.skillsAllClasses[classIndex].Count];
        for (int i = 0; i < hasLearned.Length; i++)
        {
            hasLearned[i] = PlayerPrefs.GetInt(GameCode.skillsAllClasses[classIndex][i].ToString()) == 1;
        }
        #region general attributes
        curLife = PlayerPrefs.GetFloat("curLife");
        maxLife = PlayerPrefs.GetFloat("maxLife");
        curMana = PlayerPrefs.GetFloat("curMana");
        maxMana = PlayerPrefs.GetFloat("maxMana");
        atk = PlayerPrefs.GetFloat("atk");
        maxAtk = PlayerPrefs.GetFloat("maxAtk");
        def = PlayerPrefs.GetFloat("def");
        maxDef = PlayerPrefs.GetFloat("maxDef");
        crit = PlayerPrefs.GetFloat("crit");
        critRate = PlayerPrefs.GetFloat("critRate");
        hitRate = PlayerPrefs.GetFloat("hitRate");
        atkSpeed = PlayerPrefs.GetFloat("atkSpeed");
        moveSpeed = PlayerPrefs.GetFloat("moveSpeed");
        healingRate = PlayerPrefs.GetFloat("healingRate");
        manaingRate = PlayerPrefs.GetFloat("manaingRate");
        dodgeRate = PlayerPrefs.GetFloat("dodgeRate");
        #endregion

    }

    private void LevelUp ( )
    {
        m_needExp += 100 + level * 10;
        level++;
        gameUI.SetExpSlider(exp, level);
        m_pointsToSpend++;
        // other
        // ...

        // effect
        action.LevelUpEffect();
    }

    public void GainExp (int value)
    {
        exp += value;
        gameUI.SetExpSlider(exp, level);
        if (exp >= m_needExp)
            LevelUp();
    }

    public override void Damaged (float d)
    {
        if (Random.value > dodgeRate)
        {
            base.Damaged(d);
            gameUI.SetLife(curLife / maxLife);
        }
        else
        {
            // dodge enemy's attack successfully
            // display a text "闪避" at scene near the player
        }
    }

    public override void UseMana (float value)
    {
        base.UseMana(value);
        gameUI.SetMana(curMana / maxMana);
    }

    public override void Cured (float _l, float _r)
    {
        base.Cured(_l, _r);
        gameUI.SetLife(curLife / maxLife);
        gameUI.SetMana(curMana / maxMana);
    }

}
