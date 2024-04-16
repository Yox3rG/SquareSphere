using System.Collections.Generic;
using UnityEngine;



public class AttributeDataBase : MonoBehaviour
{
    public static AttributeDataBase Instance = null;

    private Dictionary<Attribute.Type, Attribute> attributes = new Dictionary<Attribute.Type, Attribute>();


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SetDefaultState();
    }

    private void SetDefaultState()
    {
        attributes[Attribute.Type.NUMBER_OF_BALLS] = new ValueAttribute(1);
        attributes[Attribute.Type.BALL_SPEED] = new ValueAttribute(10);
        attributes[Attribute.Type.BALL_SIZE] = new ValueAttribute(1);
        attributes[Attribute.Type.BALL_DAMAGE ] = new ValueAttribute(1);

        attributes[Attribute.Type.EXPLOSION_RADIUS] = new ValueAttribute(1);

        // Damage types
        attributes[Attribute.Type.DAMAGE_PHYSICAL ] = new BonusValueAttribute(0);
        attributes[Attribute.Type.DAMAGE_MAGICAL  ] = new BonusValueAttribute(0.2f);
        attributes[Attribute.Type.DAMAGE_FIRE     ] = new BonusValueAttribute(0.2f);
        attributes[Attribute.Type.DAMAGE_COLD     ] = new BonusValueAttribute(0.2f);
        attributes[Attribute.Type.DAMAGE_LIGHTNING] = new BonusValueAttribute(0.2f);
        attributes[Attribute.Type.DAMAGE_POISON   ] = new BonusValueAttribute(0);
        attributes[Attribute.Type.DAMAGE_RADIATION] = new BonusValueAttribute(0);
        
        attributes[Attribute.Type.PU_DAMAGE_EXPLOSION] = new ValueAttribute(10); 
        attributes[Attribute.Type.PU_DAMAGE_LASER] = new ValueAttribute(10);
        attributes[Attribute.Type.PU_DAMAGE_MINILASER] = new ValueAttribute(1);
    }

    public Attribute GetAttribute(Attribute.Type type)
    {
        return attributes[type];
    }

    public ValueAttribute GetValueAttribute(Attribute.Type type)
    {
        return GetAttribute(type) as ValueAttribute;
    }

    public bool ApplyBuff(IBuff buff)
    {
        bool success = attributes[buff.AttribType].ApplyBuff(buff);

#if UNITY_EDITOR
        if(!success)
            Debug.Log("Buff not applied.\n" + buff.ID);
#endif

        return success;
    }
}
