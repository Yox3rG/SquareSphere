using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator main { get; private set; } = null;

    public GameObject placeHolderPrefab;

    public GameObject endlineTop;
    public GameObject endlineBottom;

    private Map map = new Map();

    private List<PlaceHolder[]> placeHolders = new List<PlaceHolder[]>();

    public const int defaultMapLength = 10;
    private int mapLength = defaultMapLength;
    private int maxCols = 10;
    public int numOfBalls { get; private set; } = 10;

    public float startPositionOffsetY = 3;
    private int topRowIndex = 0;
    private int bottomRowIndex = 0;
    private Vector3 startPosition;
    private float squareSize;

    // Handling interactions.
    public SaveElement currentTool { get; private set; }
    public bool isTrashCanOpen { get; private set; }

    public bool isPaintingHp { get; private set; } = false;
    public bool isPaintingColor { get; private set; } = false;
    public int currentMaxHp { get; private set; } = 10;
    public byte currentColorIndex { get; private set; }

    void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }

    void Start()
    {
        squareSize = ElementHandler.Instance.DefaultSquareSize;
        float sideSize = ElementHandler.Instance.DefaultSideSize;
        mapLength = defaultMapLength;

        currentColorIndex = Palette.DefaultColorIndex;
        isTrashCanOpen = false;

        startPosition = new Vector3(-Camera.main.orthographicSize * Camera.main.aspect + squareSize / 2 + sideSize,
            -Camera.main.orthographicSize + (squareSize / 2 + sideSize) + startPositionOffsetY, 0);

        SetDefaultTool();

        //StartEditing();
    }

    // HACK debug.
    private void DebugFunction()
    {
        SetToolType(PowerUp.ElementType.POWERUP_DINO);
        placeHolders[2][5].SetBlockAndLook();
        placeHolders[2][5].SetBlockAndLook();
        SetToolType(Destroyable.ElementType.SQUARE);
        placeHolders[3][7].SetBlockAndLook();
        placeHolders[8][2].SetBlockAndLook();
    }
    

    #region PlaceHolders
    public void ResetAllTextOnElements()
    {
        foreach (PlaceHolder[] pArray in placeHolders)
        {
            foreach (PlaceHolder p in pArray)
            {
                if(p != null && p.IsElementDestroyable())
                {
                    ((Destroyable)p.GetLook()).ResetTextPosition();
                }
            }
        }
    }

    public void DeletePlaceHolders()
    {
        int countBeforeDelete = placeHolders.Count;
        for (int i = 0; i < countBeforeDelete; i++)
        {
            RemoveRowFrom(MapPosition.TOP);
        }
        //Debug.Log(topRowIndex + bottomRowIndex);


        bottomRowIndex = topRowIndex = 0;
        RepositionEndLines();
    }

    public void AddPlaceHolders(int mapLength)
    {
        for (int i = placeHolders.Count; i < mapLength; i++)
        {
            AddPlaceHolderRow(MapPosition.TOP);
        }
    }

    private void AddPlaceHolderRow(MapPosition position)
    {
        int listIndex = 0;
        int positionIndex = (position == MapPosition.TOP) ? topRowIndex : bottomRowIndex;
        if(position == MapPosition.TOP)
        {
            if(placeHolders.Count != 0)
            {
                positionIndex = ++topRowIndex;
            }
            listIndex = placeHolders.Count;
            placeHolders.Add(new PlaceHolder[maxCols]);
        }
        else
        {
            if (placeHolders.Count != 0)
            {
                positionIndex = --bottomRowIndex;
            }
            listIndex = 0;
            placeHolders.Insert(listIndex, new PlaceHolder[maxCols]);
        }
        for (int j = 0; j < maxCols; j++)
        {
            Vector3 pos = startPosition + new Vector3(j, positionIndex) * squareSize;
            
            placeHolders[listIndex][j] = GeneratePlaceHolder(pos, listIndex, j);
        }

        RepositionEndLines();
    }

    private void RemovePlaceHolderRow(MapPosition position)
    {
        int listIndex;
        //Debug.Log("Top: " + topRowIndex + " Bottom: " + bottomRowIndex);
        if(placeHolders.Count < 1)
        {
            return;
        } 
        else if(placeHolders.Count == 1)
        {
            listIndex = 0;
        }
        else if (position == MapPosition.TOP)
        {
            listIndex = placeHolders.Count - 1;
            topRowIndex--;
        }
        else
        {
            listIndex = 0;
            bottomRowIndex++;
        }

        //Debug.Log("After\nTop: " + topRowIndex + " Bottom: " + bottomRowIndex);

        foreach (PlaceHolder p in placeHolders[listIndex])
        {
            Destroy(p.gameObject);
        }
        placeHolders.RemoveAt(listIndex);

        RepositionEndLines();
    }

    private PlaceHolder GeneratePlaceHolder(Vector3 position, int row, int col)
    {
        GameObject g = Instantiate(placeHolderPrefab, position, Quaternion.identity);

        PlaceHolder p = g.GetComponent<PlaceHolder>();
        p.SetRowAndCol(row, col);

        return p;
    }

    //TODO: Placeholder endlines unfinished.
    private void RepositionEndLines()
    {
        endlineTop.transform.position = new Vector3(0, startPosition.y + squareSize * (topRowIndex + .5f), 20);
        // HACK: It works, investigate why if bored...
        endlineBottom.transform.position = new Vector3(0, startPosition.y + squareSize * (bottomRowIndex + -.5f), 20);
    }


    #endregion

    #region UIfunctions
    public void DoCurrentActionOn(PlaceHolder placeHolder)
    {
        if (isTrashCanOpen)
        {
            placeHolder.DeleteElementAndVisuals();
        }
        else if (isPaintingHp)
        {
            placeHolder.SetHp(currentMaxHp);
        }
        else if (isPaintingColor)
        {
            placeHolder.SetColor(currentColorIndex);
        }
        else
        {
            placeHolder.SetBlockAndLook();
        }
    }

    public void StartEditing()
    {
        DeletePlaceHolders();
        AddPlaceHolders(mapLength);
    }

    public void AddRowTo(MapPosition mapPos)
    {
        AddPlaceHolderRow(mapPos);
    }

    public void RemoveRowFrom(MapPosition pos)
    {
        RemovePlaceHolderRow(pos);
    }

    public void ReceiveNumOfBalls(int numOfBalls)
    {
        this.numOfBalls = numOfBalls;
    }

    public bool InvertHpPainting()
    {
        isPaintingHp = !isPaintingHp;
        return isPaintingHp;
    }

    public void ReceiveMaxHp(int maxHp)
    {
        this.currentMaxHp = maxHp;
        RefreshCurrentTool();
    }

    public void ReceiveColor(byte color)
    {
        currentColorIndex = color;
        RefreshCurrentTool();

        isPaintingColor = true;
    }

    public void SetToolType(Element.ElementType type)
    {
        if (Element.IsDestroyable(type))
        {
            currentTool.destroyable.maxHp = currentMaxHp;
            currentTool.destroyable.colorIndex = currentColorIndex;
        }

        isPaintingColor = false;
        currentTool.type = type;
    }

    public bool InvertTrashCan()
    {
        isTrashCanOpen = !isTrashCanOpen;
        return isTrashCanOpen;
    }

    public void CloseTrashCan()
    {
        isTrashCanOpen = false;
    }

    public void SaveMap(string fileName)
    {
        BuildMapToSave();
        SaveLoadHandlerJSON<Map>.Save(map, fileName);
    }

    public void LoadMap(string fileName)
    {
        bool mapOK = SaveLoadHandlerJSON<Map>.Load(fileName, out map);
        BuildMapFromLoad();
    }

    public void SetMapLength(int length)
    {
        mapLength = length;
    }

    public int GetMapLength()
    {
        return placeHolders.Count;
    }
    #endregion


    #region BackEndToUI
    

    private void SetDefaultTool()
    {
        currentTool = new SaveElement(Element.ElementType.SQUARE, 0, 0, new SaveDestroyable(currentMaxHp, Palette.DefaultColorIndex));
    }

    private void RefreshCurrentTool()
    {
        if (Element.IsDestroyable(currentTool.type))
        {
            SetToolType(currentTool.type);
        }
    }

    private void BuildMapToSave()
    {
        map = new Map();

        // Variables.
        // HACK: [numOfBalls] map might not be a good place to store the number of balls on.
        map.numberOfBalls = numOfBalls;
        
        // Maplayout.
        Vector2Int usefulLines = UsefulLinesFromX2Y();
        int start = usefulLines.x;
        int end = usefulLines.y;
        

        for (int i = start, newIndex = 0; i < end; i++, newIndex++)
        {
            map.lines.Add(new SaveElementLine());
            for (int j = 0; j < placeHolders[i].Length; j++)
            {
                if (placeHolders[i][j].saveElement != null)
                {
                    SaveElement element = new SaveElement(placeHolders[i][j].saveElement);

                    if (!Element.IsExistingType(element.type))
                    {
                        Debug.LogError($"Trying to save element of unknown type [{element.type}] at location ({element.row}, {element.col}).");
                        continue;
                    }

                    element.row = newIndex;
                    map.lines[newIndex].list.Add(element);

                    if (element.IsDestroyable)
                    {
                        map.destroyableCount++;
                    }
                }
            }
        }
    }

    // TODO: Implement the loading mechanism for MapGenerator.
    private void BuildMapFromLoad()
    {
        DeletePlaceHolders();
        mapLength = map.lines.Count;
        numOfBalls = map.numberOfBalls;
        AddPlaceHolders(mapLength);

        for (int i = 0; i < mapLength; i++)
        {
            List<SaveElement> saveElements = map.lines[i].list;

            foreach (SaveElement s in saveElements)
            {
                if (!Element.IsExistingType(s.type))
                {
                    Debug.LogError($"Trying to load element of unknown type [{s.type}] at location ({s.row}, {s.col}).");
                    continue;
                }

                if (s.destroyable != null)
                {
                    ReceiveMaxHp(s.destroyable.maxHp);
                    ReceiveColor(s.destroyable.colorIndex);
                }
                SetToolType(s.type);

                placeHolders[s.row][s.col].SetBlockAndLook();
            }
        }
    }
    
    private Vector2Int UsefulLinesFromX2Y()
    {
        Vector2Int useful = new Vector2Int(placeHolders.Count, 0);

        Debug.Log("x: " + useful.x);
        Debug.Log("y: " + useful.y);

        for (int i = 0; i < placeHolders.Count; i++)
        {
            if (!isRowEmpty(placeHolders[i]))
            {
                useful.x = i;
                break;
            }
        }
        for (int j = placeHolders.Count - 1; j >= useful.x; j--)
        {
            if (!isRowEmpty(placeHolders[j]))
            {
                useful.y = j + 1;
                break;
            }
        }

        Debug.Log("x: " + useful.x);
        Debug.Log("y: " + useful.y);
        return useful;
    }

    private bool isRowEmpty(PlaceHolder[] pHolders)
    {
        for (int i = 0; i < pHolders.Length; i++)
        {
            if(pHolders[i] != null && pHolders[i].IsElementSet())
            {
                return false;
            }
        }
        return true;
    }
    #endregion


    public enum MapPosition
    {
        TOP,
        BOTTOM
    }
}
