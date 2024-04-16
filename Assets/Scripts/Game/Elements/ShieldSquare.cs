


using UnityEngine;

public class ShieldSquare : Destroyable
{
    public SpriteRenderer shieldRenderer;

    bool canBeDamaged = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        SquareDataBase.OnEndOfRound += EndRoundAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SquareDataBase.OnEndOfRound -= EndRoundAction;
    }

    public override void SetDefaultState(int hp, byte colorIndex)
    {
        if (type == ElementType.SHIELD_EVEN)
        {
            shieldRenderer.enabled = true;
            canBeDamaged = false;
        }
        else if (type == ElementType.SHIELD_ODD)
        {
            shieldRenderer.enabled = false;
            canBeDamaged = true;
        }

        base.SetDefaultState(hp, colorIndex);
        SetTextColor(dark: canBeDamaged);
    }

    public void EndRoundAction()
    {
        shieldRenderer.enabled = !shieldRenderer.enabled;
        canBeDamaged = !canBeDamaged;
        SetTextColor(dark: canBeDamaged);
    }

    public override void SetColor(byte colorIndex)
    {
        base.SetColor(colorIndex);
    }

    public override void TakeDmg(float dmg, bool isPowerUpDmg = false)
    {
        if (canBeDamaged)
            base.TakeDmg(dmg, isPowerUpDmg);
    }
}
