using UnityEngine;
using System.Collections;

public class Skill
{

    static protected Transform m_playerTF;

    public Skill ( )
    {
        m_playerTF = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // the effect to player when this skill learned
    protected virtual void OnAddingEffect ( )
    {

    }

    // undo the effect when player remove this skill
    public virtual void OnRemovingEffect ( )
    {

    }

    static public void OnLaunch ( )
    {
    }

    static public void OnLasting (float t)
    {
    }

    static public void OnStop ( )
    {
    }

}
