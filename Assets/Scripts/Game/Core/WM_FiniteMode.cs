



using UnityEngine;

public class WM_FiniteMode : IWinningMechanism
{
    private SquareDataBase parent;

    public WM_FiniteMode(SquareDataBase parent)
    {
        this.parent = parent;
    }

    public bool CheckIfWon()
    {
        return parent.DestroyablesLeft() == 0 &&
            parent.GetMaxSpwanRoundNumber() <= parent.GetCurrentRoundNumber();
    }
}
