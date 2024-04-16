using UnityEngine;


[CreateAssetMenu(fileName = "rwd_Currency", menuName = "ScriptableObjects/Reward/Currency", order = 101)]
public class CurrencyReward : Reward
{
    public Currency reward;

    public override Type GetRewardType()
    {
        return Type.CURRENCY;
    }

    public override string GetTypeName()
    {
        return "Currency";
    }

    public override Sprite GetIcon()
    {
        return reward.Icon;
    }

    public override string ToString()
    {
        return reward.ToString();
    }
}
