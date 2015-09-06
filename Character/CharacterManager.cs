//----------------------------------------------
// this script is used for creating player and
// enemys in every scene. It must be compiled 
// and executed first!!!
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

public class CharacterManager : MonoBehaviour
{

    public static CharacterManager charMng;

    public GameObject player;
    public Vector3 playerOriginPosition;
    public GameObject[] playerPrefabs;

    public Transform enemyUICanvas;
    public GameObject enemyLifeBar;
    public GameObject[] enemyPrefabs;
    public Slider bossHpSlider;

    // hero is healed by himself at anytime
    private float curedLife = 0;
    private float curedMana = 0;

    // for i, the enemy of type enemyPrefabs[enemyIndecies[i]] will be created at positions[i]
    public int[] enemyIndecies;
    public Vector3[] positions;
    public bool[] triggers;

    // fixed frame rate
    public int targetFrameRate = 30;

    void Awake ( )
    {
        charMng = this;

        Application.targetFrameRate = targetFrameRate;

        PlayerData.GetInstance().LoadAll();
        PlayerData.GetInstance().canMove = true;
        GameCode.ClassName className = PlayerData.GetInstance().GetClassName();
        player = (GameObject)PoolManager.GetInstance().GetPool(playerPrefabs[(int)className]).GetObject(playerOriginPosition);
        SkillManager skillMng = player.GetComponent<SkillManager>();
        for (int i = 0; i < PlayerData.GetInstance().hasLearned.Length; i++)
        {
            if (PlayerData.GetInstance().hasLearned[i])
            {
                skillMng.Learn(i);
            }
        }
    }

    // Use this for initialization
    void Start ( )
    {
        for (int i = 0; i < enemyIndecies.Length; i++)
        {
            if (triggers[i])
            {
                int enemyIndex = enemyIndecies[i];
                CreateOneEnemy(enemyIndex, positions[i]); 
            }
        }

    }

    // Update is called once per frame
    void Update ( )
    {
        PlayerCuredSelf();
    }

    // creating a enemy at a explicitly
    private void CreateOneEnemy (int enemyIndex, Vector3 position)
    {
        if (enemyIndex >= enemyPrefabs.Length)
        {
            Debug.LogWarning("We do not have this kind of enemy");
            return;
        }

        GameObject enemy = (GameObject)PoolManager.GetInstance().GetPool(enemyPrefabs[enemyIndex]).GetObject(position);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.player = player;
        if (enemyController.enemyType == fuckRPGLib.GameCode.EnemyType.Dogface)
        {
            GameObject hpBar = (GameObject)PoolManager.GetInstance().GetPool(enemyLifeBar).GetObject();
            hpBar.transform.SetParent(enemyUICanvas);
            ((DogFaceControllerBase)enemyController).hpBar = hpBar;
            Slider slider = hpBar.GetComponent<Slider>();
            slider.value = 1;
            ((DogFaceControllerBase)enemyController).hpSlider = slider;
        }
        else if (enemyController.enemyType == fuckRPGLib.GameCode.EnemyType.Boss)
        {
            bossHpSlider.gameObject.SetActive(true);
            ((BossAction)enemyController).hpSlider = bossHpSlider;
        }

        // every time we create a enemy, we should relate this enemy to the quest system, achievements system...
        // these systems are created before enemies, so we a have to do the subcribe here
        enemyController.enemyDeathEvent.AddListener(QuestManager.EnemyDeathHandler);

    }

    private void CreateEnemies (int enemyIndex, int count)
    {

    }

    private void PlayerCuredSelf ( )
    {
        if (PlayerData.GetInstance().curLife <= 0)
            return;

        curedLife += Time.deltaTime * PlayerData.GetInstance().healingRate;
        curedMana += Time.deltaTime * PlayerData.GetInstance().manaingRate;
        if (curedLife >= 1)
        {
            PlayerData.GetInstance().Cured(1, 0);
            curedLife = 0;
        }
        if (curedMana >= 1)
        {
            PlayerData.GetInstance().Cured(0, 1);
            curedMana = 0;
        }
    }

    public void OnSetTrigger(int i)
    {
        triggers[i] = true;
        int enemyIndex = enemyIndecies[i];
        CreateOneEnemy(enemyIndex, positions[i]); 
    }
}
