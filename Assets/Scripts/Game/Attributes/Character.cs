using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public IAttributeList Attributes { get; }

    public Character()
    {
        Attributes = new AttributeListFromDataBase();
        Attributes
            .Add(Attribute.Type.DAMAGE_FIRE,
                AttributeUserType.BALL | AttributeUserType.SE_BOMB)

            .Add(Attribute.Type.DAMAGE_MAGICAL,
                AttributeUserType.PU_MINILASER | AttributeUserType.SE_LASER)

            .Add(Attribute.Type.DAMAGE_COLD,
                AttributeUserType.BALL)

            .Add(Attribute.Type.DAMAGE_LIGHTNING,
                AttributeUserType.PU_MINILASER | AttributeUserType.SE_BOMB)

            .Add(Attribute.Type.DAMAGE_POISON,
                AttributeUserType.BALL);
    }
}
