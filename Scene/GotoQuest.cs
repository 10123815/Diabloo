using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GotoQuest : MonoBehaviour
{

    public GameObject gotoUI;

    // Use this for initialization
    void Start ( )
    {
    }

    public void OnTriggerEnter (Collider collider)
    {
        if (collider.tag.Equals("Player") && PlayerData.GetInstance().questCount > 0)
        {
            gotoUI.SetActive(true);
        }
    }

    public void OnTriggerExit (Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            gotoUI.SetActive(false);
        }
    }

    public void Go ( )
    {
        //PlayerPrefs.SetString("next scene", "world map");
        Application.LoadLevel("world map");
    }

    public void Cancel ( )
    {
        gotoUI.SetActive(false);
    }
}
