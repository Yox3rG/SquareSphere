

public struct Bonus
{
    public float amount;
    public float percent;

    public Bonus(float amount, float percent)
    {
        this.amount = amount;
        this.percent = percent;
    }

    public Bonus(Bonus b)
    {
        this.amount = b.amount;
        this.percent = b.percent;
    }

    public static Bonus operator +(Bonus a, Bonus b)
    {
        return new Bonus(a.amount + b.amount, a.percent + b.percent);
    }

    public static Bonus operator -(Bonus a, Bonus b)
    {
        return new Bonus(a.amount - b.amount, a.percent - b.percent);
    }

    public float GetResultWhenAppliedTo(float baseValue)
    {
        return (baseValue) * (1 + percent) + amount;
    }

    public void Clear()
    {
        amount = 0;
        percent = 0;
    }

    public override string ToString()
    {
        return "Amount: " + amount + " Percent: " + percent;
    }
}
