using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorldMap : MonoBehaviour
{

    // 
    private string m_attempQuestScene;

    // caption of quest player attempt to take in
    public Text[] texts;

    // quest introduction arrow prefab
    public GameObject arrowPrefab;

    // delta position of a instance
    public static Dictionary<string, Vector2> deltaPositions = new Dictionary<string, Vector2>()
        {
            {"snow", new Vector2(0.27f, 0.85f)},
            {"air island", new Vector2(.6f, .75f)},
            {"hell", new Vector2(.57f, .08f)},
            {"dungeon boss", new Vector2(.35f, .55f)},
        };

    // scroll bar to scale the map
    public Scrollbar bar;

    // Use this for initialization
    void Start ( )
    {
        Application.targetFrameRate = 30;
        for (int i = 0; i < PlayerData.GetInstance().questCaptions.Length; i++)
        {
            if (PlayerData.GetInstance().questCaptions[i].scene != null)
            {
                string scene = PlayerData.GetInstance().questCaptions[i].scene;
                GameObject go = PoolManager.GetInstance().GetPool(arrowPrefab).GetObject(transform);
                go.transform.position = GetQuestPosition(scene);
                int index = i;
                Button bt = go.GetComponent<Button>();
                bt.onClick.AddListener(delegate( )
                {
                    OnSelectQuest(index);
                });
            }
        }
    }

    #region scroll
    public void OnValueChanged ( )
    {
        Vector3 scale = new Vector3(bar.value, bar.value, 0) * 2 + Vector3.one;
        transform.localScale = Vector3.Lerp(transform.localScale, scale, 30 * Time.deltaTime);
    }

    public void OnAdd ( )
    {
        bar.value += 0.05f;
    }

    public void OnMinus ( )
    {
        bar.value -= 0.05f;
    }
    #endregion

    private Vector3 GetQuestPosition (string scene)
    {
        Vector3 pos = new Vector3();
        pos.x = deltaPositions[scene].x * Screen.width;
        pos.y = deltaPositions[scene].y * Screen.height;
        return pos;
    }

    // i : the index in player's quest list
    private void OnSelectQuest (int i)
    {
        fuckRPGLib.GameCode.QuestCaption qc = PlayerData.GetInstance().questCaptions[i];
        texts[0].text = qc.name;
        m_attempQuestScene = texts[1].text = qc.scene;
        texts[2].text = qc.description;
    }

    public void Go ( )
    {
        PlayerPrefs.SetString("next scene", m_attempQuestScene);
        Application.LoadLevel("load");
    }

}
