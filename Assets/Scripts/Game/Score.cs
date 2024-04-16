using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    public int[] requirements { get; private set; }

    public int currentValue { get; private set; }
    public byte currentNumberOfStars { get; private set; }

    private int nextSquareScore;
    private readonly int defaultSquareScore = 10;
    private readonly int SquareScoreIncrease = 10;

    public bool IsDeathIncreasingNext { get; set; } = true;

    private static int medalCount = -1;

    public Score(int[] _requirements)
    {
        requirements = _requirements;
        currentNumberOfStars = 0;
        nextSquareScore = defaultSquareScore;

        //Debug.Log(_requirements[0] + "\n" + _requirements[1] + "\n" + _requirements[2] + "\n");
    }

    public int AddNext()
    {
        if (IsDeathIncreasingNext)
        {
            currentValue += nextSquareScore;
            nextSquareScore += SquareScoreIncrease;
        }
        else
        {
            currentValue += defaultSquareScore;
        }

        if(currentNumberOfStars < GetMedalCount() && currentValue > requirements[currentNumberOfStars])
        {
            currentNumberOfStars++;
        }

        return currentValue;
    }

    public void ResetNextToDefault()
    {
        nextSquareScore = defaultSquareScore;
    }

    public static int GetMedalCount()
    {
        if(medalCount == -1)
        {
            medalCount = System.Enum.GetValues(typeof(MedalType)).Length;
        }
        return medalCount;
    }

    public static string[] GetMedalNames()
    {
        string[] names = new string[GetMedalCount()];
        names[(int)MedalType.BRONZE] = "Bronze";
        names[(int)MedalType.SILVER] = "Silver";
        names[(int)MedalType.GOLD] = "Gold";

        return names;
    }

    public enum MedalType
    {
        BRONZE,
        SILVER,
        GOLD
    }
}
