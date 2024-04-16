using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_GoldCannon : SpecialAbility
{
    public override bool BuyAndActivate()
    {
        if (ProfileDataBase.main.BuySpecialAbility(this) == ProfileDataBase.PaymentResult.SUCCESS)
        {
            BallController.main.SpecialAbilityGoldCannon(
                BallController.main.GetNotBonusNumberOfBalls());
            return true;
        }
        else
        {
            return false;
        }
    }

    public override Currency GetCost()
    {
        Currency cost = new Currency(Currency.Type.GOLD, 
            BallController.main.GetNotBonusNumberOfBalls() * 2);

        return cost;
    }
}