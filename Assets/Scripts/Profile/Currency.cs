using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Currency
{
    public enum Type
    {
        GOLD,
        DIAMOND_APPLE,
        AD_VIDEO
    }


    [SerializeField] private Type _type;
    [SerializeField] private int _amount;

    public Type type { get { return _type; } }
    public int amount { get { return _amount; } }

    public Sprite Icon { get { return CurrencyIconHolder.GetCurrencyIcon(type); } }

    public Currency()
    {
        _type = Type.GOLD;
        _amount = 0;
    }

    public Currency(Type type, int amount)
    {
        this._type = type;
        this._amount = amount;
    }

    public override string ToString()
    {
        string temp = _amount.ToString() + " [";
        switch (_type)
        {
            case Type.GOLD:
                temp += "Gold]";
                break;
            case Type.DIAMOND_APPLE:
                temp += "Diamond apple(s)]";
                break;
            case Type.AD_VIDEO:
                temp += "Ad video]";
                break;
            default:
                temp = base.ToString();
                break;
        }
        return temp;
    }
}