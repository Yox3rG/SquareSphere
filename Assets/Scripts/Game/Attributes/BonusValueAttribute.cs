using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusValueAttribute : ValueAttribute
{
    public BonusValueAttribute() : this(0) { }

    public BonusValueAttribute(float baseValue) : base(baseValue) { }

    public override void ClearBuffs()
    {
        base.ClearBuffs();

        bonus += new Bonus(0, baseValue);
        RecalculateCurrent();
    }

    protected override void RecalculateCurrent()
    {
        CurrentValue = bonus.GetResultWhenAppliedTo(1);
    }
}
