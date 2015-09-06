using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using fuckRPGLib;

public class ChracterCreator : MonoBehaviour
{

    private Transform m_camTF;

    private Vector3[] m_camPos = new Vector3[4]
    {
        new Vector3(11, 14f, -68),
        new Vector3(9, 15, -71.5f),
        new Vector3(9, 14.5f, -75.5f),
        new Vector3(9.5f, 14.27f, -78.5f)
    };

    private Vector3[] m_camEul = new Vector3[4]
    {
        new Vector3(13, 121, 0),
        new Vector3(13, 95, 0),
        new Vector3(13, 85, 0),
        new Vector3(13, 90, 0)
    };

    // at once we come back to a previous creating page,
    // the setting we config in current page will ne initiated
    private delegate void Init ( );
    private Init[] m_initFuncs = new Init[4];

    // three stages of creating a class
    public enum CreatingStage
    {
        ClassSelection = 0,
        PropertyAdjustment = 1,
        BloodandGift = 2
    }
    public CreatingStage m_creatingStage = CreatingStage.ClassSelection;

    // creating stages switching button
    public Button previousStageButton, nextStageButton;

    // creator gameobjects
    public GameObject[] creators;

    // game logo, display when player has completed the character creation
    public Image logo;
    public Image bg;

    // four classes can be choosed
    public GameObject[] classAvatarGOes;
    public Text briefText;
    public Text classNameText;
    public string[] classNames;
    public string[] brieves;

    // unallocated point
    public Text pointToSpendText;

    // six basci attributes 
    public Text[] basciAttrValueTexts;
    public Text attrBriefText;
    public string[] attrBrieves;

    // seven birthes to select
    //public Text birthNameText;
    //public Text birthBriefText;
    public string[] birthNames;
    //public string[] birthBrieves;

    // player name's input field
    public InputField playerNameInput;

    // review
    public Text confirmClassText;
    public Text confirmBrithText;

    public int frameRate = 30;

    void Awake ( )
    {
        Application.targetFrameRate = frameRate;
    }

    // Use this for initialization
    void Start ( )
    {
        m_camTF = Camera.main.transform;

        m_initFuncs[0] = Init0;
        m_initFuncs[1] = Init1;
        m_initFuncs[2] = Init2;
        m_initFuncs[3] = Init3;

        Init0();
    }

    // Update is called once per frame
    void Update ( )
    {

    }

    private void Init0 ( )
    {
        int _i = (int)PlayerData.GetInstance().GetClassName();
        classAvatarGOes[_i].transform.localScale *= 1.2f;
        briefText.text = brieves[_i];
        classNameText.text = classNames[_i];
    }

    private void Init1 ( )
    {
        PlayerData.GetInstance().SetPointToSpend(5);
        // init all basic attributes of the class
        PlayerData.GetInstance().SetClass(PlayerData.GetInstance().GetClassName());
        pointToSpendText.text = "5";
        for (int i = 0; i < GameCode.basicAttributeCount; i++)
        {
            basciAttrValueTexts[i].text = PlayerData.GetInstance().GetAttributeValue((GameCode.BasicAttributeName)i).ToString();
        }
        attrBriefText.text = "";
    }

    private void Init2 ( )
    {
        PlayerData.GetInstance().SetBirth(GameCode.BirthAndGiftName.Nobility);
        //birthBriefText.text = birthBrieves[0];
        //birthNameText.text = birthNames[0];
    }

    private void Init3 ( )
    {
        playerNameInput.text = PlayerData.GetInstance().GetPlayerName();
        confirmClassText.text = classNames[(int)PlayerData.GetInstance().GetClassName()];
        confirmBrithText.text = birthNames[(int)PlayerData.GetInstance().GetBrith()];
    }

    #region creating stages
    public void OnNextClick ( )
    {
        int _currentStageIndex = (int)m_creatingStage;
        creators[_currentStageIndex].SetActive(false);
        if (_currentStageIndex == creators.Length - 1)
        {
            // now we has came the last stage of creating
            // the player name should not be null
            if (!PlayerData.GetInstance().GetPlayerName().Equals(""))
            {
                // The player has completed the class creating, and should be leaded to the game
                PlayerData.GetInstance().SaveAll();
                //move the camera
                MovementPath mmPath = Camera.main.GetComponent<MovementPath>();
                mmPath.enabled = true;
                mmPath.OnReach = JumpToGame;
                previousStageButton.gameObject.SetActive(false);
                nextStageButton.gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            m_creatingStage = (CreatingStage)(++_currentStageIndex);
            creators[_currentStageIndex].SetActive(true);
            m_initFuncs[_currentStageIndex]();
        }
    }

    private void JumpToGame ( )
    {
        StartCoroutine(DoJumpToGame());
    }

    private IEnumerator DoJumpToGame(float time = 3.0f)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            // alpha is from 0 to 1
            logo.color = new Color(255.0f, 255.0f, 255.0f, i / time);
            bg.color = new Color(0.0f, 0.0f, 0.0f, i / time);
            yield return 0;
        }
        yield return new WaitForSeconds(2);
        // jump to game scene "Starter's Village"
        PlayerPrefs.SetString("next scene", "main");
        Application.LoadLevel("load");
    }

    public void OnPreviousClick ( )
    {
        int _currentStageIndex = (int)m_creatingStage;
        creators[_currentStageIndex].SetActive(false);
        if (_currentStageIndex == 0)
        {
            /* Return to the Main */
            print("Return to main!");
            // return to "start" scene

            return;
        }
        else
        {
            m_initFuncs[_currentStageIndex]();
            m_creatingStage = (CreatingStage)(--_currentStageIndex);
            creators[_currentStageIndex].SetActive(true);
        }
    }
    #endregion

    #region class selection

    /*
     * _lastClassIndex : index of the current selected class
     * _classIndex : index of the class we will select
     */
    private void DoSelectClassDisplay (int _lastClassIndex, int _classIndex)
    {
        classAvatarGOes[_lastClassIndex].transform.localScale *= 0.83f;
        classAvatarGOes[_classIndex].transform.localScale *= 1.2f;
        briefText.text = brieves[_classIndex];
        classNameText.text = classNames[_classIndex];
        // move the camera
        StartCoroutine(MoveCamera(m_camPos[_classIndex], m_camEul[_classIndex]));
        // store in the save data
        PlayerData.GetInstance().SetClass((GameCode.ClassName)_classIndex);
    }

    private IEnumerator MoveCamera (Vector3 pos, Vector3 eul)
    {
        while (Vector3.Distance(m_camTF.position, pos) > 0.1f)
        {
            m_camTF.position = Vector3.Lerp(m_camTF.position, pos, Time.deltaTime * 10);
            m_camTF.eulerAngles = Vector3.Lerp(m_camTF.eulerAngles, eul, Time.deltaTime * 10);
            yield return 0;
        }
    }

    public void OnClassNext ( )
    {
        int _currentClassName = (int)PlayerData.GetInstance().GetClassName();
        if (_currentClassName < GameCode.classCount - 1)
        {
            DoSelectClassDisplay(_currentClassName, _currentClassName + 1);
        }
    }

    public void OnClassPrevious ( )
    {
        int _currentClassName = (int)PlayerData.GetInstance().GetClassName();
        if (_currentClassName > 0)
        {
            DoSelectClassDisplay(_currentClassName, _currentClassName - 1);
        }
    }

    public void OnClassSelected (int _index)
    {
        int _currentClassName = (int)PlayerData.GetInstance().GetClassName();
        DoSelectClassDisplay(_currentClassName, _index);
    }
    #endregion

    #region attributes adjustment

    // set the brief text of this attribute regardless of if we change the point
    private void DoSetAttrText (int _index)
    {
        attrBriefText.text = attrBrieves[_index];
    }

    public void OnAddingPointTo (int _attrIndex)
    {
        GameCode.BasicAttributeName _basicAttrName = (GameCode.BasicAttributeName)_attrIndex;
        // the current point of the attribute we want to change
        int _currentPoint = PlayerData.GetInstance().GetAttributeValue(_basicAttrName);
        // unallocated point we have now
        int _pointToSpend = PlayerData.GetInstance().GetPointToSpend();
        if (_pointToSpend > 0 && _currentPoint + 1 < GameCode.maxAttributeValue)
        {
            PlayerData.GetInstance().SetPointToSpend(--_pointToSpend);
            PlayerData.GetInstance().SetAtttributeValue(_basicAttrName, ++_currentPoint);
            pointToSpendText.text = _pointToSpend.ToString();
            basciAttrValueTexts[_attrIndex].text = _currentPoint.ToString();
        }

        DoSetAttrText(_attrIndex);
    }

    public void OnMinusingPointTo (int _attrIndex)
    {
        GameCode.BasicAttributeName _basicAttrName = (GameCode.BasicAttributeName)_attrIndex;
        // the current point of the attribute we want to change
        int _currentPoint = PlayerData.GetInstance().GetAttributeValue(_basicAttrName);
        // unallocated point we have now
        int _pointToSpend = PlayerData.GetInstance().GetPointToSpend();
        if (_currentPoint - 1 > GameCode.minAttibuteValue)
        {
            PlayerData.GetInstance().SetPointToSpend(++_pointToSpend);
            PlayerData.GetInstance().SetAtttributeValue(_basicAttrName, --_currentPoint);
            pointToSpendText.text = _pointToSpend.ToString();
            basciAttrValueTexts[_attrIndex].text = _currentPoint.ToString();
        }

        DoSetAttrText(_attrIndex);
    }

    public void OnTouchAttr (int _attrIndex)
    {
        DoSetAttrText(_attrIndex);
    }
    #endregion

    #region Birth Selection
    public void OnSelectingBirth (int _birthIndex)
    {
        PlayerData.GetInstance().SetBirth((GameCode.BirthAndGiftName)_birthIndex);
        //birthBriefText.text = birthBrieves[_birthIndex];
        //birthNameText.text = birthNames[_birthIndex];
    }

    public void OnSelectingBrithRandom ( )
    {
        OnSelectingBirth((int)(Random.value * 6.99f));
    }
    #endregion

    #region review
    public void OnEnteredPlayerName ( )
    {
        PlayerData.GetInstance().SetPlayerName(playerNameInput.text);
    }
    #endregion
}
