using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlockB
{
    public bool isSquare; // { get; private set; }

    public int col; // { get; private set; }

    // Square attributes.
    public Destroyable.ElementType squareType; // { get; private set; }
    public int maxHp; // { get; private set; }
    public Color color;

    // PowerUp attribute.
    public PowerUp.ElementType powerType; // { get; private set; }

    /*
    private Block(bool isSquare, int col)
    {
        this.isSquare = isSquare;
        this.col = col;
        this.color = Color.white;
    }

    public Block(Square.Type type, int col, int maxHp) : this(true, col)
    {
        
        this.squareType = type;
        this.maxHp = maxHp;
    }

    public Block(PowerUp.Type type, int col) : this(false, col)
    {
        this.powerType = type;
    }

    public Block(Square.Type type, int col, int maxHp, Color color) : this(type, col, maxHp)
    {
        this.color = color;
    }

    public Block(PowerUp.Type type, int col, Color color) : this(type, col)
    {
        this.color = color;
    }
    */
}
