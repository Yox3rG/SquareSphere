


public class CactusHeart : Destroyable
{
    private CactusBoss _owner;

    public CactusBoss Owner { get { return _owner; } set
        {
            if (value == null)
            {
                Destroy(gameObject);
                return;
            }

            _owner = value;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SquareDataBase.OnEndOfRound += ChangePosition;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SquareDataBase.OnEndOfRound -= ChangePosition;
    }

    public override void TakeDmg(float dmg, bool isPowerUpDmg = false)
    {
        base.TakeDmg(dmg, isPowerUpDmg);
        Owner.TakeDmgFromHeart(dmg);
    }

    public void TakeDmgFromOwner(float dmg)
    {
        base.TakeDmg(dmg);
    }

    private void ChangePosition()
    {
        if (SquareDataBase.Instance != null)
            SquareDataBase.Instance.SwapArrayElementWithRandom(this);
    }
}