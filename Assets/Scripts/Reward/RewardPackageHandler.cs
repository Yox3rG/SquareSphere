using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPackageHandler : MonoBehaviour
{
    public static Sprite Peek(RewardPackage package, out string str_amount, int index = 0)
    {
        str_amount = "";
        if (package == null)
            return null;

        Reward reward = package[index];
        if (reward == null)
            return null;

        if (reward is CurrencyReward)
        {
            str_amount = ((CurrencyReward)reward).reward.amount.ToString();
        }
        else if (reward is BallReward)
        {
            //str_amount = ((BallReward)reward).reward.index.ToString();
        }

        return reward.GetIcon();
    }

    public static bool Open(RewardPackage package)
    {
        if (package == null)
            return false;

        for (int i = 0; i < package.Length; i++)
        {
            Open(package[i]);
        }

        return true;
    }

    private static bool Open(Reward reward)
    {
        if (reward == null)
            return false;

        if(reward is CurrencyReward)
        {
            ProfileDataBase.main.GainCurrency(((CurrencyReward)reward).reward);
        }
        else if (reward is BallReward)
        {
            ProfileDataBase.main.UnlockBall(((BallReward)reward).reward);
        }

        return false;
    }
}
