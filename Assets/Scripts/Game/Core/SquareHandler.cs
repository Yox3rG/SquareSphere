using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SquareHandlerB : MonoBehaviour
{
    /*
    public GameObject squarePrefab;
    public GameObject trianglePrefab;


    public float squareSize { get; protected set; } = .55f;
    protected float sideSize = .125f / 2;

    private int currentId = 0; // Used in GenerateBlock.


    void Awake()
    {
        squareSize = squarePrefab.GetComponent<BoxCollider2D>().size.x;
        currentId = 0;
    }

    protected GameObject GenerateSquare(Vector3 position)
    {
        GameObject g = GeneratePrefabWithHP(squarePrefab, position);

        return g;
    }

    protected GameObject GenerateTriangle(Vector3 position)
    {
        int orientation = Random.Range(0, 4);
        GameObject g = GenerateTriangle(position, orientation);

        return g;
    }

    protected GameObject GenerateTriangle(Vector3 position, int orientation)
    {
        GameObject g = GeneratePrefabWithHP(trianglePrefab, position);

        Triangle t = g.GetComponent<Triangle>();
        t.SetOrientationAndOffset(orientation);
        t.ResetTextPosition();

        return g;
    }

    protected GameObject GeneratePrefabWithHP(GameObject prefab, Vector3 position)
    {
        GameObject g = Instantiate(prefab, position, Quaternion.identity);
        g.name = prefab.name + "_" + currentId++;

        Text text = TextGenerator.main.Generate(g.transform.position);
        g.GetComponent<Square>().SetTextObject(text.GetComponent<Text>());

        return g;
    }

    protected GameObject GenerateSquareFromBlock(Block block, Vector3 position)
    {
        GameObject g = null;
        if (block.isSquare)
        {
            switch (block.squareType)
            {
                case Square.Type.SQUARE:
                    g = GenerateSquare(position);
                    break;
                case Square.Type.TRIANGLE_0:
                    g = GenerateTriangle(position, 0);
                    break;
                case Square.Type.TRIANGLE_90:
                    g = GenerateTriangle(position, 1);
                    break;
                case Square.Type.TRIANGLE_180:
                    g = GenerateTriangle(position, 2);
                    break;
                case Square.Type.TRIANGLE_270:
                    g = GenerateTriangle(position, 3);
                    break;
                default:
                    break;
            }

            if (g != null)
            {
                Square s = g.GetComponent<Square>();

                s.SetMaxHp(block.maxHp);
                s.ResetTextPosition();
            }
        }

        return g;
    }

    protected GameObject GeneratePowerUpFromBlock(Block block, Vector3 position, int row)
    {
        GameObject g = null;
        if (!block.isSquare)
        {
            g = PowerUpDataBase.main.GeneratePowerUp(position, new Vector2Int(row, block.col), block.powerType);
        }
        return g;
    }
    */ 
}
