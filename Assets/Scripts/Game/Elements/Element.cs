using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    public int row { get; protected set; }
    public int col { get; protected set; }

    public ElementType type { get; protected set; }

    public virtual Vector3 MiddlePoint { get { return transform.position; } }
    public virtual Vector3 DefaultScale { get { return Vector3.one; } }

    // Used mostly for random generating types, and setting UI elements.
    public const int destroyableStart = 0;
    public const int destroyableEnd = (int)ElementType.DESTROYABLE_END;
    public const int powerupStart = 10000;
    public const int powerupEnd = (int)ElementType.POWERUP_END;
    public const int specialElementStart = (int)ElementType.SPIKY;
    public const int specialElementEnd = (int)ElementType.DESTROYABLE_END;
    public const int bossRelatedStart = 20000;
    public const int bossRelatedEnd = (int)ElementType.BOSSRELATED_END;

    public enum ElementType
    {
        // DESTROYABLES
        SQUARE = destroyableStart,

        TRIANGLE_0,
        TRIANGLE_90,
        TRIANGLE_180,
        TRIANGLE_270,

        // SPECIAL ELEMENTS start here, end with destroyables ending.
        SPIKY,
        BOMB,
        LASER_HORIZONTAL,
        LASER_VERTICAL,
        LASER_BOTH,
        VASE,
        SHIELD_ODD,
        SHIELD_EVEN,

        // DESTROYABLES ending. This should stay as the last element.
        // Add DESTROYABLES before, and do not change the sequence.
        DESTROYABLE_END,

        // POWERUPS
        POWERUP_BONUSBALL = powerupStart,
        POWERUP_HORIZONTAL_LINE,
        POWERUP_VERTICAL_LINE,
        POWERUP_BOTH_LINE,
        POWERUP_DINO,
        POWERUP_SHIELD,

        // POWERUPS ending.This should stay as the last element.
        // Add POWERUPS before, and do not change the sequence.
        POWERUP_END,

        CACTUS_HEART = bossRelatedStart,

        BOSSRELATED_END,
    }

    public static bool IsExistingType(ElementType type)
    {
        return System.Enum.IsDefined(typeof(ElementType), type);
    }

    public static bool IsLaser(ElementType type)
    {
        return ((type == ElementType.LASER_BOTH) || (type == ElementType.LASER_VERTICAL) || (type == ElementType.LASER_HORIZONTAL));
    }

    public static bool IsTriangle(ElementType type)
    {
        return ((type == ElementType.TRIANGLE_0) || (type == ElementType.TRIANGLE_90) || (type == ElementType.TRIANGLE_180) || (type == ElementType.TRIANGLE_270));
    }

    public static bool IsDestroyable(ElementType type)
    {
        int typeNumber = (int)type;
        if ((typeNumber >= destroyableStart && typeNumber < destroyableEnd) || type == ElementType.CACTUS_HEART)
        {
            return true;
        }
        return false;
    }

    public static bool IsSpecial(ElementType type)
    {
        int typeNumber = (int)type;
        if (typeNumber >= specialElementStart && typeNumber < specialElementEnd)
        {
            return true;
        }
        return false;
    }

    public static bool IsPowerUp(ElementType type)
    {
        int typeNumber = (int)type;
        if (typeNumber >= powerupStart && typeNumber < powerupEnd)
        {
            return true;
        }
        return false;
    }

    // TODO: Check if these can simplify things...
    public bool IsDestroyable()
    {
        return IsDestroyable(type);
    }

    public bool IsPowerUp()
    {
        return IsPowerUp(type);
    }

    public void SetRowAndCol(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public virtual void SetType(ElementType type)
    {
        this.type = type;
    }
}
