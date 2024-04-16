using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_RandomForVideo : SpecialAbility
{
    private void ActivateRandomAbility()
    {
        int random = Random.Range(0, 4);
        switch (random)
        {
            case 0:
                SquareDataBase.Instance.SpecialAbilityBreakSides(2);
                break;
            case 1:
                SquareDataBase.Instance.SpecialAbilityMoveUp(2);
                break;
            case 2:
                BallController.main.SpecialAbilityGoldCannon(BallController.main.GetNotBonusNumberOfBalls());
                break;
            case 3:
                BallController.main.SpeciaAbilityShieldForBalls();
                break;
        }
    }

    public override bool BuyAndActivate()
    {
        // Buying an action instead of special ability,
        // because it is more in line with the rewarded ad.
        if (ProfileDataBase.main.BuyActionForRewardedVideo(ActivateRandomAbility) ==
            ProfileDataBase.PaymentResult.WATCHING_AD)
        {
            return true;
        }
        return false;
    }

    public override Currency GetCost()
    {
        return new Currency(Currency.Type.AD_VIDEO, 1);
    }
}