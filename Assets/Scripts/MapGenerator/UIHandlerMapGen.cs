using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHandlerMapGen : MonoBehaviour
{
    // New panels and buttons should be added into the fill method.
    public Button propertiesTabButton;
    public Button buildTabButton;
    public Button paletteTabButton;

    // For mouse over UI calculations.
    public RectTransform topPanel;
    public RectTransform bottomPanel;


    public GameObject propertiesPanel;
    public GameObject buildPanel;
    public GameObject palettePanel;

    public GameObject popupWindow;
    public GameObject newMapPanel;
    public GameObject quitPanel;
    public GameObject loadMapPanel;


    public enum PopupPanelType
    {
        NEWMAP, QUIT, LOAD
    }

    public GameObject blockContent;
    public GameObject powerUpContent;

    public InputField hpInputField;
    public InputField numOfBallsInputField;
    public InputField fileNameField;
    public InputField mapLengthField;
    public InputField currentShownFileNameField;

    public ScrollRect existingFilesScrollrect;
    public GameObject existingFileButton;

    public Text mapLengthText;

    private List<Button> tabs = new List<Button>();
    private List<GameObject> panels = new List<GameObject>();

    private List<Button> blockButtons = new List<Button>();
    private List<Button> powerUpButtons = new List<Button>();
    public Transform elementSelectionFrame;

    private List<Image> paletteButtonImages = new List<Image>();
    private List<Color32> paletteColors = new List<Color32>();
    private int currentSelectedColorIndex = 0;
    private const string paletteFile = "palette.json";

    public GameObject prefabPaletteButton;

    private int currentTab = 0;
    private Color32 activeTabColor;
    private Color32 passiveTabColor;

    //private const int uiLayer = 5;
    private const int placeHolderLayer = 11;
    private List<PlaceHolder> placeHoldersSetThisOperation = new List<PlaceHolder>();
    private bool isDrawing = false;
    private bool isDrawingToolInUse = true;

    public GameObject go_trashButton;
    private Image img_trashButton;
    public GameObject go_scrollHandButton;
    private Image img_scrollHandButton;

    private Sprite spr_trashClosed;
    private Sprite spr_trashOpened;
    private Sprite spr_handOpened;
    private Sprite spr_handClosed;


    private string fileName = "default";

    private float topPanelLowestY;
    private float bottomPanelHeight;

    void Start()
    {
        SetDefaultAppearenceAndFunctionality();
        
        topPanelLowestY = Screen.height - topPanel.rect.height;
        bottomPanelHeight = bottomPanel.rect.height;

        if (MapDataBase.main != null && MapDataBase.main.IsTesting)
        {
            MapDataBase.main.StopTesting();
            DisableAllPopupWindowPanels();
            fileNameField.text = MapDataBase.main.GetFileName();
            Load();
        }

        UpdateShownMapRelatedData();
    }

    void Update()
    {
        if (isDrawingToolInUse)
        {
            WaitingForTouch();
            WaitingForMouse();
        }
    }

    private void SetDefaultAppearenceAndFunctionality()
    {
        SetDefaultTabColors();
        FillTabsAndPanels();

        SetPaletteColors();
        GenerateButtonsAndAssignOnClickToPalette();

        AssignOnClickToDestroyables();
        AssignOnClickToPowerUps();

        GetResourcesAndSetDefaultSpritesOnInvertables();
        SetDefaultTabAndPanelState();

        SetDefaultSelectedElementAndColor();
    }

    #region ElementCreating
    private void HandleTarget(RaycastHit2D hit)
    {
        PlaceHolder placeHolder = hit.transform.GetComponent<PlaceHolder>();
        if (placeHolder != null && placeHolder.isSetThisOperation == false)
        {
            placeHolder.isSetThisOperation = true;
            placeHoldersSetThisOperation.Add(placeHolder);

            MapGenerator.main.DoCurrentActionOn(placeHolder);
        }
    }
    private void WaitingForTouch()
    {
        if(Input.touchCount > 0)
        {
            Touch firstTouch = Input.GetTouch(0);

            if (firstTouch.phase == TouchPhase.Began)
            {
                isDrawing = true;
            }
            else if (firstTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled)
            {
                isDrawing = false;
                foreach (PlaceHolder p in placeHoldersSetThisOperation)
                {
                    p.isSetThisOperation = false;
                }
                placeHoldersSetThisOperation.Clear();
            }

            if (isDrawing && !IsTouchOverUI(firstTouch))
            {
                RaycastHit2D hit = CastRayOnTouch(firstTouch);
                if (hit.collider != null)
                {
                    HandleTarget(hit);
                }
            }
        }
    }
    private bool IsTouchOverUI(Touch touch)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, results);
        return results.Count != 0;
    }
    private RaycastHit2D CastRayOnTouch(Touch touch)
    {
        return CastRayFromScreenPosition(touch.position);
    }

    private void WaitingForMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            foreach (PlaceHolder p in placeHoldersSetThisOperation)
            {
                p.isSetThisOperation = false;
            }
            placeHoldersSetThisOperation.Clear();
        }
        else if(Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            if (isDrawing && !IsMouseOverUI(mousePosition))
            {
                RaycastHit2D hit = CastRayOnMouse(mousePosition);
                if (hit.collider != null)
                {
                    HandleTarget(hit);
                }
            } 
        }
    }

    private bool IsMouseOverUI(Vector2 mousePosition)
    {
        if(mousePosition.y < bottomPanelHeight || mousePosition.y > topPanelLowestY)
        {
            return true;
        }
        return false;
    }

    private RaycastHit2D CastRayOnMouse(Vector2 mousePosition)
    {
        return CastRayFromScreenPosition(mousePosition);
    }

    private RaycastHit2D CastRayFromScreenPosition(Vector2 start, int layerMask = (1 << placeHolderLayer))
    {
        start = Camera.main.ScreenToWorldPoint(start);
        Vector2 direction = Vector2.up;

        RaycastHit2D hit = Physics2D.Raycast(start, direction, .2f, layerMask);
        Debug.DrawRay(start, direction, Color.cyan);

        return hit;
    }
    #endregion


    #region onButtonClicks
    
    public void Save()
    {
        MapDataBase.main.SetFileName(fileName);

        MapGenerator.main.SaveMap(MapDataBase.main.GetCurrentFilePath());
    }

    public void Load()
    {
        UpdateFileName();
        MapDataBase.main.SetFileName(fileName);

        MapGenerator.main.LoadMap(MapDataBase.main.GetCurrentFilePath());

        SetDefaultSelectedElementAndColor();
        MapGenerator.main.ReceiveMaxHp(10);

        EnableSaveLoadPopUpWindow(false);
        UpdateShownMapRelatedData();
    }

    public void CreateNewMap()
    {
        UpdateFileName();
        MapDataBase.main.SetFileName(fileName);

        int mapLength = GuaranteePositiveFromInput(mapLengthField, 10);
        MapGenerator.main.SetMapLength(mapLength);
        MapGenerator.main.StartEditing();

        EnableSaveLoadPopUpWindow(false);
        UpdateShownMapRelatedData();
    }

    public void StartTesting()
    {
        Save();
        MapDataBase.main.InitiateTesting();
        if (Menu.main)
        {
            Menu.main.SetGameScreen();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Menu is not present, can't start testing.");
#endif
        }
    }

    public void SetHomeScreen()
    {
        if (Menu.main != null)
        {
            Menu.main.SetHomeScreen();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void ShowNewMapPanel()
    {
        ShowPanel(PopupPanelType.NEWMAP);
    }

    public void ShowLoadMapPanel()
    {
        ShowPanel(PopupPanelType.LOAD);
    }

    public void ShowQuitPanel()
    {
        ShowPanel(PopupPanelType.QUIT);
    }

    public void CancelPopupWindow()
    {
        EnableSaveLoadPopUpWindow(false);
    }

    public void InvertPaintingHp(Text callerText)
    {
        bool isPaintingHp = MapGenerator.main.InvertHpPainting();

        Color newColor = isPaintingHp ? Color.gray : Color.white;
        callerText.color = newColor;

        CloseTrashCan();
    }

    public void InvertCameraMovement()
    {
        isDrawingToolInUse = !isDrawingToolInUse;
        CameraMovement.main.InvertCameraMovementTrigger();

        Sprite currentHandSprite = isDrawingToolInUse ? spr_handOpened : spr_handClosed;
        img_scrollHandButton.sprite = currentHandSprite;
    }

    public void InvertTrashCan()
    {
        bool isTrashCanOpen = MapGenerator.main.InvertTrashCan();

        Sprite currentTrashSprite = isTrashCanOpen ? spr_trashOpened : spr_trashClosed;
        img_trashButton.sprite = currentTrashSprite;
    }

    public void CloseTrashCan()
    {
        MapGenerator.main.CloseTrashCan();
        img_trashButton.sprite = spr_trashClosed;
    }

    public void ChangeTabTo(int newTab)
    {
        if(newTab != currentTab && newTab >= 0 && newTab < tabs.Count)
        {
            SetTabAndPanelActive(currentTab, false);

            SetTabAndPanelActive(newTab, true);

            currentTab = newTab;
        }

        blockContent.GetComponent<RectTransform>().anchoredPosition.Set(0, 0);


    }

    public void AddRowToTop()
    {
        MapGenerator.main.AddRowTo(MapGenerator.MapPosition.TOP);
        RefreshShownMapLengthText();
    }

    public void AddRowToBottom()
    {
        MapGenerator.main.AddRowTo(MapGenerator.MapPosition.BOTTOM);
        RefreshShownMapLengthText();
    }

    public void RemoveRowFromTop()
    {
        MapGenerator.main.RemoveRowFrom(MapGenerator.MapPosition.TOP);
        RefreshShownMapLengthText();
    }

    public void RemoveRowFromBottom()
    {
        MapGenerator.main.RemoveRowFrom(MapGenerator.MapPosition.BOTTOM);
        RefreshShownMapLengthText();
    }

    // Assigned via script, not found in the editor.
    public void ChangeColor(int index)
    {
        try
        {
            MapGenerator.main.ReceiveColor((byte)index);

            paletteButtonImages[currentSelectedColorIndex].color = paletteColors[currentSelectedColorIndex];

            Color newColor = paletteButtonImages[index].color;
            newColor *= .6f;
            newColor.a = 1f;
            paletteButtonImages[index].color = newColor;

            currentSelectedColorIndex = index;

            CloseTrashCan();
        }
        catch(System.Exception e)
        {
            Debug.Log("Palette buttons count, and count of colors might not match.\n" + e);
        }
    }

    public int ChangeElement(Element.ElementType type, Transform caller)
    {
        Debug.Log(type);
        MapGenerator.main.SetToolType(type);

        elementSelectionFrame.SetParent(caller, false);
        //elementSelectionFrame.position = caller.position;

        CloseTrashCan();
        return 0;
    }
    #endregion

    #region onTextChanges
    public void OnNumberOfBallsChanged()
    {
        int numOfBalls = MapGenerator.main.numOfBalls;
        numOfBalls = GuaranteePositiveFromInput(numOfBallsInputField, numOfBalls);

        MapGenerator.main.ReceiveNumOfBalls(numOfBalls);
    }

    public void OnMaxHpChanged()
    {
        int maxHp = MapGenerator.main.currentMaxHp;
        maxHp = GuaranteePositiveFromInput(hpInputField, maxHp);

        MapGenerator.main.ReceiveMaxHp(maxHp);
    }

    public void OnNumOfBallsEditEnd()
    {
        RefreshShownNumOfBalls();
    }

    public void OnMaxHpEditEnd()
    {
        RefreshShownMaxHp();
    }

    private void RefreshShownNumOfBalls()
    {
        numOfBallsInputField.text = MapGenerator.main.numOfBalls.ToString();
    }

    private void RefreshShownMaxHp()
    {
        hpInputField.text = MapGenerator.main.currentMaxHp.ToString();
    }

    private void RefreshShownMapLengthText()
    {
        mapLengthText.text = MapGenerator.main.GetMapLength().ToString();
    }

    private void RefreshCurrentShownMapName()
    {
        currentShownFileNameField.text = fileName;
    }
    #endregion



    #region Preparation
    private void UpdateShownMapRelatedData()
    {
        RefreshShownNumOfBalls();
        RefreshShownMaxHp();
        RefreshShownMapLengthText();
        RefreshCurrentShownMapName();
    }

    private void SetDefaultTabAndPanelState()
    {
        currentTab = 0;
        SetTabAndPanelActive(currentTab, true);

        for (int i = 1; i < tabs.Count; i++)
        {
            SetTabAndPanelActive(i, false);
        }
    }

    private void SetDefaultTabColors()
    {
        passiveTabColor = new Color32(105, 112, 130, 255);
        activeTabColor = new Color32(70, 82, 116, 255);
    }
    
    private void SetPaletteColors()
    {
        paletteColors = Palette.GetDefaultColors();
    }

    private void SetDefaultSelectedElementAndColor()
    {
        // Last color is default.
        ChangeColor(paletteColors.Count - 1);

        // First element is default.
        ChangeElement(Element.ElementType.SQUARE, blockButtons[0].transform);
    }
    
    private void GenerateButtonsAndAssignOnClickToPalette()
    {
        //Button[] buttons = palettePanel.GetComponentsInChildren<Button>();
        Sprite spritePaletteBlock = Resources.Load<Sprite>("paletteBlock");
        Transform trf_paletteGrid = palettePanel.transform.GetChild(0).transform;

        for (int i = 0; i < paletteColors.Count; i++)
        {
            GameObject g = Instantiate(prefabPaletteButton, trf_paletteGrid);
            g.name = "btn_Palette_" + i;
            Image sr = g.GetComponent<Image>();
            sr.sprite = spritePaletteBlock;
            sr.color = paletteColors[i];

            paletteButtonImages.Add(sr);

            int index = i;
            g.GetComponent<Button>().onClick.AddListener(delegate { ChangeColor(index); });
        }
    }

    private void AssignOnClickToDestroyables()
    {
        blockButtons = new List<Button>(blockContent.GetComponentsInChildren<Button>());
        AssignOnClickToButtonsForElements(blockButtons, Element.destroyableStart, Element.destroyableEnd, ChangeElement);
    }

    private void AssignOnClickToPowerUps()
    {
        powerUpButtons = new List<Button>(powerUpContent.GetComponentsInChildren<Button>());
        AssignOnClickToButtonsForElements(powerUpButtons, Element.powerupStart, Element.powerupEnd, ChangeElement);
    }

    private void AssignOnClickToButtonsForElements(List<Button> buttons, int start, int end, System.Func<Element.ElementType, Transform, int> func)
    {
        if(end - start == buttons.Count)
        {
            for(int i = 0; i < buttons.Count; i++)
            {
                Element.ElementType type = (Element.ElementType)(i + start);
                int index = i;
                buttons[i].onClick.AddListener(delegate { func(type, buttons[index].transform); });
            }
        }
        else
        {
            Debug.Log("The amount of the buttons not equal to the amount of types.");
        }
    }

    private void GetResourcesAndSetDefaultSpritesOnInvertables()
    {
        // Trash can
        spr_trashClosed = Resources.Load<Sprite>("trash");
        spr_trashOpened = Resources.Load<Sprite>("trashOpened");
        img_trashButton = go_trashButton.GetComponent<Image>();

        img_trashButton.sprite = spr_trashClosed;

        // Scroll hand
        spr_handClosed = Resources.Load<Sprite>("handClosed");
        spr_handOpened = Resources.Load<Sprite>("hand");
        img_scrollHandButton = go_scrollHandButton.GetComponent<Image>();

        img_scrollHandButton.sprite = spr_handOpened;

    }

    private void FillTabsAndPanels()
    {
        tabs.Add(propertiesTabButton);
        panels.Add(propertiesPanel);

        tabs.Add(buildTabButton);
        panels.Add(buildPanel);

        tabs.Add(paletteTabButton);
        panels.Add(palettePanel);
    }
    #endregion

    

    private void SetTabAndPanelActive(int index, bool value)
    {
        tabs[index].targetGraphic.color = GetActiveTabColor(value);
        panels[index].SetActive(value);
    }

    private Color32 GetActiveTabColor(bool value)
    {
        if (value)
        {
            return activeTabColor;
        }
        else
        {
            return passiveTabColor;
        }
    }

    #region SaveAndLoad
    private bool UpdateFileName()
    {
        if (fileNameField != null)
        {
            if (fileNameField.text != "")
            {
                this.fileName = fileNameField.text;
                return true;
            }
            else
            {
                this.fileName = "default";
            }
        }
        return false;
    }

    public void ShowPanel(PopupPanelType type)
    {
        EnableSaveLoadPopUpWindow(true);
        DisableAllPopupWindowPanels();
        switch (type)
        {
            case PopupPanelType.NEWMAP:
                newMapPanel.SetActive(true);
                EnableFileNameField(true);
                break;
            case PopupPanelType.LOAD:
                loadMapPanel.SetActive(true);

                ShowExistingFileNames();
                EnableFileNameField(true);
                break;
            case PopupPanelType.QUIT:
                quitPanel.SetActive(true);
                break;
        }
    }

    private void DisableAllPopupWindowPanels()
    {
        newMapPanel.SetActive(false);
        quitPanel.SetActive(false);
        loadMapPanel.SetActive(false);

        EnableFileNameField(false);
    }

    private void ShowExistingFileNames()
    {
        DeleteExistingFileSuggestions();
        List<string> available = MapDataBase.main.GetAvailableMapList();

        foreach(string file in available)
        {
            GameObject g = Instantiate<GameObject>(existingFileButton, existingFilesScrollrect.content);
            g.GetComponentInChildren<Text>().text = file;
            g.GetComponent<Button>().onClick.AddListener(delegate { FillFileNameField(file); });

        }
    }

    private void DeleteExistingFileSuggestions()
    {
        foreach(Transform t in existingFilesScrollrect.content.transform)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    public void FillFileNameField(string name)
    {
        fileNameField.text = name;
    }

    private void EnableFileNameField(bool value)
    {
        fileNameField.transform.parent.gameObject.SetActive(value);
    }

    private void EnableSaveLoadPopUpWindow(bool value)
    {
        popupWindow.SetActive(value);
    }
    #endregion

    private int GuaranteePositiveFromInput(InputField input, int currentValue)
    {
        int value;
        if (int.TryParse(input.text, out value))
        {
            // What a beautiful if statement. *.*
        }
        if (value <= 0)
        {
            value = currentValue;
        }

        return value;
    }
}
