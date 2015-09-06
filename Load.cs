using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Load : MonoBehaviour
{
    private Dictionary<string, string> m_mapBrieves;

    public Text mapBriefText;

    private AsyncOperation m_async;

    public Slider progress;
    //public Text proText;

    // Use this for initialization
    void Start ( )
    {
        //proText = GetComponentInChildren<Text>();
        StartCoroutine(LoadGame());
        m_mapBrieves = new Dictionary<string, string>()
        {
            {"cave", "cave"},
            {"snow", "snow"},
            {"air island", "air island"},
            {"dungeon boss", "dungeon boss"},
            {"main", "欢迎来到FUCKRPG"}
        };
        mapBriefText.text = m_mapBrieves[PlayerPrefs.GetString("next scene")];
        StartCoroutine(LoadGame());
    }

    // Update is called once per frame
    void Update ( )
    {
        progress.value = m_async.progress;
        //proText.text = (int)(m_async.progress * 100) + "%";
    }

    private IEnumerator LoadGame()
    {
        m_async = Application.LoadLevelAsync(PlayerPrefs.GetString("next scene"));
        yield return m_async;
    }
}
