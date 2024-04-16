using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement main { get; private set; } = null;

    private bool isMovingCamera = false;
    private Vector2? touchOrigin = null;
    private Vector3? cameraOrigin = null;
    /*
    [SerializeField]
    private float speed = 10f;
    */

    void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }
    
    void Update()
    {
        if (isMovingCamera)
        {
            MoveCameraInverted(false);
        }
    }

    private void MoveCameraInverted(bool invert)
    {
        int invertSign = -1;
        if (!invert)
        {
            invertSign = 1;
        }

        if (Input.touchCount > 0)
        {
            Touch firstTouch = Input.GetTouch(0);

            if (firstTouch.phase == TouchPhase.Began)
            {
                touchOrigin = firstTouch.position;
                cameraOrigin = GetComponent<Camera>().transform.position;
            }
            else if (firstTouch.phase == TouchPhase.Moved)
            {
                if (touchOrigin.HasValue && cameraOrigin.HasValue)
                {
                    Camera cam = GetComponent<Camera>();

                    Vector3 worldOrigin = cam.ScreenToWorldPoint(touchOrigin.Value);
                    Vector3 worldTouchPosition = cam.ScreenToWorldPoint(firstTouch.position);

                    float differenceY = worldOrigin.y - worldTouchPosition.y;
                    cam.transform.position = cameraOrigin.Value + Vector3.up * differenceY * invertSign;

                    // Only call to other classes.
                    MapGenerator.main.ResetAllTextOnElements();
                }
            }
            else if(firstTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled)
            {
                touchOrigin = null;
                cameraOrigin = null;
            }
        }
    }

    public void InvertCameraMovementTrigger()
    {
        isMovingCamera = !isMovingCamera;
    }
}
