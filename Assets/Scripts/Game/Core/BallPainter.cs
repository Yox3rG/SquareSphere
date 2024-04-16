using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPainter
{
    public enum BallPrefabType
    {
        DEFAULT,
        SELECTED,
        GOLD
    }

    private Dictionary<BallPrefabType, GameObject> prefabs;

    private BallPrefabType currentType;

    private int shotsUntilGold = 0;

    public BallPainter(GameObject prefab)
    {
        prefabs = new Dictionary<BallPrefabType, GameObject>();

        prefabs.Add(BallPrefabType.DEFAULT, GameObject.Instantiate(prefab));
        prefabs.Add(BallPrefabType.SELECTED, GameObject.Instantiate(prefab));
        prefabs.Add(BallPrefabType.GOLD, GameObject.Instantiate(prefab));

        prefabs[BallPrefabType.DEFAULT] .SetActive(false);
        prefabs[BallPrefabType.SELECTED].SetActive(false);
        prefabs[BallPrefabType.GOLD]    .SetActive(false);

        prefabs[BallPrefabType.DEFAULT].name = "DefaultBall";
        prefabs[BallPrefabType.SELECTED].name = "SelectedBall";
        prefabs[BallPrefabType.GOLD].name = "GoldBall";

        prefabs[BallPrefabType.DEFAULT].GetComponent<SpriteRenderer>().sprite = 
            BallTextureHolder.GetGameBallSprite(0);
        prefabs[BallPrefabType.SELECTED].GetComponent<SpriteRenderer>().sprite = 
            BallTextureHolder.GetSelectedGameBallSprite();
        prefabs[BallPrefabType.GOLD].GetComponent<SpriteRenderer>().sprite = 
            BallTextureHolder.GetGoldBall();

        currentType = BallPrefabType.SELECTED;
    }

    public void BallShot()
    {
        if(shotsUntilGold > 0)
        {
            if(--shotsUntilGold == 0)
            {
                currentType = BallPrefabType.GOLD;
            }
        }
    }

    public void Reset()
    {
        shotsUntilGold = 0;
        currentType = BallPrefabType.SELECTED;
    }

    public void ShootGoldAfter(int shots)
    {
        shotsUntilGold = shots;
    }

    public GameObject GetCurrentPrefab()
    {
        return prefabs[currentType];
    }
}
