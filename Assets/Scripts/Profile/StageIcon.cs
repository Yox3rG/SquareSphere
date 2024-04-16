using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIcon : MonoBehaviour
{
    // static?
    public static Sprite spr_NormalLock;
    public static Sprite spr_BossLock;
    public static Sprite spr_RibbonLock;

    // Colors.
    private static Color32 lvlFilledStarColor;
    private static Color32 lvlEmptyStarColor;

    public GameObject lockedPanel;
    public GameObject unlockedPanel;

    public Image img_Lock;
    public Image img_Boss;
    public Image img_Ribbon;

    public Image[] stars;

    public Text txt_stageNumber;

    private bool isLocked = true;

    private void Awake()
    {
        // HACK: real sprites not present.
        spr_NormalLock = Resources.Load<Sprite>("lock");
        spr_BossLock = Resources.Load<Sprite>("lockBoss");
        spr_RibbonLock = Resources.Load<Sprite>("lockRibbon");

        lvlFilledStarColor = new Color32(255, 221, 85, 255);
        lvlEmptyStarColor = Color.white;
    }

    public void SetLocked(bool locked = true)
    {
        lockedPanel.SetActive(locked);
        unlockedPanel.SetActive(!locked);

        isLocked = locked;
    }

    public void SetState(Stage.LayoutType type)
    {
        switch (type)
        {
            case Stage.LayoutType.BOSS:
                SetBossState();
                break;
            case Stage.LayoutType.COLORED:
                SetRibbonState();
                break;
            case Stage.LayoutType.DEFAULT:
            case Stage.LayoutType.GENERATED:
            default:
                SetDefaultState();
                break;
        }
    }

    public void SetStageTextAndStars(int stageNumber, byte starCount)
    {
        if (!isLocked)
        {
            txt_stageNumber.text = stageNumber.ToString();

            for(int i = 0; i < stars.Length; i++)
            {
                stars[i].color = starCount > i ? lvlFilledStarColor : lvlEmptyStarColor;
            }
        }
    }

    private void SetDefaultState()
    {
        img_Lock.sprite = spr_NormalLock;
        img_Lock.SetNativeSize();

        txt_stageNumber.gameObject.SetActive(true);


        if (!isLocked)
        {
            img_Boss.enabled = false;
            img_Ribbon.enabled = false;
        }
    }

    private void SetBossState()
    {
        img_Lock.sprite = spr_BossLock;
        img_Lock.SetNativeSize();

        txt_stageNumber.gameObject.SetActive(false);

        if (!isLocked)
        {
            img_Boss.enabled = true;
            img_Ribbon.enabled = false;
        }
    }

    private void SetRibbonState()
    {
        img_Lock.sprite = spr_RibbonLock;
        img_Lock.SetNativeSize();

        txt_stageNumber.gameObject.SetActive(true);


        if (!isLocked)
        {
            img_Boss.enabled = false;
            img_Ribbon.enabled = true;
        }
    }

    //private void ShowAllPlayableLevels(int from)
    //{
    //    if (from < 0)
    //    {
    //        from = 0;
    //    }

    //    int i = from;
    //    int lastPlayableLevel = ProfileDataBase.main.GetLastPlayableMap();

    //    while (i < lastPlayableLevel)
    //    {
    //        lvlButtons[i].transform.Find("Locked").gameObject.SetActive(false);
    //        lvlButtons[i].transform.Find("Unlocked").gameObject.SetActive(true);
    //        for (int j = 0; j < ProfileDataBase.main.GetStarsOnMap(i); j++)
    //        {
    //            string currentTransformPath = "Unlocked/Stars/" + starNumberToName[j];
    //            lvlButtons[i].transform.Find(currentTransformPath).GetComponent<Image>().color = lvlFilledStarColor;
    //        }

    //        i++;
    //    }

    //    lvlButtons[i].transform.Find("Locked").gameObject.SetActive(false);
    //    lvlButtons[i].transform.Find("Unlocked").gameObject.SetActive(true);
    //}
}
