using UnityEngine;

public class GooFactory : MonoBehaviour
{
    public static GooFactory Instance;

    public GameObject gooPrefab;

    public Sprite[] levels;
    public Sprite[] triangleLevels;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public Goo GetGoo()
    {
        GameObject goo = Instantiate<GameObject>(gooPrefab);

        return goo.GetComponent<Goo>();
    }

    public Sprite GetGooSprite(int level)
    {
        return levels[level - 1];
    }

    public Sprite GetTriangleGooSprite(int level)
    {
        return triangleLevels[level - 1];
    }
}