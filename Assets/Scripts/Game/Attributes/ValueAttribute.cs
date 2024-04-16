using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueAttribute : Attribute
{
    [SerializeField] protected float baseValue;

    protected Bonus bonus;

    private HashSet<ValueBuff> buffs;

    public float CurrentValue { get; protected set; }

    public ValueAttribute() : this(0) { }

    public ValueAttribute(float baseValue)
    {
        this.baseValue = baseValue;
        buffs = new HashSet<ValueBuff>();

        ClearBuffs();
    }

    public override bool ApplyBuff(IBuff buff)
    {
        if (!(buff is ValueBuff))
            return false;
        ValueBuff valueBuff = buff as ValueBuff;

        if (buffs.Contains(valueBuff))
            return false;

        buffs.Add(valueBuff);
        bonus += valueBuff.bonus;

        RecalculateCurrent();
        return true;
    }

    public override bool RemoveBuff(IBuff buff)
    {
        if (!(buff is ValueBuff))
            return false;
        ValueBuff valueBuff = buff as ValueBuff;

        if (!buffs.Contains(valueBuff))
            return false;

        buffs.Remove(valueBuff);
        bonus -= valueBuff.bonus;

        RecalculateCurrent();
        return true;
    }

    public override void ClearBuffs()
    {
        buffs.Clear();
        bonus.Clear();
        RecalculateCurrent();
    }

    public Bonus GetBonus()
    {
        return new Bonus(bonus);
    }

    protected virtual void RecalculateCurrent()
    {
        CurrentValue = bonus.GetResultWhenAppliedTo(this.baseValue);
    }
}

