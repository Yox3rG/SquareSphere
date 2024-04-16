using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; } = null;

    public GameObject topPanel;
    public GameObject topPanelBottom;

    public GameObject bottomPanel;
    public GameObject bottomPanelTop;


    public float topPanelSize;
    public Vector3 topPanelGamePosition;

    public float bottomPanelSize;
    public Vector3 bottomPanelGamePosition;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [ContextMenu("InitializeAgain")]
    public void InitializeSizes()
    {
        Camera cam = GameManager.Instance.MainCamera;
        float sizeOfScreen = cam.orthographicSize;

        topPanelSize = Mathf.Abs(topPanel.transform.position.y - topPanelBottom.transform.position.y);
        topPanelGamePosition = new Vector3(0f, sizeOfScreen - topPanelSize / 2);

        bottomPanelSize = Mathf.Abs(bottomPanel.transform.position.y - bottomPanelTop.transform.position.y);
        bottomPanelGamePosition = new Vector3(0f, -sizeOfScreen + bottomPanelSize / 2);
    }
}
