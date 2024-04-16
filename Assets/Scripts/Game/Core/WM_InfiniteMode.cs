


public class WM_InfiniteMode : IWinningMechanism
{
    private SquareDataBase parent;

    public WM_InfiniteMode(SquareDataBase parent)
    {
        this.parent = parent;
    }

    public bool CheckIfWon()
    {
        return false;
    }
}
