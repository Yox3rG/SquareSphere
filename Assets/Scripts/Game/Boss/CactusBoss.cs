using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusBoss : Boss
{
    public CactusHeart Heart { get; set; }

    public override bool IsAffectedByPowepUp { get { return false; } }

    public override void CollidedWith(Ball ball)
    {
        //base.CollidedWith(ball);
        BallController.main.BallIsGone();
        ball.Break();
    }

    public override void TakeDmg(float dmg, bool isPowerUpDmg = false)
    {
        base.TakeDmg(dmg, isPowerUpDmg);
        if(!isPowerUpDmg)
            Heart.TakeDmgFromOwner(dmg);
    }

    public void TakeDmgFromHeart(float dmg)
    {
        base.TakeDmg(dmg);
    }
}
