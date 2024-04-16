


public class LM_NormalMode : ILoosingMechanism
{
    private SquareDataBase parent;

    public LM_NormalMode(SquareDataBase parent)
    {
        this.parent = parent;
    }

    public bool CheckIfLost()
    {
        return parent.IsDestroyableInRow(parent.LastRowIndex);
    }
}