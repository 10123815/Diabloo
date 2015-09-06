using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class AreaControllSkill : MonoBehaviour
{
    public string controlType;
    public float time;
    public float d;
    public float atk;

    // the tag of characters this script want to control
    public string targetTag = "Enemy";

    private SphereCollider m_collider;
    // disable collider after this time
    public float disableTime;

    // Use this for initialization
    void Start ( )
    {
        m_collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update ( )
    {
        m_collider.enabled = true;
        StartCoroutine(DisableTrigger());
        StartCoroutine(Return());
    }

    public void OnTriggerEnter (Collider collider)
    {
        if (controlType.Equals(""))
            Debug.LogError("NUll Control type");
        
        if (collider.tag.Equals(targetTag))
        {
            if (targetTag.Equals("Player"))
            {
                PlayerData.GetInstance().Damaged(atk);
            }
            else
            {
                Damage(collider.transform);
            }

            Controlled ctrl = collider.GetComponent<Controlled>();
            if (ctrl == null)
                Debug.LogError("Controlled is not found");
            if (ctrl.controller == null)
                ctrl.controller = PlayerData.GetInstance().action.gameObject;
            StartCoroutine(ctrl.controlFuncDic[controlType](time, d));
        }

    }

    protected void Damage (Transform target)
    {
        BaseData data = target.GetComponent<BaseData>();
        if (data != null)
        {
            data.Damaged(atk);
            // the shooting may launch some adnormal stat on enemy
            if (data.curLife <= 0)
            {
                PlayerData.GetInstance().GainExp(data.valueExp);
                PlayerData.GetInstance().gold += data.valueGold;
            }

        }
    }

    private IEnumerator DisableTrigger ( )
    {
        yield return new WaitForSeconds(disableTime);
        m_collider.enabled = false;
    }
    private IEnumerator Return ( )
    {
        yield return new WaitForSeconds(time * 1.1f);
        PoolManager.GetInstance().GetPool(gameObject.name).GivebackObject(gameObject);
    }
}
