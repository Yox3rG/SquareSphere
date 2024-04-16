using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stage
{
    public enum LayoutType
    {
        GENERATED,
        BOSS,
        DEFAULT,
        COLORED
    }

    public LayoutType type;
    public int number;

    public virtual bool IsNamed()
    {
        if(type == LayoutType.GENERATED || type == LayoutType.BOSS)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public abstract TopMostRow GetTopMostRows();

    // Might not be the best solution.
}

public class SeededStage : Stage
{
    public int stageSeed;

    public SeededStage(LayoutType _type, int stageSeed)
    {
        if (_type == LayoutType.COLORED || _type == LayoutType.DEFAULT)
        {
            Debug.Log("Seeded map generated with not seeded parameters.");
            type = LayoutType.GENERATED;
            this.stageSeed = 0;
        }
        else
        {
            type = _type;
            this.stageSeed = stageSeed;
        }
    }

    public override bool IsNamed()
    {
        return false;
    }

    public override TopMostRow GetTopMostRows()
    {
        return new TopMostRow();
    }
}

public class NamedStage : Stage
{
    public string stagePath;

    public NamedStage(LayoutType _type, string _stageName)
    {
        if (_type == LayoutType.GENERATED || _type == LayoutType.BOSS)
        {
            Debug.Log("Generated level type cannot have a name. Values given: \n" + _type + _stageName);
            type = LayoutType.DEFAULT;
            stagePath = "";
        }
        else
        {
            stagePath = _stageName;
            type = _type;
        }

    }

    public override bool IsNamed()
    {
        return true;
    }

    public override TopMostRow GetTopMostRows()
    {
        return new TopMostRow();
    }
}
