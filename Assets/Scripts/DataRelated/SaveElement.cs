using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveElement
{
    public int row, col;

    public Element.ElementType type;

    public SaveDestroyable destroyable;


    public bool IsDestroyable { get
        {
            return Element.IsDestroyable(type);
        } 
    }

    private const int defaultMaxHp = 100;

    public SaveElement(Element.ElementType type, int row, int col)
    {
        this.row = row;
        this.col = col;
        this.type = type;

        if (Element.IsDestroyable(type))
        {
            Debug.Log("The SaveElement created is destroyable, but has no health or color. \nDefault properties set.");
            destroyable = new SaveDestroyable(defaultMaxHp, (byte)Palette.DefaultColorIndex);
        }
    }

    public SaveElement(Element.ElementType type, int row, int col, SaveDestroyable destroyable)
    {
        this.row = row;
        this.col = col;
        this.type = type;

        this.destroyable = new SaveDestroyable(destroyable);
    }

    public SaveElement(SaveElement s) : this(s.type, s.row, s.col, s.destroyable) { }
}
