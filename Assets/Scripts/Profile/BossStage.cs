


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossStage : SeededStage
{
    public System.Type BossType { get; private set; }

    public BossStage(int stageSeed, System.Type bossType)
        : base(LayoutType.BOSS, stageSeed)
    {
        SetBoss(bossType);
    }

    public BossStage(LayoutType _type, int stageSeed, System.Type bossType)
        : this(stageSeed, bossType) { }

    public override TopMostRow GetTopMostRows()
    {
        return BossTypes.GetTopMostRow(BossType);
    }

    private void SetBoss(System.Type type)
    {
        if (type.IsSubclassOf(typeof(Boss)))
            BossType = type;
        else
            BossType = BossTypes.GetDefaultBossType();
    }
}