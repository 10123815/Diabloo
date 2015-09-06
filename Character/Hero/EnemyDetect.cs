using UnityEngine;
using System.Collections;

public class EnemyDetect : MonoBehaviour
{

    protected CharacterAction m_action;

    // Use this for initialization
    void Start ( )
    {
        m_action = GetComponentInParent<CharacterAction>();
    }

    // Update is called once per frame
    void Update ( )
    {

    }

    virtual public void OnTriggerEnter (Collider collider)
    {
        if (collider.tag == "Enemy")
        {
            m_action.targets.Add(collider.gameObject);
        }
    }

    virtual public void OnTriggerExit (Collider collider)
    {
        if (!collider.tag.Equals("Enemy"))
        {
            return;
        }

        int index = m_action.targets.FindIndex((GameObject go) =>
        {
            return go.GetInstanceID() == collider.gameObject.GetInstanceID();
        });
        if (index != -1)
        {
            m_action.targets.RemoveAt(index);
            // disable "checking" effect when the enemy leave player's attacking area
            m_action.UndoCheckEffect(index);
        }
    }
}
