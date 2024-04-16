


public class WM_BossMode : IWinningMechanism
{
    private SquareDataBase parent;
    private Boss boss;

    public WM_BossMode(SquareDataBase parent, Boss boss)
    {
        this.parent = parent;
        this.boss = boss;
    }

    public bool CheckIfWon()
    {
        return boss == null || !boss.IsAlive;
    }
}