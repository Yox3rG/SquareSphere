


using System.Collections.Generic;

public abstract class Attribute
{
    private const int stepBetweenSets = 10000;

    private static Dictionary<TypeGroup, UnityEngine.Vector2Int> groups;

    // Should be indexed from 0 to n-1
    // Do NOT change the order! (although I think you can)
    public enum TypeGroup
    {
        STATIC = 0,
        DAMAGE,
        ROUND_END,
        AFTER_X_ROUND,
        SHOOTING_PERCENT,
        BLOCT_DESTROYED,
        BOSS,
    }

    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // Do NOT change the order!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public enum Type
    {
        NUMBER_OF_BALLS = (int)TypeGroup.STATIC * stepBetweenSets,
        BALL_SPEED,
        BALL_SIZE,
        BALL_DAMAGE,

        EXPLOSION_RADIUS,

        PU_DAMAGE_EXPLOSION,
        PU_DAMAGE_LASER,
        PU_DAMAGE_MINILASER,

        
        DAMAGE_PHYSICAL = (int)TypeGroup.DAMAGE * stepBetweenSets,
        DAMAGE_MAGICAL,
        DAMAGE_FIRE,
        DAMAGE_COLD,
        DAMAGE_LIGHTNING,
        DAMAGE_POISON,
        DAMAGE_RADIATION,
    }

    static Attribute()
    {
        groups = new Dictionary<TypeGroup, UnityEngine.Vector2Int>();
        for (int i = 0; i < System.Enum.GetNames(typeof(TypeGroup)).Length; i++)
        {
            groups[(TypeGroup)i] = GenerateBoundaryFor(i);
        }

    }

    public static bool IsInGroup(Attribute.Type type, Attribute.TypeGroup group)
    {
        if ((int)group < 0)
            return true;

        int typeNum = (int)type;
        return typeNum >= groups[group].x && typeNum <= groups[group].y;
    }

    private static UnityEngine.Vector2Int GenerateBoundaryFor(int index)
    {
        return new UnityEngine.Vector2Int(stepBetweenSets * index++, stepBetweenSets * index - 1);
    }

    public abstract bool ApplyBuff(IBuff buff);

    public abstract bool RemoveBuff(IBuff buff);

    public abstract void ClearBuffs();
}