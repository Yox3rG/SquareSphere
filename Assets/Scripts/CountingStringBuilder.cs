using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CountingStringBuilder
{
    private StringBuilder strBuilder = null;
    private int count;

    public int Count
    {
        get
        {
            return count;
        }
    }

    public CountingStringBuilder()
    {
        strBuilder = new StringBuilder();
        count = 0;
    }

    public CountingStringBuilder AppendLine(string line)
    {
        strBuilder.AppendLine(line);
        count++;

        return this;
    }

    public CountingStringBuilder Append(string str)
    {
        strBuilder.Append(str);

        return this;
    }

    public CountingStringBuilder Clear()
    {
        strBuilder.Clear();
        count = 0;

        return this;
    }

    public override string ToString()
    {
        return strBuilder.ToString();
    }
}
