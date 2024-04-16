using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyButton : MonoBehaviour
{
    [Flags]
    public enum HighLightType
    {
        CLAIMED = 1,
        CURRENT = 2,
        LOCKED = 4
    }

    public bool isRandom;

    public Image img_reward;
    public Text txt_amount;

    public Text txt_day;
    public Image img_frame;
    public GameObject shine;
    public GameObject claimedShade;
    public GameObject claimedCheckMark;

    public void SetHighLightType(HighLightType type)
    {
        bool isClaimed = false;
        if ((type & HighLightType.CLAIMED) == HighLightType.CLAIMED)
        {
            isClaimed = true;

            img_frame.color = Color.gray;
            txt_day.color = Color.gray;

            if (!shine.activeSelf)
                shine.SetActive(true);
            if (!claimedShade.activeSelf)
                claimedShade.SetActive(true);
        }
        if ((type & HighLightType.CURRENT) == HighLightType.CURRENT)
        {
            img_frame.color = Color.yellow;
            txt_day.color = Color.yellow;

            if (!shine.activeSelf)
                shine.SetActive(true);
            if (claimedShade.activeSelf && !isClaimed)
                claimedShade.SetActive(false);
        }
        if ((type & HighLightType.LOCKED) == HighLightType.LOCKED)
        {
            img_frame.color = Color.white;
            txt_day.color = Color.white;

            if(shine.activeSelf)
                shine.SetActive(false);
            if (claimedShade.activeSelf)
                claimedShade.SetActive(false);
        }
    }

    public void SetRewardPackage(RewardPackage package)
    {
        if (isRandom)
            return;
        if (package == null)
            return;

        Sprite sprite = RewardPackageHandler.Peek(package, out string amount);

        img_reward.sprite = sprite;
        txt_amount.text = amount;
    }

    public void SetHighLightTypeWithRewardPackage(HighLightType type, RewardPackage package)
    {
        SetRewardPackage(package);
        SetHighLightType(type);
    }
}
