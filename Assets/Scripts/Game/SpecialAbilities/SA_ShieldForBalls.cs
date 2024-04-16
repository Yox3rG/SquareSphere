using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_ShieldForBalls : SpecialAbility
{
    public override bool BuyAndActivate()
    {
        if (ProfileDataBase.main.BuySpecialAbility(this) == ProfileDataBase.PaymentResult.SUCCESS)
        {
            BallController.main.SpeciaAbilityShieldForBalls();
            return true;
        }
        return false;
    }

    public override Currency GetCost()
    {
        return new Currency(Currency.Type.DIAMOND_APPLE, 100);
    }
}