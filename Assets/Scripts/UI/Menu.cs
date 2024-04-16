using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class Menu : MonoBehaviour
{
    public static Menu main { get; private set; }

    public event Action OnBeforeLoadingGameScene;
    public event Action OnBeforeLoadingMenuScene;
    public event Action OnBeforeLoadingMapEditorScene;

    public event Action OnLevelStarted;
    public event Action OnLevelQuit;

    public const int MAP_TESTING_LVL = -696969;

    public GameObject canvas;
    // HACK: Changed to private, not sure if healthy.
    private GameObject gameCanvas;

    // Used for the selected ball in the shop.
    public GameObject checkMark;
    private int checkMarkOffsetY = -100;

    // Colors for the different states of the buttons.
    private Color selectedColor;
    private Color lockedColor;
    private Color unlockedColor;

    // Text.
    public Text txt_versionNumber;

    public Text goldText_Shop;
    public Text goldText_Profile;
    public Text diamondText_Shop;
    public Text starText_Profile;
    public Text nameText_Profile;
    public Text xpText_Profile;
    public Text lvlText_Profile;
    public Text statList_Profile;
    public Text statListUpdated_Profile;
    public Text statListUpdatedNumber_Profile;


    public InputField inf_changeName;
    public Button btn_changeName;
    public Text txt_changeNameInvalid;

    // Panels.
    // Contains all the other ones except the last one, updated in Start()
    private GameObject[] panels;
    public GameObject menuPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;
    public GameObject profilePanel;
    // This one isn't truly a panel.
    public GameObject popupForBuyPanel;
    public GameObject popupForNameChange;

    // ScrollRelated.
    public LoopScrollRect lvlLoopScrollRect;
    public GameObject shopScrollContent;

    // Prefabs.
    //public GameObject lvlPrefab;
    public GameObject shopPrefab;

    // GeneratedButtons.
    private List<GameObject> shopButtons;

    private int currentSelectedBallToBuy = 0;

    private int currentLvl = 0;


    private bool isInteractive = true;
    private bool isInteractiveItemActive;
    private float freqInteractiveItemUpdate = 1f;
    private float nextActivationTime = 0;

    void Awake()
    {
        if(main == null)
        {
            main = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(canvas);

            currentLvl = 0;

            //OnBeforeLoadingMenuScene += delegate { Debug.Log("BeforeLoadingMenu"); };
            //OnBeforeLoadingGameScene += delegate { Debug.Log("BeforeLoadingGame"); };
        }
        else
        {
            Destroy(canvas);
            Destroy(gameObject);
        }
    }

    void Start()
    {
        panels = new GameObject[] { menuPanel, shopPanel, profilePanel, settingsPanel };


        SetDefaultState();
        SetShownVersionNumber();

        AskForNameIfEmpty();
        CreateAnalysisFile();
    }

    private void Update()
    {
        UpdateInteractiveStats();
    }

    private void CreateAnalysisFile()
    {
        ProfileDataBase.main.CreateHeaderToChainAnalysis();
    }

    // TODO: Save should probably be called more frequently.
    void OnApplicationQuit()
    {
        if(SquareDataBase.Instance != null)
        {
            bool? isTesting = MapDataBase.main?.IsTesting;
            if (isTesting.HasValue && !isTesting.Value)
            {
                OnLevelQuit?.Invoke();
            }
        }

        ProfileDataBase.main.StatIncreaseTimeSpent();
        ProfileDataBase.main.SaveData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        ProfileDataBase.main.StatIncreaseTimeSpent();
        ProfileDataBase.main.SaveData();
    }


    private void SetShownVersionNumber()
    {
        txt_versionNumber.text = Application.version;
    }

    #region ButtonGeneration
    private void GenerateShopPrefabs(int itemCount)
    {
        shopButtons = GeneratePrefabsWithOnclick(OnShopElementClick, itemCount, shopPrefab, shopScrollContent.transform);
    }

    private List<GameObject> GeneratePrefabsWithOnclick(System.Action<int> onclickFunc, int count, GameObject prefab, Transform parent)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject g = Instantiate(prefab, parent);
            g.GetComponentInChildren<Text>(true).text = i.ToString();
            g.name = i.ToString();
            int iValue = i;
            g.GetComponent<Button>().onClick.AddListener(delegate { onclickFunc(iValue); });

            gameObjects.Add(g);
        }

        return gameObjects;
    }
    #endregion

    #region ButtonOnClicks
    public void OnShopElementClick(int index)
    {
        //Debug.Log("Clicked on shop element : " + index);
        if (ProfileDataBase.main.IsBallUnLocked(index))
        {
            if (ProfileDataBase.main.GetSelectedBallIndex() == index)
            {
                // Clicking the selected ball shouldn't do anything, right?!
            }
            else
            {
                SelectBall(index);
            }
        }
        else
        {
            currentSelectedBallToBuy = index;
            EnableBuyingPopupWindow(index);
        }
    }

    public void LevelOnClick(int lvl)
    {
        if (ProfileDataBase.main.IsLevelPlayable(lvl))
        {
            OnLevelStarted?.Invoke();
            currentLvl = lvl;
            SetGameScreen();
        }
        else
        {
            // TODO: Unplayable level onClick.
        }
    }

    public void BuyBallOnClick()
    {
        BuyBall(currentSelectedBallToBuy);
    }

    // HACK lot of debugging here, but these could have importance. (grammar?!)
    private void BuyBall(int index)
    {
        ProfileDataBase.PaymentResult result = ProfileDataBase.main.BuyBall(index);
        switch (result)
        {
        case ProfileDataBase.PaymentResult.SUCCESS:
                OnSuccessfulBallPurchase();
                Debug.Log("Ball payment: SUCCESS ");
            break;
        case ProfileDataBase.PaymentResult.WATCHING_AD:
                AdManager.Instance.SetAfterRewardAction(OnSuccessfulBallPurchase);
                Debug.Log("Ball payment: WATCHING_AD ");
            break;
        case ProfileDataBase.PaymentResult.NOT_ENOUGH_CURRENCY:
                Debug.Log("Ball payment: NOT_ENOUGH_CURRENCY ");
            break;
        // These in theory should never run. There is a problem if you see them...
        case ProfileDataBase.PaymentResult.ALREADY_PURCHASED:
                Debug.Log("Ball payment: ALREADY_PURCHASED ");
            break;
        case ProfileDataBase.PaymentResult.NO_SUCH_ITEM:
                Debug.Log("Ball payment: NO_SUCH_ITEM ");
            break;
        case ProfileDataBase.PaymentResult.UNSPECIFIED_ERROR:
            Debug.Log("Ball payment: UNSPECIFIED_ERROR ");
            break;

        }
    }

    public void DisableBuyingPopupWindow()
    {
        popupForBuyPanel.SetActive(false);
    }

    // This is not an onclick function (is also private), it is used in the OnShopElementClick().
    private void EnableBuyingPopupWindow(int clickedBallIndex)
    {
        popupForBuyPanel.SetActive(true);
        Currency cost = ProfileDataBase.main.GetBallCost(clickedBallIndex);
        popupForBuyPanel.transform.Find("PopUpBuy/Price").GetComponent<Text>().text = cost.amount.ToString();
        popupForBuyPanel.transform.Find("PopUpBuy/CurrencyIcon").GetComponent<Image>().sprite = cost.Icon;
        popupForBuyPanel.transform.Find("PopUpBuy/Ball").GetComponent<Image>().sprite = BallTextureHolder.GetShopBallSprite(clickedBallIndex);
    }

    // This neither is for onclick, in BuyBall().
    private void OnSuccessfulBallPurchase()
    {
        // Ball is selected in ProfileDataBase already.
        RefreshAllShownBallStates();
        DisableBuyingPopupWindow();
        RefreshShownCurrencyAmount();
        RefreshShownStats();
    }

    public void EnableNameChangePopupWindow()
    {
        txt_changeNameInvalid.gameObject.SetActive(false);
        inf_changeName.text = "";

        popupForNameChange.SetActive(true);
        Transform cancel = popupForNameChange.transform.Find("Background/Cancel");
        cancel.GetComponent<Button>().interactable = true;
        // HACK: Lazy coloring solution.
        cancel.GetComponentInChildren<Text>().color = Color.white;
    }

    private void EnableNameChangePopupWindowWithoutCancel()
    {
        EnableNameChangePopupWindow();
        Transform cancel = popupForNameChange.transform.Find("Cancel");
        cancel.GetComponent<Button>().interactable = false;
        // Lazy coloring solution.
        cancel.GetComponentInChildren<Text>().color = new Color32(200, 200, 200, 128);
    }

    public void DisableChangeNamePopupWindow()
    {
        popupForNameChange.SetActive(false);
    }

    public enum NamingError
    {
        NONE,
        EMPTY,
        TOO_LONG,
        NOT_ALLOWED_CHARACTERS
    }

    public void ChangeName()
    {
        string temp = inf_changeName.text;
        NamingError error = NamingError.NONE;

        if (IsNameCorrect(temp, out error))
        {
            ProfileDataBase.main.ChangeName(temp);
            RefreshShownName();
            DisableChangeNamePopupWindow();
        }
        else
        {
            txt_changeNameInvalid.gameObject.SetActive(true);

            string errorMessage = "";

            switch (error)
            {
                case NamingError.EMPTY:
                    errorMessage = "Name can not be empty string";
                    break;
                case NamingError.TOO_LONG:
                    errorMessage = "The name is too long";
                    break;
                case NamingError.NOT_ALLOWED_CHARACTERS:
                    errorMessage = "Not allowed characters are used";
                    break;
            }

            txt_changeNameInvalid.text = errorMessage;
        }


    }
    #endregion

    private bool IsNameCorrect(string name, out NamingError error)
    {
        bool value = true;
        error = NamingError.NONE;

        if(name.Length == 0)
        {
            error = NamingError.EMPTY;
            value = false;
        }
        if(name.Length >= 14)
        {
            error = NamingError.TOO_LONG;
            value = false;
        }

        return value;
    }

    private void AskForNameIfEmpty()
    {
        if (ProfileDataBase.main.GetName().Length == 0)
        {
            EnableNameChangePopupWindowWithoutCancel();
        }
    }

    private void ApplyTextureToShopPrefabs()
    {
        if (shopScrollContent.transform.childCount == BallTextureHolder.GetBallSpriteCount())
        {
            for(int i = 0; i < shopButtons.Count; i++)
            {
                shopButtons[i].transform.Find("Ball").GetComponent<Image>().sprite = BallTextureHolder.GetShopBallSprite(i);
            }

            /*
            int index = 0;
            foreach(Transform t in shopScrollContent.transform.Cast<Transform>().OrderBy(pref => System.Convert.ToInt32(pref.name)))
            {
                Debug.Log(t.name);
                t.Find("Ball").GetComponent<Image>().sprite = ProfileDataBase.main.GetBallResource(index);
                index++;
            }
            */
        }
        else
        {
            // TODO: Implement what happens if the shop for some reason has less or more prefabs than we have balls.
        }
    }

    #region ColoringButtonStates
    // This function sets unfocused unlocked or unfocused locked.
    private void SetNotSelectedShopPrefabColorAndCost(int index)
    {
        if (ProfileDataBase.main.IsBallUnLocked(index))
        {
            shopButtons[index].transform.Find("Frame").GetComponent<Image>().color = unlockedColor;
            shopButtons[index].transform.Find("Price").gameObject.SetActive(false);
        }
        else
        {
            shopButtons[index].transform.Find("Frame").GetComponent<Image>().color = lockedColor;


            Transform priceTransform = shopButtons[index].transform.Find("Price");
            Currency cost = ProfileDataBase.main.GetBallCost(index);

            priceTransform.GetComponent<Text>().text = cost.amount.ToString();
            priceTransform.GetComponentInChildren<Image>().sprite = cost.Icon;
        }

        Animator animator = shopButtons[index].transform.Find("Ball").GetComponent<Animator>();
        if (shopButtons[index].activeInHierarchy)
        {
            animator.PlayInFixedTime("startBouncing", 0, 0f);
        }
        animator.speed = 0;
    }

    private void RefreshSelectedBallColor()
    {
        Transform selectedTransform = shopButtons[ProfileDataBase.main.GetSelectedBallIndex()].transform;

        selectedTransform.Find("Frame").GetComponent<Image>().color = selectedColor;
        selectedTransform.GetComponentInChildren<Animator>().speed = 1;

        checkMark.SetActive(true);
        checkMark.transform.SetParent(selectedTransform);
        //checkMark.GetComponent<RectTransform>().anchoredPosition = new Vector2(selectedTransform.position.x, selectedTransform.position.y + checkMarkOffsetY);
        checkMark.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, checkMarkOffsetY);
    }

    private void RefreshAllShownBallStates()
    {
        for (int i = 0; i < shopButtons.Count; i++)
        {
            SetNotSelectedShopPrefabColorAndCost(i);
        }
        RefreshSelectedBallColor();
    }
    #endregion

    #region RefreshValues
    private void SelectBall(int index)
    {
        SetNotSelectedShopPrefabColorAndCost(ProfileDataBase.main.GetSelectedBallIndex());

        ProfileDataBase.main.SelectBall(index);
        RefreshSelectedBallColor();
    }

    public void UpdateStarAmountOnCurrentLevelButton()
    {
        // HACK : There is no specific way atm.
        lvlLoopScrollRect.RefreshCells();
    }

    public void RefreshLastShownLevelButtons()
    {
        // HACK : There is no specific way atm.
        lvlLoopScrollRect.RefreshCells();
    }

    private void RefreshShownCurrencyAmount()
    {
        string gold = ProfileDataBase.main.GetCurrentGold().ToString();
        string diamond = ProfileDataBase.main.GetCurrentDiamondApples().ToString();

        goldText_Shop.text = gold;
        goldText_Profile.text = gold;
        diamondText_Shop.text = diamond;
    }

    private void RefreshShownStarAmount()
    {
        string stars = ProfileDataBase.main.GetCurrentStars().ToString();

        starText_Profile.text = stars;
    }

    private void RefreshShownName()
    {
        string name = ProfileDataBase.main.GetName();
        nameText_Profile.text = name;
    }

    private void RefreshShownExperience()
    {
        string xp = ProfileDataBase.main.GetExperienceOnCurrentLevel().ToString() + 
            "/" + ProfileDataBase.main.GetExperienceRequiredOnCurrentLevel();
        string lvl = ProfileDataBase.main.GetCurrentLevel().ToString();

        xpText_Profile.text = xp;
        lvlText_Profile.text = lvl;
    }

    private void RefreshShownStats()
    {
        ProfileDataBase.main.StatIncreaseTimeSpent();
        string stats = ProfileDataBase.main.GetStatisticString(out int lineCount);

        statList_Profile.text = stats;
        int fontSize = statList_Profile.fontSize;

        statList_Profile.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (fontSize + 5) * lineCount);

        statListUpdatedNumber_Profile.text = ProfileDataBase.main.GetTimeSpent().ToString("F1");
        nextActivationTime = Time.time + freqInteractiveItemUpdate;
    }

    private void UpdateInteractiveStats()
    {
        if (isInteractive && isInteractiveItemActive)
        {
            if(nextActivationTime <= Time.time)
            {
                nextActivationTime = Time.time + freqInteractiveItemUpdate;

                ProfileDataBase.main.StatIncreaseTimeSpent();
                statListUpdatedNumber_Profile.text = ProfileDataBase.main.GetTimeSpent().ToString("F1");
            }
        }
    }

    private void RefreshAllShownValues()
    {
        RefreshShownCurrencyAmount();
        RefreshShownStarAmount();
        RefreshShownName();
        RefreshShownExperience();
        RefreshShownStats();
    }
    #endregion


    private void SetDefaultStateColorsAndCheckMark()
    {
        // TODO: Obviusly, these are not the refined final colors. (states of shop elements)
        selectedColor = Color.yellow;
        lockedColor = Color.white;
        unlockedColor = Color.gray;

        checkMark.GetComponent<Image>().color = selectedColor;
        checkMark.SetActive(true);
    }

    public int GetCurrentLvl()
    {
        return currentLvl;
    }

    public bool IsNamedMap()
    {
        return IsNamedMap(out _);
    }

    public bool IsNamedMap(out Stage level)
    {
        level = StageLayout.GetMain().GetStage(currentLvl);
        if (level != null && level.IsNamed()){
            return true;
        }
        else
        {
            return false;
        }
    }


    #region ScreenManipulation
    public void SetShopActive(bool value)
    {
        if (value)
        {
            SetPanelActive(shopPanel);
        }
        else
        {
            SetPanelActive(menuPanel);
        }
    }

    public void SetProfileActive(bool value)
    {
        if (value)
        {
            isInteractiveItemActive = true;
            SetPanelActive(profilePanel);
        }
        else
        {
            isInteractiveItemActive = false;
            SetPanelActive(menuPanel);
        }
    }

    public void SetSettingsActive(bool value)
    {
        if (value)
        {
            SetPanelActive(settingsPanel);
        }
        else
        {
            ProfileDataBase.main.SaveData();
            SetPanelActive(menuPanel);
        }
    }

    private void SetPanelActive(GameObject panel)
    {
        foreach(GameObject p in panels)
        {
            p.SetActive(false);
        }

        panel.SetActive(true);

        RefreshAllShownValues();
    }

    public void SetHomeScreen(bool comingFromActiveLevel = false)
    {
        if (comingFromActiveLevel)
        {
            OnLevelQuit?.Invoke(); 
        }
        OnBeforeLoadingMenuScene?.Invoke();

        SceneManager.LoadScene(0);
        canvas.SetActive(true);
        UpdateStarAmountOnCurrentLevelButton();
    }

    public void SetGameScreen()
    {
        OnBeforeLoadingGameScene?.Invoke();

        SceneManager.LoadScene(1);
        canvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void SetMapEditorScreen()
    {
        OnBeforeLoadingMapEditorScene?.Invoke();

        SceneManager.LoadScene(2);
        canvas.SetActive(false);
    }

    public void SetDefaultState()
    {
        canvas.SetActive(true);

        SetShopActive(false);

        SetDefaultStateColorsAndCheckMark();

        GenerateShopPrefabs(BallTextureHolder.GetBallSpriteCount());
        ApplyTextureToShopPrefabs();

        SetLvlIconsToDefault();

        RefreshAllShownBallStates();
    }

    private void SetLvlIconsToDefault()
    {
        // It refills itself on Start(), so we just position it here.
        lvlLoopScrollRect.JumpTo(ProfileDataBase.main.GetLastPlayableMap(), true);
    }
    #endregion
}
