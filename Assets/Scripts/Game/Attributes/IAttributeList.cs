using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttributeList
{
    IAttributeList Add(Attribute.Type type);
    IAttributeList Add(Attribute.Type type, Bonus? bonus);
    IAttributeList Add(Attribute.Type type, AttributeUserType attrUserType = AttributeUserType.NONE);
    IAttributeList Remove(Attribute.Type type);
    IAttributeList Remove(Attribute.Type type, AttributeUserType attrUserType = AttributeUserType.NONE);

    Bonus GetBonus(Attribute.Type type);
    Bonus GetBonuses(Attribute.TypeGroup filter = (Attribute.TypeGroup)(-1));
    Bonus GetBonuses(AttributeUserType attrUserType = AttributeUserType.NONE);
    Bonus CombineWith(IAttributeList other, Attribute.TypeGroup filter = (Attribute.TypeGroup)(-1));
    Bonus CombineWith(IAttributeList other, AttributeUserType attrUserType = AttributeUserType.NONE);
}
