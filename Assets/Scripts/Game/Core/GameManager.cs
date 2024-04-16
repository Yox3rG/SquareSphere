using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    public SquareDataBase squareDatabase;
    public ScreenRelativeSizeCalculator screenRelativeSizeCalculator;

    public GameUIManager gameUIManager;
    public CameraBoundaries cameraBoundaries;
    public GamePlayMenu gamePlayMenu;

    private Camera mainCamera;

    public Camera MainCamera
    {
        get
        {
            if(!mainCamera)
            {
                mainCamera = Camera.main;
            }
            return mainCamera;
        }
    }

    void Awake()
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

    void Start()
    {
        gameUIManager.InitializeSizes();
        gamePlayMenu.Initialize();

        squareDatabase.InitializeSquareDataBase();

        cameraBoundaries.SetupBoundaries();
    }
}
