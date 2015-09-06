using UnityEngine;
using System.Collections;

public class GreenDragonAction : BossAction
{

    // effect prefab
    public Transform smokeBirth;
    public GameObject smokeEffectPrefab;
    public Transform fireBirth;
    public GameObject fireEffectPrefab;
    private GameObject m_fire;
    private Transform m_fireAtkTF;
    private float m_fireOffset = 0;

    // player transform
    private Transform m_playerTF;

    private CapsuleCollider m_collider;

    public Vector3 upVec = new Vector3(0, 100, 0);

    // is flying completed
    private bool m_atAir = false;

    public override void Start ( )
    {
        m_data = GetComponent<GreenDragonData>();
        base.Start();
        m_playerTF = player.transform;
        m_collider = GetComponent<CapsuleCollider>();
    }

    protected override void Init ( )
    {
        enemyId = fuckRPGLib.GameCode.EnemyID.GreenDragon;
        base.Init();
    }

    override protected void Animate ( )
    {

        #region AnimatorStateInfo
        if (m_anmSttInfo.IsName("0.fly"))
        {
            LookAtPlayer();
            if (m_animator.GetBool("fly"))
            {
                if (!m_atAir)
                    StartCoroutine(FlyOneShot(8));
                m_animator.SetBool("fly", false);
            }
            if (m_atAir)
            {
                m_animator.SetBool("fight", true);
            }
        }
        else if (m_anmSttInfo.IsName("0.fly 2 player"))
        {
            if (HeightFromPLayer() > 4)
            {
                Fly(false);
            }
            MoveToPLayer(speed: 14, minDis: 8);
            LookAtPlayer();
            if (m_animator.GetFloat("dis") < 16)
            {
                m_animator.SetBool("fire", AtFront(m_playerTF.position));
            }
            if (Random.value < 0.01f)
                m_animator.SetBool("atk", true);

        }
        else if (m_anmSttInfo.IsName("0.fly fire"))
        {
            if (HeightFromTerrain() < 1 && HeightFromPLayer() < 6)
            {
                Fly(speed: 4);
            }
            else if (HeightFromPLayer() > 6)
            {
                Fly(false);
            }
            MoveToPLayer();
            LookAtPlayer(1);

            if (m_anmSttInfo.normalizedTime > 0.18f && m_animator.GetBool("fire"))
            {
                //StartCoroutine(LookAtPlayer(3));
                m_animator.SetBool("fire", false);
                m_fire = PoolManager.GetInstance().GetPool(fireEffectPrefab).GetObject(fireBirth.position);
                m_fire.transform.SetParent(fireBirth);
                m_fire.transform.localEulerAngles = Vector3.zero;
                Vector3 m_firePosition = Vector3.Project(m_playerTF.position + new Vector3(0, 1, 0) - transform.position, transform.forward) + transform.position;
                m_fire.transform.LookAt(m_firePosition);
                m_fireAtkTF = m_fire.transform.GetChild(0);
                m_fireAtkTF.tag = "Enemy Atk";
            }

            if (m_fire && m_fire.activeSelf)
            {
                if (m_anmSttInfo.normalizedTime > 0.98f)
                {
                    PoolManager.GetInstance().GetPool(m_fire.name).GivebackObject(m_fire);
                }
                else
                {
                    // swing the fire spray
                    m_fireAtkTF.localEulerAngles = new Vector3(0, -m_fireOffset, 0);
                }
            }
        }
        else if (m_anmSttInfo.IsName("0.fly atk"))
        {
            if (m_anmSttInfo.normalizedTime > 0.45f && m_animator.GetBool("atk"))
            {
                m_animator.SetBool("atk", false);
                Vector3 smokePosition = smokeBirth.position;
                smokePosition.y = player.transform.position.y;
                PoolManager.GetInstance().GetPool(smokeEffectPrefab).GetObject(smokePosition);
                if (Vector3.Distance(smokePosition, player.transform.position) < m_data.atkRange)
                    PlayerData.GetInstance().Damaged(m_data.atk);
            }
        }
        #endregion

        #region AnimatorTransitionInfo
        if (m_anmTrsInfo.IsUserName("f2i"))
        {
            m_atAir = false;
        }
        else if (m_anmTrsInfo.IsUserName("2f"))
        {
            //m_animator.SetBool("fly", true);
        }
        else if (m_anmTrsInfo.IsUserName("2a"))
        {

        }
        else if (m_anmTrsInfo.IsUserName("2d"))
        {
            StartCoroutine(FlyOneShot(0.1f, false));
        }
        #endregion
    }

    protected override void Rage ( )
    {
        isRage = true;
    }

    protected IEnumerator FlyOneShot (float time, bool isup = true)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            Fly(isup, 10);
            if (i > time * 0.67f)
                m_collider.enabled = true;
            yield return 0;
        }
        m_atAir = true;
    }

    protected void Fly (bool isup = true, float speed = 1)
    {
        if (isup)
            transform.position += upVec * Time.deltaTime * speed;
        else
            transform.position -= upVec * Time.deltaTime * speed;
    }

    protected void LookAtPlayer (float speed = 0.5f)
    {
        Vector3 dragonToPLayer = (m_playerTF.position - transform.position).normalized;
        // the cos
        float angle = Vector3.Dot(transform.forward, dragonToPLayer);
        // eulur angle
        angle = Mathf.Acos(angle) * 180 / Mathf.PI;
        if (angle > 2)
        {
            // player at right of the dragon
            if (Vector3.Dot(transform.right, dragonToPLayer) > 0)
            {
                transform.Rotate(0, speed * angle * Time.deltaTime, 0);
                if (Vector3.Dot(transform.right, dragonToPLayer) < 0)
                    m_fireOffset = 0;
                else
                    m_fireOffset = angle;
                //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, angle, 0), Time.deltaTime * 10);
                //if (m_fireOffset > 0 || (m_fireOffset == 0 && angle > 8))
                //    m_fireOffset = angle;
                //else
                //    m_fireOffset = 0;
            }
            else
            {
                transform.Rotate(0, -speed * angle * Time.deltaTime, 0);
                if (Vector3.Dot(transform.right, dragonToPLayer) > 0)
                    m_fireOffset = 0;
                else
                    m_fireOffset = -angle;
                //if (m_fireOffset < 0 || (m_fireOffset == 0 && angle > 8))
                //    m_fireOffset = angle;
                //else
                //    m_fireOffset = 0;
            }
        }
        else
        {
            m_fireOffset = 0;
        }
        //Debug.DrawRay(fireBirth.position, Quaternion.Euler(0, -m_fireOffset, 0) * transform.forward * 100, Color.red);
    }

    protected void MoveToPLayer (float speed = 8, float minDis = 10, float smooth = 10)
    {
        if (m_animator.GetFloat("dis") > minDis)
        {
            Vector3 dragonToPlayer = (m_playerTF.position - transform.position).normalized;
            Vector3 desPos = speed * dragonToPlayer * Time.deltaTime + transform.position;
            transform.position = Vector3.Lerp(transform.position, desPos, Time.deltaTime * smooth);
        }
    }

    protected float HeightFromTerrain (float maxDis = 100)
    {
        Ray ray = new Ray(fireBirth.position, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDis, LayerMask.NameToLayer("ground")))
        {
            return Vector3.Distance(fireBirth.position, hitInfo.point);
        }
        return -1;
    }

    protected float HeightFromPLayer ( )
    {
        return Vector3.Project(m_playerTF.position - fireBirth.position, Vector3.down).magnitude;
    }

    protected bool AtFront (Vector3 pos, float threshold = 0.9f)
    {
        return Vector3.Dot(transform.forward, pos - transform.position) > threshold;
    }


}
