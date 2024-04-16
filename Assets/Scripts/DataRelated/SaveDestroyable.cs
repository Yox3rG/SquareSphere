using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDestroyable
{
    public int maxHp;

    public byte colorIndex;

    public SaveDestroyable(int maxHp, byte colorIndex)
    {
        this.maxHp = maxHp;
        this.colorIndex = colorIndex;
    }

    public SaveDestroyable(SaveDestroyable s) : this(s.maxHp, s.colorIndex) { }
}
