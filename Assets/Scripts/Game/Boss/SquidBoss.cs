using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBoss : Boss
{
    public override bool IsAffectedByPowepUp { get { return false; } }

    protected override void OnEnable()
    {
        base.OnEnable();
        Goo.OnNewGooAttached += OnNewGooAttached;
        Goo.OnGooDestroyed += OnGooDestroyed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Goo.OnNewGooAttached -= OnNewGooAttached;
        Goo.OnGooDestroyed -= OnGooDestroyed;
    }

    public override void CollidedWith(Ball ball)
    {
        //base.CollidedWith(ball);
    }

    private void OnNewGooAttached()
    {
        TakeDmg(-1);
    }

    private void OnGooDestroyed()
    {
        TakeDmg(1);
    }
}
