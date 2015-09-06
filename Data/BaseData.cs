using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseData : MonoBehaviour
{

    // false when stun, knocked up, knocked back...
    [System.NonSerialized]
    public bool canMove = true;

    [System.NonSerialized]
    public bool hasDead = false;

    // the gold a hero will gain when he or she kill this character
    public int valueExp;
    public int valueGold;

    // some general attributes
    public float curLife = 200;
    public float maxLife = 200;
    public float curMana = 0;
    public float maxMana = 0;
    public float atk = 20;
    public float maxAtk = 500;
    public float def = 5;
    public float maxDef = 500;
    public float crit = 20;
    public float critRate = 0;
    public float hitRate = 1;
    public float atkSpeed = 1;
    public float moveSpeed = 3;
    public float atkRange = 1;
    public float dodgeRate = 0;

    // Use this for initialization
    void Awake ( )
    {

    }

    // Update is called once per frame
    void Update ( )
    {
        BaseUpdate();
    }

    protected void BaseUpdate ( )
    {

    }

    #region life
    private void LifeChange (float value)
    {
        curLife += value;
        curLife = (curLife > maxLife) ? maxLife : curLife;
        if (curLife <= 0)
        {
            curLife = 0;
            hasDead = true;
        }
    }

    virtual public void Damaged (float d)
    {
        d = d * (1 - def / maxDef);
        LifeChange(-d);
    }

    public void Damaged (float d, float apr)
    {
        float df = def - apr;
        df = (df < 0) ? 0 : df;
        d = d * (1 - df / maxDef);
        LifeChange(-d);
    }

    #endregion

    virtual public void Cured (float _l, float _r)
    {

        LifeChange(_l);
        ManaChange(_r);
    }

    #region mana
    private void ManaChange (float value)
    {
        curMana += value;
        curMana = (curMana > maxMana) ? maxMana : curMana;
        curMana = (curMana < 0) ? 0 : curMana;
    }

    virtual public void UseMana (float value)
    {
        ManaChange(-value);
    }
    #endregion

}
