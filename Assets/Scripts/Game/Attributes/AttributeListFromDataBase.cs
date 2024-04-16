using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeListFromDataBase : IAttributeList
{
    private Dictionary<Attribute.Type, AttributeUserType> attributes;

    public AttributeListFromDataBase()
    {
        attributes = new Dictionary<Attribute.Type, AttributeUserType>();
    }

    public AttributeListFromDataBase(Dictionary<Attribute.Type, AttributeUserType> attributes)
    {
        this.attributes = attributes;
    }


    public IAttributeList Add(Attribute.Type type)
    {
        return Add(type, AttributeUserType.NONE);
    }

    public IAttributeList Add(Attribute.Type type, Bonus? bonus = null)
    {
        
        return Add(type);
    }

    public IAttributeList Add(Attribute.Type type, AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        if (attributes.ContainsKey(type))
        {
            attributes[type] |= attrUserType;
        }
        else
        {
            attributes[type] = attrUserType;
        }
        return this;
    }

    public IAttributeList Remove(Attribute.Type type)
    {
        attributes.Remove(type);
        return this;
    }

    public IAttributeList Remove(Attribute.Type type, AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        if (attributes.ContainsKey(type))
        {
            attributes[type] &= ~attrUserType;
            if(attributes[type] == AttributeUserType.NONE)
            {
                Remove(type);
            }
        }
        return this;
    }

    public Bonus CombineWith(IAttributeList other, Attribute.TypeGroup filter = (Attribute.TypeGroup)(-1))
    {
        if (other == null)
        {
            return GetBonuses(filter);
        }

        Bonus bonus = new Bonus();
        foreach (var attr in attributes)
        {
            if (Attribute.IsInGroup(attr.Key, filter))
                bonus += GetBonus(attr.Key) + other.GetBonus(attr.Key);
        }
        return bonus;
    }

    public Bonus CombineWith(IAttributeList other, AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        if (other == null)
        {
            return GetBonuses(attrUserType);
        }

        Bonus bonus = new Bonus();
        foreach (var attr in attributes)
        {
            if ((attr.Value & attrUserType) == attrUserType)
                bonus += GetBonus(attr.Key) + other.GetBonus(attr.Key);
        }
        return bonus;
    }

    public Bonus GetBonus(Attribute.Type type)
    {
        ValueAttribute v = AttributeDataBase.Instance.GetValueAttribute(type);
        if (v != null)
            return v.GetBonus();
        else
            return new Bonus();
    }

    public Bonus GetBonuses(Attribute.TypeGroup filter = (Attribute.TypeGroup)(-1))
    {
        Bonus bonuses = new Bonus();
        foreach (var attr in attributes)
        {
            if(Attribute.IsInGroup(attr.Key, filter))
                bonuses += GetBonus(attr.Key);
        }
        return bonuses;
    }

    public Bonus GetBonuses(AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        Bonus bonuses = new Bonus();
        foreach (var attr in attributes)
        {
            if ((attr.Value & attrUserType) == attrUserType)
                bonuses += GetBonus(attr.Key);
        }
        return bonuses;
    }
}
