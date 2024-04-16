using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map
{
    // Different versions have to be interpreted differently.
    // Changing the layout should be followed by changing the version as well.
    public int version = 1;

    public List<SaveElementLine> lines = new List<SaveElementLine>();
    public int numberOfBalls = 10;

    public int destroyableCount = 0;
}
