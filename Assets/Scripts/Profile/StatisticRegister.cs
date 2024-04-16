using System.Collections.Generic;
using UnityEngine;


public class StatisticRegister
{
    private ProfileDataBase profileDataBase;


    private Dictionary<System.Type, System.Action> bossToAction;

    public StatisticRegister(ProfileDataBase pb)
    {
        profileDataBase = pb;

        bossToAction = new Dictionary<System.Type, System.Action>()
        {
            { typeof(CubeKingBoss),
                () => { profileDataBase.StatIncreaseBossCubeKilled(); } },
            { typeof(CactusBoss),
                () => { profileDataBase.StatIncreaseBossCactusKilled(); } },
            { typeof(SquidBoss),
                () => { profileDataBase.StatIncreaseBossSquidKilled(); } }
        };

        RegisterActions();
    }


    public void RegisterActions()
    {
        SquareDataBase.OnLevelFailed += profileDataBase.StatIncreaseDeath;
        Menu.main.OnLevelStarted += profileDataBase.StatIncreaseLevelsStarted;
        Menu.main.OnLevelQuit += profileDataBase.StatIncreaseLevelsQuit;
        BallController.OnBallFired += profileDataBase.StatIncreaseBallsFired;
        BallController.OnNewBlocksBrokenInOneRoundHighScore += profileDataBase.StatSetBlocksBrokenOneRound;
        Destroyable.OnDestroyableBroken += profileDataBase.StatIncreaseBlocksBroken;
        Destroyable.OnDamaged += ProfileDataBase.main.StatIncreaseDamageDealt;
        Boss.OnBossBroken += StatIncreaseBossKill;
    }

    public void UnRegisterActions()
    {
        SquareDataBase.OnLevelFailed -= profileDataBase.StatIncreaseDeath;
        Menu.main.OnLevelStarted -= profileDataBase.StatIncreaseLevelsStarted;
        Menu.main.OnLevelQuit -= profileDataBase.StatIncreaseLevelsQuit;
        BallController.OnBallFired -= profileDataBase.StatIncreaseBallsFired;
        BallController.OnNewBlocksBrokenInOneRoundHighScore -= profileDataBase.StatSetBlocksBrokenOneRound;
        Destroyable.OnDestroyableBroken -= profileDataBase.StatIncreaseBlocksBroken;
        Destroyable.OnDamaged -= ProfileDataBase.main.StatIncreaseDamageDealt;
        Boss.OnBossBroken -= StatIncreaseBossKill;
    }

    private void StatIncreaseBossKill(System.Type bossType)
    {
        if (bossToAction.ContainsKey(bossType))
        {
            bossToAction[bossType]();
        }

#if UNITY_EDITOR
        Debug.Log("Type of boss killed: " + bossType);
#endif
    }
}
