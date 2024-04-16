



public abstract class SpecialAbility : ISpecialAbility
{
    private static int nextID = 0;
    
    private int ID;
    private Currency cost;

    public SpecialAbility()
    {
        this.ID = nextID++;
    }

    public virtual bool BuyAndActivate()
    {
        throw new System.NotImplementedException();
    }

    public virtual Currency GetCost()
    {
        throw new System.NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as SpecialAbility);
    }

    public bool Equals(SpecialAbility specialAbility)
    {
        return specialAbility != null && specialAbility.ID == this.ID;
    }

    public override int GetHashCode()
    {
        return ID;
    }
}