using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBasket : MonoBehaviour
{
    public static BallBasket main { get; private set; } = null;

    private Vector3 startPosition;
    private float colliderHeight = 1;

    private bool first = false;

    void Start()
    {
        if (main == null)
        {
            main = this;
        }
        Initialize();
    }

    private void Initialize()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(Camera.main.orthographicSize * Camera.main.aspect * 2, ScreenRelativeSizeCalculator.Instance.GetBasketHeight());
        startPosition = new Vector3(0, ScreenRelativeSizeCalculator.Instance.GetBasketYPosition(), 0);
        transform.position = startPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Ball ball = collision.transform.GetComponent<Ball>();

            if (ball.IsShielded)
            {
                ball.IsShielded = false;
            }
            else
            {
                if (first)
                {
                    BallController.main.FirstBallFell(ball.transform.position);
                    first = false;
                }

                ball.Return();
                BallController.main.BallIsGone();
            }
        }
    }

    public void GameIsRolling()
    {
        first = true;
    }

    public void GameStoppedRolling()
    {
        first = false;
    }
}
