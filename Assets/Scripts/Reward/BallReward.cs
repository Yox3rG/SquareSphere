using UnityEngine;

[CreateAssetMenu(fileName = "rwd_Ball", menuName = "ScriptableObjects/Reward/Ball", order = 100)]
public class BallReward : Reward
{
    public UnlockBall reward;

    public override Type GetRewardType()
    {
        return Type.BALL;
    }

    public override string GetTypeName()
    {
        return "Ball";
    }

    public override string ToString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (reward.isRandom)
        {
            sb.Append("Random ");
        }
        else
        {
            sb  .Append('G').Append(reward.generation)
                .Append('N').Append(reward.index).Append(' ');
        }
        sb.Append("[Ball]");
        return sb.ToString();
    }
}
