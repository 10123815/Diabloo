using UnityEngine;
using System.Collections;

public class NashorAction : BossAction
{

    public float argular = 1000;

    // atk effect
    // acid orbs
    public GameObject acidOrbPrefab;
    public Transform acidOrbBirth;
    // green down orbs
    public GameObject greenOrbPrefab;
    public GameObject greenCheckPrefab;
    private GameObject[] m_greenChecks = new GameObject[3];
    private Vector3[] m_greenPositions = new Vector3[3];
    private int m_greenCount = 0;
    public Transform greenOrbBirth;
    public float maxDis = 12;
    // sweep attack
    public GameObject sweep;
    private BoxCollider m_sweepBox;

    // Use this for initialization
    void Start ( )
    {
        m_data = GetComponent<NashorData>();
        base.Start();

        acidOrbPrefab.GetComponent<RayShoot>().dmg = m_data.atk;
        m_sweepBox = sweep.GetComponent<BoxCollider>();
    }

    protected override void Init ( )
    {
        enemyId = fuckRPGLib.GameCode.EnemyID.Nashor;
        base.Init();
    }

    protected override void Animate ( )
    {
        if (m_anmSttInfo.IsName("0.idle"))
        {
            if (!m_animator.GetBool("atk") && Time.frameCount % 20 == 0)
                m_animator.SetBool("atk", Random.value < 0.1f);

            if (m_animator.GetBool("atk") && m_animator.GetFloat("dis") < 6)
            {
                if (!m_animator.GetBool("1") && !m_animator.GetBool("2") && Time.frameCount % 21 == 0)
                {
                    m_animator.SetBool("1", Random.value < 0.5f);
                    m_animator.SetBool("2", !m_animator.GetBool("1"));
                }
            }
        }
        else if (m_anmSttInfo.IsName("0.atk"))
        {
            if (m_anmSttInfo.normalizedTime < .6f)
            {
                RotateToPlayer();
            }
            if (m_anmSttInfo.normalizedTime > .5f && m_anmSttInfo.normalizedTime < .6f && m_animator.GetBool("atk"))
            {
                m_animator.SetBool("atk", false);
                Vector3 targetPosition = transform.forward * m_animator.GetFloat("dis") + transform.position + new Vector3(0, 1.5f, 0);
                PoolManager.GetInstance().GetPool(acidOrbPrefab).GetObject(acidOrbBirth.position, (targetPosition - acidOrbBirth.position).normalized);
            }
        }
        else if (m_anmSttInfo.IsName("0.1"))
        {
            // confirm three target positions
            ConfirmGreenPositions(m_anmSttInfo.normalizedTime);

            // shoot
            if (m_anmSttInfo.normalizedTime > .8f && m_anmSttInfo.normalizedTime < .9f && m_animator.GetBool("atk") && m_animator.GetBool("1"))
            {
                m_animator.SetBool("atk", false);
                m_animator.SetBool("1", false);
                m_greenCount = 0;
                StartCoroutine(Shoot());
            }
        }
        else if (m_anmSttInfo.IsName("0.2"))
        {
            if (m_anmSttInfo.normalizedTime > .5f && m_anmSttInfo.normalizedTime < .7f && m_animator.GetBool("atk") && m_animator.GetBool("2"))
            {
                m_animator.SetBool("atk", false);
                m_sweepBox.enabled = true;
            }
            if (m_anmSttInfo.normalizedTime > .7f && m_animator.GetBool("2"))
            {
                m_animator.SetBool("2", false);
                m_sweepBox.enabled = false;
            }
        }

    }

    private void ConfirmGreenPositions (float time)
    {
        if (time > 0.35f && time < 0.4f && m_greenCount == 0)
        {
            ConfirmGreenPosition(time, 0.35f, 0.4f, 0);
        }
        else if (time > 0.55f && time < 0.6f && m_greenCount == 1)
        {
            ConfirmGreenPosition(time, 0.55f, 0.6f, 1);
        }
        else if (time > 0.75f && time < 0.8f && m_greenCount == 2)
        {
            ConfirmGreenPosition(time, 0.75f, 0.8f, 2);
        }
    }

    private void ConfirmGreenPosition (float time, Vector2 range, int index)
    {
        if (time > range.x && time < range.y && m_greenCount == index)
        {
            m_greenCount++;
            if (m_animator.GetFloat("dis") < maxDis)
            {
                m_greenPositions[index] = player.transform.position + new Vector3(0, 0.5f, 0);
                m_greenChecks[index] = PoolManager.GetInstance().GetPool(greenCheckPrefab).GetObject(m_greenPositions[index]);
            }
        }
    }

    private void ConfirmGreenPosition (float time, float start, float end, int index)
    {
        ConfirmGreenPosition(time, new Vector2(start, end), index);
    }

    private IEnumerator Shoot ( )
    {
        ShootOneGreen(0);
        yield return new WaitForSeconds(0.5f);
        ShootOneGreen(1);
        yield return new WaitForSeconds(0.5f);
        ShootOneGreen(2);

    }

    private void ShootOneGreen (int index)
    {
        GameObject greenOrb = PoolManager.GetInstance().GetPool(greenOrbPrefab).GetObject(greenOrbBirth.position, (m_greenPositions[index] - greenOrbBirth.position).normalized);
        greenOrb.GetComponent<DownShoot>().check = m_greenChecks[index];
    }

    private void RotateToPlayer ( )
    {
        Vector3 fw = transform.forward;
        fw.y = 0;
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0;
        float angle = Vector3.Angle(fw, toPlayer);
        Vector3 right = transform.right;
        right.y = 0;
        if (Vector3.Dot(right, toPlayer) > 0)
            //transform.Rotate(0, angle * Time.deltaTime * argular, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y + angle, 0), Time.deltaTime * argular);
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y + -angle, 0), Time.deltaTime * argular);
    }


    protected override void Rage ( )
    {
        isRage = true;
    }
}
