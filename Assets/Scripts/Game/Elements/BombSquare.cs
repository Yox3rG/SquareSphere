


using UnityEngine;
public class BombSquare : Destroyable
{
    bool didTriggerDeathEvent = false;

    public override void SetDefaultState(int hp, byte colorIndex)
    {
        base.SetDefaultState(hp, colorIndex);
        SetTextColor(dark: false);
    }

    public override void SetColor(byte colorIndex)
    {
        base.SetColor(colorIndex);
    }

    public override void Break()
    {
        if (didTriggerDeathEvent)
            return;

        didTriggerDeathEvent = true;
        SquareDataBase.Instance.SpecialElementBomb(row, col);
        base.Break();
    }
}
