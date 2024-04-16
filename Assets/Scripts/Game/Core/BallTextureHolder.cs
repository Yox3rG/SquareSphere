using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTextureHolder
{
    private static List<Sprite> ballSpriteSheetShop = new List<Sprite>();
    private static List<Sprite> ballSpriteSheetGame = new List<Sprite>();

    private static Sprite goldCoin;

    static BallTextureHolder()
    {
        LoadResources();
    }

    private static void LoadResources()
    {
        ballSpriteSheetShop = new List<Sprite>();
        ballSpriteSheetGame = new List<Sprite>();

        ballSpriteSheetShop.AddRange(Resources.LoadAll<Sprite>("shopBalls"));
        ballSpriteSheetGame.AddRange(Resources.LoadAll<Sprite>("gameBalls"));

        goldCoin = Resources.Load<Sprite>("gold");

        if(ballSpriteSheetGame.Count != ballSpriteSheetShop.Count)
        {
            Debug.Log("GameBall count and ShopBall count not the same!!!!!!!!");
        }
    }

    public static Sprite GetShopBallSprite(int index)
    {
        return ballSpriteSheetShop[index];
    }

    public static Sprite GetGameBallSprite(int index)
    {
        return ballSpriteSheetGame[index];
    }

    public static Sprite GetSelectedGameBallSprite()
    {
        int selectedBall = 0;
        if (ProfileDataBase.main)
        {
            selectedBall = ProfileDataBase.main.GetSelectedBallIndex();
        }

        return GetGameBallSprite(selectedBall);
    }

    public static Sprite GetGoldBall()
    {
        return goldCoin;
    }

    public static int GetBallSpriteCount()
    {
        return ballSpriteSheetShop.Count;
    }
}
