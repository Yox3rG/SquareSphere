using System.CodeDom;
using UnityEngine;
using UnityEngine.UI;
using System;



public class BossFactory : MonoBehaviour
{
    public GameObject prefab_cubeKing;
    public GameObject prefab_cactus;
    public GameObject prefab_squid;

    public GameObject GetBoss(System.Type type)
    {
        GameObject g;
     
        if (type.IsAssignableFrom(typeof(CactusBoss)))
            g = Instantiate(prefab_cactus);
        else if (type.IsAssignableFrom(typeof(SquidBoss)))
            g = Instantiate(prefab_squid);
        else if (type.IsAssignableFrom(typeof(CubeKingBoss)))
            g = Instantiate(prefab_cubeKing);
        else
            g = Instantiate(prefab_cubeKing);

        Text text = TextGenerator.main.Generate(g.transform.position);
        g.GetComponent<Boss>().SetTextObject(text.GetComponent<Text>());
        g.transform.localScale *= ElementHandler.Instance.SquareScaleFactor;

        return g;
    }
}
