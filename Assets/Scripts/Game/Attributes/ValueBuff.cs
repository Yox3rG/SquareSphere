

public class ValueBuff : IBuff
{
    private static int nextID = 0;

    private int id;
    private Attribute.Type type;
    public Bonus bonus;

    int IBuff.ID { get { return id; } }

    public Attribute.Type AttribType { get { return type; } }

    public ValueBuff(Attribute.Type type, Bonus bonus)
    {
        id = nextID++;
        this.type = type;
        this.bonus = bonus;
    }

    public int CompareTo(IBuff other)
    {
        return id - other.ID;
    }
}