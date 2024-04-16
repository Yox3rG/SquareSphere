using System.Collections.Generic;
using UnityEngine;

public class CurrencyIconHolder : MonoBehaviour
{
    private static List<Sprite> currencyIcons;

    private void Awake()
    {
        if (currencyIcons == null)
        {
            currencyIcons = new List<Sprite>();

            // Order matters. Keep the order of the enum in Currency.
            Sprite temp = Resources.Load<Sprite>("gold");
            currencyIcons.Add(temp);

            temp = Resources.Load<Sprite>("diamond");
            currencyIcons.Add(temp);

            temp = Resources.Load<Sprite>("smallVideo");
            currencyIcons.Add(temp);
        }
    }

    public static Sprite GetCurrencyIcon(Currency.Type type)
    {
        if(currencyIcons != null)
            return currencyIcons[(int)type];
        return null;
    }
}