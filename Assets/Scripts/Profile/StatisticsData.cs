using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatisticsData
{
    // Profile related;
    public long experienceGained;
    public long goldEarned;


    // Gameplay related.
    public int deathCount;              // SquareDataBase
    public int levelsStarted;           // Menu
    public int levelsQuit;              // GamePlayMenu

    public long ballsFired;             // BallController
    public long blocksBroken;           // Destroyable
    public int blocksBrokenOneRound;    // BallController
    public double damageDealt;            // Destroyable
    public long attachmentsBroken;
    public long powerUpsActivated;

    public int highestScore;
    public double timeSpent;

    // Boss
    public int bossKilled;
    public int bossCubeKingKilled;
    public int bossCactusKilled;
    public int bossSquidKilled;
}
