using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TopMostRow
{
    private int[] topRows = new int[SquareDataBase.cols];

    public TopMostRow()
    {
    }

    public TopMostRow(int[] topRows)
    {
        for (int i = 0; i < this.topRows.Length; i++)
        {
            if (i < topRows.Length)
                this.topRows[i] = Math.Max(topRows[i], 0);
            else
                this.topRows[i] = 0;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < this.topRows.Length; i++)
        {
            this.topRows[i] = 0;
        }
    }

    public int Get(int index)
    {
        return topRows[index];
    }

    public void Set(int index, int value)
    {
        topRows[index] = value;
    }

    public void CopyTo(ref TopMostRow target)
    {
        for (int i = 0; i < topRows.Length; i++)
        {
            target.Set(i, topRows[i]);
        }
    }

    public override string ToString()
    {
        StringBuilder sr = new StringBuilder();
        for (int i = 0; i < topRows.Length; i++)
        {
            sr.Append(topRows[i]).Append(", ");
        }
        return sr.ToString();
    }
}
