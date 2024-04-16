using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueCacher : MonoBehaviour
{
    public static ValueCacher Instance { get; private set; } = null;


    private Canvas _currentMainCanvas;
    private RectTransform _currentMainCanvasRect;
    public Vector2 CurrentCanvasSize
    {
        get
        {
            if (_currentMainCanvas && _currentMainCanvasRect)
            {
                return new Vector2 (_currentMainCanvasRect.rect.width, _currentMainCanvasRect.rect.height);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("There is no current main canvas.");
#endif
                return Vector2.zero;
            }
        }
    }

    #region Camera
    private Camera _mainCamera;

    public Camera MainCamera { 
        get
        {
            if (!_mainCamera)
                _mainCamera = Camera.main;
            return _mainCamera;
        } 
    }

    public float HalfScreenSize { get { return MainCamera.orthographicSize * MainCamera.aspect; } }
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        _mainCamera = Camera.main;
    }

    public void SetCurrentCanvas(Canvas canvas)
    {
        _currentMainCanvas = canvas;
        _currentMainCanvasRect = canvas.GetComponent<RectTransform>();
    }
}
