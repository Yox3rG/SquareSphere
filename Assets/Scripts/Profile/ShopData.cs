using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopData
{
    public List<bool> ballsUnlocked;

    // Split at every 5th. Not Serialized...
    private static List<Currency> _ballCostList;
    private static List<Currency> BallCostList { get
        {
            if(_ballCostList == null)
            {
                _ballCostList = new List<Currency>() {
                    new Currency(Currency.Type.GOLD, 10),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.DIAMOND_APPLE, 10),

                    new Currency(Currency.Type.GOLD, 10),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.DIAMOND_APPLE, 10),
                
                    new Currency(Currency.Type.GOLD, 10),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.GOLD, 10),

                    new Currency(Currency.Type.GOLD, 10),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.DIAMOND_APPLE, 10),

                    new Currency(Currency.Type.GOLD, 10),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.GOLD, 10),
                    // 5 ^
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),
                    
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.GOLD, 100),
                    
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),
                    
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.AD_VIDEO, 1),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),

                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),
                    // 10 ^
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),

                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),

                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.GOLD, 100),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),

                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),

                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),
                    // 15 ^
                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.DIAMOND_APPLE, 100),

                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.GOLD, 200),
                    new Currency(Currency.Type.DIAMOND_APPLE, 200) };
            }
            return _ballCostList;
        }
        set
        {
            
        }
    }
    

    public int selectedBall;

    public ShopData()
    {
        ballsUnlocked = new List<bool>();
        selectedBall = 0;
    }

    public int UnlockableBallListCount()
    {
        return ballsUnlocked.Count;
    }

    public int BallCostListCount()
    {
        return BallCostList.Count;
    }

    public Currency GetBallCost(int index)
    {
        if (index >= BallCostList.Count)
            return new Currency(Currency.Type.DIAMOND_APPLE, 6969);
        return BallCostList[index];
    }

    public bool IsBallUnLocked(int index)
    {
        if (index >= ballsUnlocked.Count)
            return false;
        return ballsUnlocked[index];
    }

    public List<int> GetLockedBallIndexes()
    {
        List<int> lockedBalls = new List<int>();

        for (int i = 0; i < UnlockableBallListCount(); i++)
        {
            if (ballsUnlocked[i])
                continue;

            lockedBalls.Add(i);
        }

        return lockedBalls;
    }

    public int UnlockedBallCount()
    {
        int count = 0;
        foreach(bool b in ballsUnlocked)
        {
            if (b)
            {
                count++;
            }
        }

        return count;
    }

    public void UpdateCountOfUnlockedBalls()
    {
        while(BallTextureHolder.GetBallSpriteCount() > ballsUnlocked.Count)
        {
            ballsUnlocked.Add(false);
        }
    }
}
