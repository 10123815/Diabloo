using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartUI : MonoBehaviour
{

    public GameObject gameStart;
    private Button m_gameStartButton;

    // Use this for initialization
    void Start ( )
    {
        m_gameStartButton = gameStart.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update ( )
    {

    }

    public void OnGameStart()
    {
        // whenever play saves the game, the "new" will excatly be 0, and the player data will be saved at hard disk
        if (PlayerPrefs.GetInt("new") == 1)
        {
            /* Create a new hero */
        }
        else
        {
            /* Load player data and jump to game scene */
        }
    }
}
