using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : Destroyable
{
    private float offsetAmout = .1f;

    private Vector3 textOffset;

    private Vector2Int offset;


    public override Vector3 MiddlePoint => base.MiddlePoint + textOffset;
    public Vector2Int Offset { get { return offset; } }

    public override void SetDefaultState(int hp, byte colorIndex)
    {
        SetOrientationAndOffset();
     
        base.SetDefaultState(hp, colorIndex);
    }

    public override void SetMaxHp(int hp)
    {
        base.SetMaxHp(hp);
    }

    public override void ResetTextPosition()
    {
        base.ResetTextPosition();
        //TextGenerator.main.Move(text.gameObject, transform.position + textOffset);
    }

    public void SetOrientationAndOffset()
    {
        int orientation = 0;
        switch (type)
        {
            case ElementType.TRIANGLE_0:
                orientation = 0;
                break;
            case ElementType.TRIANGLE_90:
                orientation = 1;
                break;
            case ElementType.TRIANGLE_180:
                orientation = 2;
                break;
            case ElementType.TRIANGLE_270:
                orientation = 3;
                break;
        }
        SetOrientation(orientation);

        offset = FindOffset(transform.rotation.eulerAngles.z);

        textOffset = new Vector3(offset.x, offset.y) * offsetAmout;
    }

    private void SetOrientation(int orientation)
    {
        transform.eulerAngles = new Vector3(0, 0, orientation * 90f);
    }

    private Vector2Int FindOffset(float degrees)
    {
        int rotation = Mathf.FloorToInt(degrees);
        Vector2Int offset;

        switch (rotation)
        {
            case 0:
                offset = new Vector2Int(1, 1);
                break;
            case 270:
                offset = new Vector2Int(1, -1);
                break;
            case 180:
                offset = new Vector2Int(-1, -1);
                break;
            case 90:
                offset = new Vector2Int(-1, 1);
                break;
            default:
                offset = Vector2Int.zero;
                break;
        }

        return offset;
    }
}
