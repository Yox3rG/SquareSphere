using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : Element
{
    private static List<Sprite> powerUpSpriteSheet = new List<Sprite>();

    public bool isDestroyedNextTurn { get; private set; } = false;


    void Awake()
    {
        if(powerUpSpriteSheet.Count == 0)
        {
            powerUpSpriteSheet.AddRange(Resources.LoadAll<Sprite>("powerUp"));
        }
    }

    /*
    public void LiftDown(bool value)
    {
        Vector3 temp;
        if (value)
        {
            temp = Vector3.down;
        }
        else
        {
            temp = Vector3.up;
        }

        row -= (int)temp.y;
        transform.position += temp * ElementHandler.main.squareSize;
    }

    public void SetRowAndCol(Vector2Int intPosition)
    {
        row = intPosition.x;
        col = intPosition.y;
    }
    */

    public override void SetType(ElementType type)
    {
        if(!IsPowerUp(type))
        {
            type = ElementType.POWERUP_BONUSBALL;
        }
        this.type = type;
        name = "powerup_" + ((int)type - powerupStart);
        GetComponent<SpriteRenderer>().sprite = Icon((int)type - powerupStart);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball;
        if (collision.gameObject.layer == 10)
        {
            switch (type)
            {
                case Element.ElementType.POWERUP_BONUSBALL:
                    BonusBall();
                    Destroy(gameObject);
                    break;
                case Element.ElementType.POWERUP_HORIZONTAL_LINE:
                    HorizontalPowerUp();
                    break;
                case Element.ElementType.POWERUP_VERTICAL_LINE:
                    VerticalPowerUp();
                    break;
                case Element.ElementType.POWERUP_BOTH_LINE:
                    HorizontalPowerUp();
                    VerticalPowerUp();
                    break;
                case Element.ElementType.POWERUP_DINO:
                    ball = collision.GetComponent<Ball>();
                    if (ball)
                    {
                        Dino(ball);
                    }
                    break;
                case Element.ElementType.POWERUP_SHIELD:
                    ball = collision.GetComponent<Ball>();
                    if (ball)
                    {
                        ball.IsShielded = true;
                    }
                    break;
                default:
                    break;
            }

            isDestroyedNextTurn = true;
        }
    }

    private void BonusBall()
    {
        BallController.main.BonusBallPowerUp();
    }

    private void HorizontalPowerUp()
    {
        SquareDataBase.Instance.DamageRow(row, Attribute.Type.PU_DAMAGE_MINILASER);
    }

    private void VerticalPowerUp()
    {
        SquareDataBase.Instance.DamageCol(col, Attribute.Type.PU_DAMAGE_MINILASER);
    }

    private void Dino(Ball ball)
    {
        Vector3 temp = new Vector3(Random.Range(-1f, 1f), Random.Range(.3f, 1f), 0);

        ball.SetVelocity(temp.normalized);
    }

    private Sprite Icon(int index)
    {
        if (index < 0 || index >= powerUpSpriteSheet.Count)
        {
#if UNITY_EDITOR
            Debug.Log("A powerup with this index [" + index + "] has no icon!");
#endif
            return null;
        }
        return powerUpSpriteSheet[index];
    }
}
