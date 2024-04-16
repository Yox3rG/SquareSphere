using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class StageLayout
{
    private static StageLayout main;

    private static Dictionary<int, Stage> levels;

    private int lastCreatedKey = 0;
    private int levelCount = 0;

    private int lastGeneratedValue = 0;

    private const int bossLevelCounter = 5;
    private const int bossLevelCounterBits = 3; // 0011

    // MapDataBase doesn't know about stage numbers, only the seed.
    // Assigned in constructor.
    private int lastNoTriangleSeed = 0;
    private int lastNoPowerUpSeed = 0;
    private int lastNoSpecialElementSeed = 0;

    public static StageLayout GetMain()
    {
        if(main == null)
        {
            main = new StageLayout();
        }
        return main;
    }

    private StageLayout()
    {
        levels = new Dictionary<int, Stage>() {
            { 0, new NamedStage(Stage.LayoutType.DEFAULT, "tutorial01") },
            { 1, new NamedStage(Stage.LayoutType.DEFAULT, "tutorial02") },
            { 2, new NamedStage(Stage.LayoutType.DEFAULT, "tutorial03") },
            { 4, new NamedStage(Stage.LayoutType.COLORED, "colored01") },
            { 6, new NamedStage(Stage.LayoutType.COLORED, "colored02") }
        };

        lastCreatedKey = levels.Keys.Max();
        levelCount = levels.Count();

        // Change these when tutorial length changes.
        lastNoTriangleSeed = 3;
        lastNoPowerUpSeed = 6;
        lastNoSpecialElementSeed = 10;

        int next = 0;
        for (int i = 0; i < lastCreatedKey; i++)
        {
            if (!levels.ContainsKey(i))
            {
                levels.Add(i, GetSeededStageFromIndex(i, next++));
            }
            levels[i].number = i;
        }

        lastGeneratedValue = next == 0 ? next : --next;
        
    }

    private SeededStage GetSeededStageFromIndex(int index, int next)
    {
        if ((index % bossLevelCounter) == 0)
            return new BossStage(next, GetBossType(index));
        else
            return new SeededStage(Stage.LayoutType.GENERATED, next);
    }

    private System.Type GetBossType(int index)
    {
        return BossTypes.Get(index % BossTypes.Length);
    }

    //private SeededStage GetSeededStageFromIndexBitWise(int i, int next)
    //{
    //    if ((i & bossLevelCounterBits) == bossLevelCounterBits)
    //        return new SeededStage(Stage.LayoutType.BOSS, next);
    //    else
    //        return new SeededStage(Stage.LayoutType.GENERATED, next);
    //}

    public bool HasTriangles(int seed)
    {
        return seed > lastNoTriangleSeed;
    }
    public bool HasPowerUps(int seed)
    {
        return seed > lastNoPowerUpSeed;
    }
    public bool HasSpecialElements(int seed)
    {
        return seed > lastNoSpecialElementSeed;
    }

    public Stage GetStage(int stageNumber)
    {
        // Chosen level is greater than any created level.
        if(stageNumber > lastCreatedKey)
        {
            int resultInt = stageNumber - lastCreatedKey + lastGeneratedValue;
            Stage stage = GetSeededStageFromIndex(stageNumber, resultInt);
            stage.number = stageNumber;

            return stage;
        }

        // Chosen level is part of the dictionary of created maps.
        if (levels.ContainsKey(stageNumber))
        {
            return levels[stageNumber];
        }

        return null;
    }
}
