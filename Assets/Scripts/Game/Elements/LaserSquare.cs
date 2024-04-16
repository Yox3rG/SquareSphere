using UnityEngine;



public class LaserSquare : Destroyable
{
    public SpriteRenderer horizontal;
    public SpriteRenderer vertical;
    public SpriteRenderer both;

    private SpriteRenderer current;

    bool didTriggerDeathEvent = false;

    public override void SetDefaultState(int hp, byte colorIndex)
    {
        SetOrientation();

        base.SetDefaultState(hp, colorIndex);
    }

    public override void SetColor(byte colorIndex)
    {
        base.SetColor(colorIndex);
        current.color = Palette.GetColor(colorIndex);
    }

    public void SetOrientation()
    {
        if(type == ElementType.LASER_HORIZONTAL)
        {
            current = horizontal;
            horizontal.enabled = true;
        }
        else if(type == ElementType.LASER_VERTICAL)
        {
            current = vertical;
            vertical.enabled = true;
        }
        else if(type == ElementType.LASER_BOTH)
        {
            current = both;
            both.enabled = true;
        }
    }

    public override void Break()
    {
        if (didTriggerDeathEvent)
            return;
        
        didTriggerDeathEvent = true;

        switch (type)
        {
            case ElementType.LASER_HORIZONTAL:
                SquareDataBase.Instance.SpecialElementLaserHorizontal(row);
                break;
            case ElementType.LASER_VERTICAL:
                SquareDataBase.Instance.SpecialElementLaserVertical(col);
                break;
            case ElementType.LASER_BOTH:
                SquareDataBase.Instance.SpecialElementLaserHorizontal(row);
                SquareDataBase.Instance.SpecialElementLaserVertical(col);
                break;
        }
        base.Break();
    }
}
