using UnityEngine;
using System.Collections;

public class BossTrigger : MonoBehaviour
{

    public int bossIndex;
    private NavMeshObstacle m_obstacle;
    private BoxCollider m_collider;

    void Start ( )
    {
        m_collider = GetComponent<BoxCollider>();
        m_obstacle = GetComponent<NavMeshObstacle>();
    }

    public void OnTriggerEnter (Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            CharacterManager.charMng.OnSetTrigger(bossIndex);
            m_collider.enabled = false;
            m_obstacle.enabled = true;
        }
    }
}
