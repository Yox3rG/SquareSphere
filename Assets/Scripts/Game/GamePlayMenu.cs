using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayMenu : MonoBehaviour
{
    public static GamePlayMenu main { get; private set; } = null;

    // "Temporary" array for the star alignment.
    private static string[] starNumberToName = new string[] { "L", "M", "R" };

    public GameObject gameplayPanel;
    public GameObject pausePanel;
    public GameObject gameEndPanel;

    public GameObject notRollingPanel;
    public GameObject rollingPanel;

    public GameObject SpeedUpIcon;

    public GameObject scorePanel;
    private Slider slr_Score;
    public Slider slr_XPbar;

    private Image[] img_Medals;
    private Color[] color_Medals;
    private int[] medalScoreRequirements;

    // Special abilities
    public Button btn_SpecialAbility_GoldCannon;
    public Button btn_SpecialAbility_BreakSides;
    public Button btn_SpecialAbility_MoveUp;
    public Button btn_SpecialAbility_ShieldForBalls;
    public Button btn_SpecialAbility_RandomForVideo;

    public SA_GoldCannon sa_GoldCannon = new SA_GoldCannon();
    public SA_BreakSides sa_BreakSides = new SA_BreakSides();
    public SA_MoveUp sa_MoveUp = new SA_MoveUp();
    public SA_ShieldForBalls sa_ShieldForBalls = new SA_ShieldForBalls();
    public SA_RandomForVideo sa_RandomForVideo = new SA_RandomForVideo();

    public Dictionary<SpecialAbility, Button> specialAbilitiesAndButtons;

    public Button btn_Next;
    public Button btn_SecondChance;

    public Text txt_LvlGameOver;
    public Text txt_LvlPause;
    public Text txt_Gold;
    public Text txt_Score;
    public Text txt_stageResult;

    private float sliderSize = 0f;
    private const float goldPercentage = .95f;

    private bool isScoreUpdating = false;
    private float startScore = 0f;
    private float targetScore = 0f;
    private float lerpingTime = 0f;

    private Score.MedalType nextMedal;
    private bool isNextMedal = true;

    private bool hadSecondChance = false;

    void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }

    public void Initialize()
    {
        if(ProfileDataBase.main != null)
        {
            txt_Gold.text = ProfileDataBase.main.GetCurrentGold().ToString();
        }
        else
        {
            txt_Gold.text = "69";
        }

        if (Menu.main)
        {
            txt_LvlPause.text = Menu.main.GetCurrentLvl().ToString();
        }
        else
        {
            txt_LvlPause.text = "UNKNOWN";
        }


        // Setting up score related variables.
        slr_Score = scorePanel.transform.Find("Line").GetComponent<Slider>();
        sliderSize = slr_Score.GetComponent<RectTransform>().sizeDelta.x;

        int medalCount = Score.GetMedalCount();
        img_Medals = new Image[medalCount];
        color_Medals = new Color[medalCount];

        string[] medalNames = Score.GetMedalNames();
        for(int i = 0; i < medalCount; i++)
        {
            img_Medals[i] = scorePanel.transform.Find(medalNames[i]).GetComponent<Image>();
            color_Medals[i] = img_Medals[i].color;
        }

        hadSecondChance = false;

        FillSADictionary();
        SetSAonClicksAndCosts();

        SetDefaultScoreState();
        SetMedalValues(new int[] { 100, 200, 400});
    }

    void Update()
    {
        // Testing purposes
        if (false && Input.GetKeyDown(KeyCode.A))
        {
            UpdateTargetScore(Mathf.FloorToInt(targetScore) + 40);
        }
        if (isScoreUpdating)
        {
            float smoothValue = Mathf.SmoothStep(startScore, targetScore, lerpingTime);

            slr_Score.value = smoothValue;

            if (isNextMedal && medalScoreRequirements[(int)nextMedal] <= slr_Score.value)
            {
                TriggerMedal();
            }

            lerpingTime += Time.deltaTime;
            if(lerpingTime >= 1f)
            {
                isScoreUpdating = false;
            }
        }
    }

    #region InGameScore
    private void SetDefaultScoreState()
    {
        slr_Score.value = 0f;
        slr_Score.minValue = 0f;
        foreach(Image img in img_Medals)
        {
            img.color = Color.gray;
        }

        txt_Score.text = "0";

        //float[] temp = { .25f, .5f, .8f };
        //SetMedalRequirementPercentages(temp);
    }

    public void SetMedalValues(int[] values)
    {
        //medalScoreRequirements = Array.ConvertAll(values, x => (float)x);
        medalScoreRequirements = values;

        slr_Score.maxValue = values[(int)Score.MedalType.GOLD] * (1 / goldPercentage);

        for (int i = 0; i < Score.GetMedalCount(); i++)
        {
            Vector2 temp = img_Medals[i].rectTransform.anchoredPosition;
            temp.x = sliderSize * (medalScoreRequirements[i] / slr_Score.maxValue);
            img_Medals[i].rectTransform.anchoredPosition = temp;
        }

        nextMedal = Score.MedalType.BRONZE;
        isNextMedal = true;
    }

    private void TriggerMedal()
    {
        int targetMedal = (int)nextMedal;
        img_Medals[targetMedal].color = color_Medals[targetMedal];

        if(++targetMedal >= Score.GetMedalCount())
        {
            isNextMedal = false;
        }
        else
        {
            nextMedal = (Score.MedalType)targetMedal;
        }
    }

    public void UpdateTargetScore(int newScore)
    {
        targetScore = newScore;
        startScore = slr_Score.value;

        lerpingTime = 0f;

        txt_Score.text = newScore.ToString();

        isScoreUpdating = true;
    }
    #endregion

    // Sometimes reffered to as "SA".
    #region SpecialAbility
    private void FillSADictionary()
    {
        specialAbilitiesAndButtons = new Dictionary<SpecialAbility, Button>();

        specialAbilitiesAndButtons[sa_GoldCannon] = btn_SpecialAbility_GoldCannon;
        specialAbilitiesAndButtons[sa_BreakSides] = btn_SpecialAbility_BreakSides;
        specialAbilitiesAndButtons[sa_MoveUp] = btn_SpecialAbility_MoveUp;
        specialAbilitiesAndButtons[sa_ShieldForBalls] = btn_SpecialAbility_ShieldForBalls;
        specialAbilitiesAndButtons[sa_RandomForVideo] = btn_SpecialAbility_RandomForVideo;
    }

    private void SetSAonClicksAndCosts()
    {
        SpecialAbility[] specialAbilities = specialAbilitiesAndButtons.Keys.ToArray();
        Button[] button_specialAbilities = specialAbilitiesAndButtons.Values.ToArray();

        for (int i = 0; i < specialAbilities.Length; i++)
        {
            int index = i;
            button_specialAbilities[i].onClick.AddListener(
                delegate { ActivateSpecialAbility(specialAbilities[index]); });
            
            Currency cost = specialAbilities[i].GetCost();
            if(cost.type != Currency.Type.AD_VIDEO)
            {
                button_specialAbilities[i].transform.GetComponentInChildren<Text>().text = cost.amount.ToString();
                // ############################### this is really bad :/
                button_specialAbilities[i].transform.Find("currency").GetComponent<Image>().sprite = cost.Icon;
            }
        }

        //btn_SpecialAbility_GoldCannon.onClick.AddListener(      delegate { ActivateSpecialAbility(sa_GoldCannon); });
        //btn_SpecialAbility_BreakSides.onClick.AddListener(      delegate { ActivateSpecialAbility(sa_BreakSides);});
        //btn_SpecialAbility_MoveUp.onClick.AddListener(          delegate { ActivateSpecialAbility(sa_MoveUp);});
        //btn_SpecialAbility_ShieldForBalls.onClick.AddListener(  delegate { ActivateSpecialAbility(sa_ShieldForBalls);});
        //btn_SpecialAbility_RandomForVideo.onClick.AddListener(  delegate { ActivateSpecialAbility(sa_RandomForVideo);});
    }

    // Only the gold cannon changes value at the moment, and it doesn't change icon.
    public void UpdateCosts()
    {
        specialAbilitiesAndButtons[sa_GoldCannon].transform.GetComponentInChildren<Text>().text = 
            sa_GoldCannon.GetCost().amount.ToString();
    }

    public void EnableAllSAButtons()
    {
        foreach(var button in specialAbilitiesAndButtons.Values)
        {
            button.interactable = true;
        }
    }

    private bool ActivateSpecialAbility(SpecialAbility ability)
    {
        if (ProfileDataBase.main)
        {
            bool canActivate = ability.BuyAndActivate();
            specialAbilitiesAndButtons[ability].interactable = !canActivate;
            return canActivate;
        }
        return false;
    }
    #endregion

    public void SetSpeedUp(bool value)
    {
        SpeedUpIcon.SetActive(value);
    }

    public void Pause(bool value)
    {
        pausePanel.SetActive(value);
        if (value)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void GameOverPanel(bool victory, bool visible = true)
    {
        gameEndPanel.SetActive(visible);
        if (visible)
        {
            slr_XPbar.normalizedValue = ProfileDataBase.main.GetExperiencePercentageOnCurrentLevel();

            btn_Next.gameObject.SetActive(victory);
            btn_SecondChance.gameObject.SetActive(!victory);
            txt_stageResult.text = victory ? "Level Cleared!" : "Level Failed!";

            // Testing purposes.
            string temp = Menu.main == null ? "UNKNOWN" : Menu.main.GetCurrentLvl().ToString();
            txt_LvlGameOver.text = "Stage " + temp;

            if (victory)
            {
                int earnedStarAmount = SquareDataBase.Instance.GetCurrentNumberOfStars();
                string starParentPath = "Panel/Stars/";

                for (int i = 0; i < earnedStarAmount; i++)
                {
                    StartCoroutine(ShowStarAfterDelay(i, gameEndPanel.transform.Find(starParentPath + starNumberToName[i] + "/StarAndParticle").gameObject));
                }
            }
        }
    }

    IEnumerator ShowStarAfterDelay(float time, GameObject starObject)
    {
        yield return new WaitForSeconds(time);

        starObject.SetActive(true);
    }

    public void SecondChance()
    {
        if (!hadSecondChance)
        {
            ProfileDataBase.main?.BuyActionForRewardedVideo(
                delegate
                {
                    Pause(false);
                    gameEndPanel.SetActive(false);
                    SquareDataBase.Instance.SpecialAbilityMoveUp(2);
                    btn_SecondChance.interactable = false;
                    hadSecondChance = true;
                });
        }
    }

    public void LoadNextLevel()
    {
        Pause(false);
        if (Menu.main != null)
        {
            Menu.main.LevelOnClick(Menu.main.GetCurrentLvl() + 1);
        }
    }

    public void ResetLevel()
    {
        Pause(false);
        if(Menu.main != null)
        {
            Menu.main.LevelOnClick(Menu.main.GetCurrentLvl());
        }
    }

    public void SetHomeScreen()
    {
        Pause(false);
        if (Menu.main != null)
        {
            if (MapDataBase.main != null && MapDataBase.main.IsTesting)
            {
                Menu.main.SetMapEditorScreen();
            }
            else
            {
                Menu.main.SetHomeScreen(comingFromActiveLevel: true);
            }

        }
        else
        {
            // HACK : This is for unity editor only.
            if (MapDataBase.main != null && MapDataBase.main.IsTesting)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }

    public void SetGameState(bool isRolling)
    {
        rollingPanel.SetActive(isRolling);
        notRollingPanel.SetActive(!isRolling);
    }
}
