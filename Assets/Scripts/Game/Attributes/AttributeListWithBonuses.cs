using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeListWithBonuses : IAttributeList
{
    private Dictionary<Attribute.Type, Bonus> attributesAndBonuses;

    public AttributeListWithBonuses()
    {
        attributesAndBonuses = new Dictionary<Attribute.Type, Bonus>();
    }

    public AttributeListWithBonuses(Dictionary<Attribute.Type, Bonus> attributes)
    {
        this.attributesAndBonuses = attributes;
    }


    public IAttributeList Add(Attribute.Type type)
    {
        return Add(type, null);
    }

    public IAttributeList Add(Attribute.Type attribute, Bonus? bonus)
    {
        if (!attributesAndBonuses.ContainsKey(attribute))
        {
            attributesAndBonuses.Add(attribute, bonus.GetValueOrDefault(new Bonus()));
        }

        return this;
    }

    public IAttributeList Add(Attribute.Type type, AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        return Add(type, null);
    }


    public IAttributeList Remove(Attribute.Type type)
    {
        attributesAndBonuses.Remove(type);
        return this;
    }

    public IAttributeList Remove(Attribute.Type type, AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        return Remove(type);
    }

    public Bonus CombineWith(IAttributeList other, Attribute.TypeGroup filter = (Attribute.TypeGroup)(-1))
    {
        if (other == null)
        {
            return GetBonuses(filter);
        }

        Bonus bonuses = new Bonus();
        foreach (var attrAndBonus in attributesAndBonuses)
        {
            if (Attribute.IsInGroup(attrAndBonus.Key, filter))
                bonuses += attrAndBonus.Value + other.GetBonus(attrAndBonus.Key);
        }
        return bonuses;
    }

    public Bonus CombineWith(IAttributeList other, AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        return CombineWith(other, AttributeUserType.NONE);
    }

    public Bonus GetBonus(Attribute.Type type)
    {
        if (attributesAndBonuses.TryGetValue(type, out Bonus bonus))
            return bonus;
        
        return new Bonus();
    }

    public Bonus GetBonuses(Attribute.TypeGroup filter = (Attribute.TypeGroup)(-1))
    {
        Bonus bonuses = new Bonus();
        foreach (var attrAndBonus in attributesAndBonuses)
        {
            if (Attribute.IsInGroup(attrAndBonus.Key, filter))
                bonuses += attrAndBonus.Value;
        }
        return bonuses;
    }

    public Bonus GetBonuses(AttributeUserType attrUserType = AttributeUserType.NONE)
    {
        return new Bonus();
    }
}
