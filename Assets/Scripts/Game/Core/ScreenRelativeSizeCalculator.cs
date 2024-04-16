using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRelativeSizeCalculator : MonoBehaviour
{
    public static ScreenRelativeSizeCalculator Instance { get; private set; } = null;

    public static System.Action<float> OnCalculatingSquareSize;
    public static System.Action<Vector3> OnCalculatingSquareScale;


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

    #region Square
    public Vector3 GetTopLeftSquarePositionFromScreen(float squareSize, int maxRows)
    {
        float edgeOfAreaFromMiddle = squareSize * 5;

        float yStartPosition = 
            -GameManager.Instance.MainCamera.orthographicSize +
            squareSize * maxRows - squareSize / 2 +
            GameUIManager.Instance.bottomPanelSize;

        Vector3 startPosition = new Vector3(-edgeOfAreaFromMiddle + squareSize / 2, yStartPosition, 0f);
        return startPosition;
    }

    public float GetSquareSizeFromScreen(int numberOfSquares, out Vector3 newScale)
    {
        float squareSize = ElementHandler.Instance.DefaultSquareSize;
        float sideSize = ElementHandler.Instance.DefaultSideSize;

        float newSquareSize = ValueCacher.Instance.HalfScreenSize * 2 / numberOfSquares;
        OnCalculatingSquareSize?.Invoke(newSquareSize);

        float newScaleValue = (newSquareSize / squareSize);
        newScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);
        OnCalculatingSquareScale?.Invoke(newScale);

        return newSquareSize;
    }
    #endregion

    #region Ball
    public float GetBallHeight()
    {
        return 
            -GameManager.Instance.MainCamera.orthographicSize +
            GameUIManager.Instance.bottomPanelSize +
            ElementHandler.Instance.SquareSize / 2;
    }

    public float GetBasketHeight()
    {
        return GameUIManager.Instance.bottomPanelSize;
    }

    public float GetBasketYPosition()
    {
        return GameUIManager.Instance.bottomPanelGamePosition.y;
    }
    #endregion
}
