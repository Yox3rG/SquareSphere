using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBoundaries : MonoBehaviour
{
    public static CameraBoundaries Instance { get; private set; }

    public LayerMask boundaryLayer;

    private GameObject[] bounds;

    private Vector2[] middlePoints;

    private enum Side : int { LEFT = 0, TOP, RIGHT, BOTTOM };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetupBoundaries()
    {
        bounds = new GameObject[4];
        middlePoints = new Vector2[4];

        Camera cam = GameManager.Instance.MainCamera;
        float top = cam.orthographicSize;
        float right = cam.orthographicSize * cam.aspect;

        int negativeSide = -1;

        for (int i = 0; i < 4; i++)
        {
            bounds[i] = new GameObject("Boundary " + System.Enum.GetName(typeof(Side), i));
            bounds[i].layer = (int) Mathf.Log(boundaryLayer.value, 2); // 12;
            BoxCollider2D boxCol = bounds[i].AddComponent<BoxCollider2D>();

            if (i % 2 == 0)
            {
                boxCol.size = new Vector2(1, top * 2);
                boxCol.transform.position = new Vector3(right + .5f, 0) * negativeSide;

                middlePoints[i] = new Vector2(right * negativeSide, 0);

                negativeSide *= -1;
            }
            else
            {
                boxCol.size = new Vector2(right * 2, 1);
                boxCol.transform.position = new Vector3(0, top + .5f) * negativeSide;

                middlePoints[i] = new Vector2(0, top * negativeSide);
            }
        }

        AlignTopToUI();
        //AlignBottomToUI();
    }

    private void AlignTopToUI()
    {
        Camera cam = GameManager.Instance.MainCamera;
        float playAreaHeight = ElementHandler.Instance.SquareSize * SquareDataBase.rows;
        float inverseSizeOfTopPanel = GameUIManager.Instance.bottomPanelSize;
        float sizeOfPanel = cam.orthographicSize * 2 - (playAreaHeight + inverseSizeOfTopPanel);

        GameObject topBoundGameObject = bounds[(int)Side.TOP];

        BoxCollider2D bc = topBoundGameObject.GetComponent<BoxCollider2D>();
        bc.size = new Vector2(bc.size.x, sizeOfPanel);

        middlePoints[(int)Side.BOTTOM].y = cam.orthographicSize - sizeOfPanel / 2;
        Transform t = topBoundGameObject.transform;
        t.position = middlePoints[(int)Side.BOTTOM];

        //sizeOfPanel -= topPanel.GetComponent<RectTransform>().position.y;
        //sizeOfPanel *= 2;

        //middlePoints[(int)Side.TOP].y -= sizeOfPanel;

        //BoxCollider2D bc = bounds[(int)Side.TOP].GetComponent<BoxCollider2D>();
        //bc.size = new Vector2(bc.size.x, sizeOfPanel);

        //Transform t = bounds[(int)Side.TOP].transform;
        //t.position = topPanel.GetComponent<RectTransform>().position;
    }

    private void AlignBottomToUI()
    {
        GameObject bottomBoundGameObject = bounds[(int)Side.BOTTOM];

        middlePoints[(int)Side.BOTTOM].y = GameUIManager.Instance.bottomPanelGamePosition.y;

        BoxCollider2D bc = bottomBoundGameObject.GetComponent<BoxCollider2D>();
        bc.size = new Vector2(bc.size.x, GameUIManager.Instance.bottomPanelSize);

        Transform t = bottomBoundGameObject.transform;
        t.position = middlePoints[(int)Side.BOTTOM];
    }

    public Vector2[] GetScreenMiddlePoints()
    {
        return middlePoints;
    }
}
