


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class BossTypes
{
    private static Type[] types;
    private static Dictionary<Type, TopMostRow> topMostRows;

    public static int Length { get { return types.Length; } }

    static BossTypes()
    {
        LoadBossTypes();
        LoadTopMostRowsForBosses();
    }

    public static Type Get(int index)
    {
        try
        {
            //Debug.Log(types[index]);
            return types[index];
        }
        catch(IndexOutOfRangeException e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
        return GetDefaultBossType();
    }

    public static TopMostRow GetTopMostRow(Type bossType)
    {
        //Debug.Log(topMostRows[types[0]] + " | " + topMostRows[types[1]] + " | " + topMostRows[types[2]]);
        if (!topMostRows.ContainsKey(bossType))
            return topMostRows[GetDefaultBossType()];
        return topMostRows[bossType];
    }

    public static Type GetDefaultBossType()
    {
        return types[0];
    }

    // New bosses should be added to this 2 functions.
    private static void LoadBossTypes()
    {
        types = new Type[3];
        types[0] = typeof(CubeKingBoss);
        types[1] = typeof(CactusBoss);
        types[2] = typeof(SquidBoss);
    }

    private static void LoadTopMostRowsForBosses()
    {
        topMostRows = new Dictionary<Type, TopMostRow>(3);
        topMostRows[types[0]] = new TopMostRow(new[] { 0, 0, 0, 4, 4, 4, 4 });
        topMostRows[types[1]] = new TopMostRow(new[] { 0, 0, 0, 4, 4, 4, 4 });
        topMostRows[types[2]] = new TopMostRow(new[] { 0, 0, 0, 4, 4, 4, 4 });
    }
}
