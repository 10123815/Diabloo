using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using fuckRPGLib;

public class SkillManager : MonoBehaviour
{

    // skill effect prefab
    public GameObject shieldPrefab;

    // names of all skills
    // it must be same as the name of class because we use reflection to get it
    public string[] skillNames;

    // the index of active skill, now we have three
    public int[] active = { 0, 3, 6 };
    public float[] cds = { 5, 5, 5 };
    public float[] manaCosts = { 10, 10, 10 };
    public float[] duration = { 2, 0, 0 };
    public float[] last = { 4, 3, 3 };

    // only when it is 0, player can use the skill
    private float[] m_curCd;

    public delegate void LaunchDL ( );
    public delegate void LastingDL (float t);
    public delegate void StopDL ( );
    public LaunchDL[] launchFuncs;
    public LastingDL[] lastingFuncs;
    public StopDL[] stopFuncs;

    public void Start ( )
    {
        launchFuncs = new LaunchDL[active.Length];
        lastingFuncs = new LastingDL[active.Length];
        stopFuncs = new StopDL[active.Length];
        m_curCd = new float[active.Length];

        // the skills the players has learned!!!
        for (int i = 0; i < PlayerData.GetInstance().hasLearned.Length; i++)
        {
            if (PlayerData.GetInstance().hasLearned[i])
            {
                Debug.Log("learn " + i);
                Learn(i);
            }
        }
    }

    // learn a skill
    // this index is include all skill. it is from 0 to 6/7/8
    public void Learn (int skillIndex)
    {
        if (!PlayerData.GetInstance().hasLearned[skillIndex])
        {
            Debug.Log("This skill has not been learned!");
            return;
        }

        // it is a active skill, so at has Launch method
        for (int i = 0; i < active.Length; i++)
        {
            if (active[i] == skillIndex)
            {
                DoLearn(skillIndex, i);
                return;
            }
        }

        string[] str = skillNames[skillIndex].Split('_');

        // it is an unactive skill and have a lasting effect to enhance a active skill
        if (str.Length == 2 && str[1] == "Lasting")
        {
            int activeSkillIndex = GameCode.skillsAllClasses[(int)PlayerData.GetInstance().GetClassName()][skillIndex] / 10;
            DoLearn(skillIndex, activeSkillIndex);
            return;
        }

        // a normal unactive skill
        DoLearn(skillIndex);
    }

    private void DoLearn (int skillIndex, int activeSkillIndex = -1)
    {
        string name = skillNames[skillIndex];
        Debug.Log("PLayer learn " + name);
        Type t = null;
        try
        {
            t = Type.GetType(name);
            object skill = Activator.CreateInstance(t, new object[] { });
            LaunchDL launch = (LaunchDL)Delegate.CreateDelegate(typeof(LaunchDL), t, "OnLaunch");
            LastingDL last = (LastingDL)Delegate.CreateDelegate(typeof(LastingDL), t, "OnLasting");
            StopDL stop = (StopDL)Delegate.CreateDelegate(typeof(StopDL), t, "OnStop");
            // it is a active skill or a upgrading effect of a active skill
            if (activeSkillIndex != -1)
            {
                launchFuncs[activeSkillIndex] = launch;
                lastingFuncs[activeSkillIndex] = last;
                stopFuncs[activeSkillIndex] = stop;
            }
        }
        catch (ArgumentNullException)
        {
            Debug.LogError("We do not have this skill!");
        }
    }

    // this index is for the active skill, 0/1/2
    public void Skill (int i)
    {
        // this skill has not be learned
        if (launchFuncs[i] == null || lastingFuncs[i] == null)
            return;

        // the skill is in CD or player do not have enough mana
        if (m_curCd[i] > 0 || PlayerData.GetInstance().curMana < manaCosts[i])
            return;

        // instantaneous effect
        launchFuncs[i]();
        // lasting effect
        StartCoroutine(DoSkill(i));
        // stop skill
        StartCoroutine(DoStop(i));

        // cold down
        m_curCd[i] = cds[i];
        StartCoroutine(CD(i));

        // spend mana
        PlayerData.GetInstance().UseMana(manaCosts[i]);
    }

    private IEnumerator DoSkill (int i)
    {
        // duration for use this skill
        for (float t = 0; t < duration[i]; t += Time.deltaTime)
        {
            lastingFuncs[i](t / duration[i]);
            yield return 0;
        }
    }

    private IEnumerator DoStop (int i)
    {
        yield return new WaitForSeconds(last[i]);
        stopFuncs[i]();
    }

    private IEnumerator CD (int i)
    {
        while (m_curCd[i] > 0)
        {
            m_curCd[i] -= Time.deltaTime;
            PlayerData.GetInstance().gameUI.DisplayCD(i, m_curCd[i] / cds[i]);
            yield return 0;
        }
        m_curCd[i] = 0;
        PlayerData.GetInstance().gameUI.DisplayCD(i, 0);
    }


}
