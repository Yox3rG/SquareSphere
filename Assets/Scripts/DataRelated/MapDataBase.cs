using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapDataBase : MonoBehaviour
{
    public static MapDataBase main = null;

    private Map currentMap = new Map();

    // Created in Start.
    private Map defaultMap = new Map();

    private MapLayoutGenerator randomLayoutGenerator;
    private readonly float numberofBallsSQRTMultiplier = 3f;
    private readonly float minSquareHPMultiplier = .8f;
    private readonly float maxSquareHPMultiplier = 1.8f;

    private readonly float bossHpSQRTMultiplier = 10f;

    public int BossHp { get; private set; } = 0;
    public TopMostRow _topMostRows;
    public TopMostRow TopMostRows { get { return _topMostRows; } private set { value.CopyTo(ref _topMostRows); } }

    private readonly string fileLocation = "maps";
    private string fileName = "default";
    private readonly string fileExtension = ".json";
    private string mapFolder;

    public bool IsTesting { get; private set; }
    public bool IsDefault { get; private set; }
    
    private int currentIndex = 0;

    void Awake()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Setting up default (impossible) map.
        GenerateDefaultMap();

        randomLayoutGenerator = new MapLayoutGenerator(SquareDataBase.rows, SquareDataBase.cols);

        _topMostRows = new TopMostRow();
        //topMostRows = new TopMostRow(new[] { 0, 0, 0, 4, 4, 4, 4 });

        mapFolder = Path.Combine(Application.persistentDataPath, fileLocation);
        if (!Directory.Exists(mapFolder))
        {
            Directory.CreateDirectory(mapFolder);
        }
    }
    
    public int GetNumberOfDestroyables()
    {
        return currentMap.destroyableCount;
    }

    public int GetNumberOfBalls()
    {
        if (Menu.main != null)
        {
            if (Menu.main.IsNamedMap() || IsTesting)
            {
                return currentMap.numberOfBalls;
            }
            else
            {
                return StageNumberToNumberOfBalls(Menu.main.GetCurrentLvl());
            }
        }
        return 10;
    }

    private int StageNumberToNumberOfBalls(int stageNumber, bool includeBonuses = true)
    {
        int balls = 1 + Mathf.FloorToInt(Mathf.Sqrt(stageNumber) * numberofBallsSQRTMultiplier);

        if (AttributeConverter.Instance != null && includeBonuses)
        {
            balls = AttributeConverter.Instance.ApplyBonusToNumberOfBalls(balls);
        }

        return balls;
    }

    private int StageNumberToBossHp(int stageNumber, Type bossType)
    {

#if UNITY_EDITOR
        Debug.Log("type: " + bossType);
#endif
        if (bossType.IsAssignableFrom(typeof(SquidBoss)))
            return 0;
        // HACK double sqrt on the same value. ^ V
        int hp = 30 + Mathf.FloorToInt(Mathf.Sqrt(stageNumber) * bossHpSQRTMultiplier);

        return hp;
    }

    private void GenerateDefaultMap()
    {
        defaultMap = new Map();
        defaultMap.numberOfBalls = 1;
        defaultMap.lines = new List<SaveElementLine>();
        defaultMap.lines.Add(new SaveElementLine());
        defaultMap.lines[0].list = new List<SaveElement>();
        for (int i = 0; i < 10; i++)
        {
            defaultMap.lines[0].list.Add(new SaveElement(Element.ElementType.SQUARE, 0, i, new SaveDestroyable(6969, 0)));
        }
    }


    public bool LoadMapFromStage(Stage stage)
    {
        TopMostRows.Reset();
        if(stage is SeededStage)
        {
            int seed = (stage as SeededStage).stageSeed;
            TopMostRows = stage.GetTopMostRows();
            int numOfBalls = StageNumberToNumberOfBalls(stage.number, includeBonuses: false);
            randomLayoutGenerator.SetHpPool(Mathf.FloorToInt(numOfBalls * minSquareHPMultiplier), Mathf.FloorToInt(numOfBalls * maxSquareHPMultiplier));
            GenerateRandomMap(seed);

            if (stage is BossStage)
                BossHp = StageNumberToBossHp(stage.number, (stage as BossStage).BossType);
            else
                BossHp = 0;

            return true;
        }
        else if(stage is NamedStage)
        {
            SetFileName((stage as NamedStage).stagePath);
            LoadMap();
            return true;
        }

        return false;
    }


    #region FileRelated
    public void LoadDefaultMap()
    {
        currentMap = defaultMap;
        // Remove this later..
        currentMap = new Map();
        currentIndex = 0;
    }

    public bool LoadMap()
    {
        return LoadMap(GetCurrentFilePath());
    }

    // These ( \/ /\ ) for some reason does not change the stored file name.. Something to consider later.
    private bool LoadMap(string filePath)
    {
        bool loadOK = SaveLoadHandlerJSON<Map>.Load(filePath, out currentMap);
        IsDefault = false;

        currentIndex = 0;

        if (!loadOK)
        {
            IsDefault = true;
            LoadDefaultMap();
        }

        if(currentMap.destroyableCount <= 0)
        {
            currentMap.destroyableCount = CountDestroyables();
        }

        return loadOK;
    }

    public string GetFileName()
    {
        return fileName;
    }

    public void SetFileName(string fileName)
    {
        this.fileName = fileName;
    }

    public string GetCurrentFilePath()
    {
        return Path.Combine(fileLocation, fileName + fileExtension);
    }

    public List<string> GetAvailableMapList()
    {
        List<string> fileNames = new List<string>();

        DirectoryInfo info = new DirectoryInfo(mapFolder);
        FileInfo[] files = info.GetFiles("*.json");

        foreach(FileInfo file in files)
        {
            fileNames.Add(Path.GetFileNameWithoutExtension(file.FullName));
        }

        return fileNames;
    }
    #endregion

    #region RandomGeneration
    public void GenerateRandomMap(int seed)
    {
        currentMap = randomLayoutGenerator.CreateMap(seed);
        currentIndex = 0;
    }
    #endregion

    #region Map
    public void InitiateTesting()
    {
        IsTesting = true;
        LoadMap();
    }

    public void StopTesting()
    {
        IsTesting = false;
    }

    public List<SaveElement> GetNextLine()
    {
        if(currentIndex < currentMap.lines.Count)
        {
            //Debug.Log("Row: " + currentIndex);
            return currentMap.lines[currentIndex++].list;
        }
        else
        {
            Debug.Log("Map over, Row: " + currentIndex);
            return new List<SaveElement>();
        }
    }

    private int CountDestroyables()
    {
        int count = 0;

        for (int i = 0; i < currentMap.lines.Count; i++)
        {
            count += currentMap.lines[i].list.Count;
        }
        
        return count;
    }

    public int GetMapLength()
    {
        return currentMap.lines.Count;
    }

    /*
    private void Test()
    {
        currentMap = new Map();
        for (int i = 0; i < 10; i++)
        {
            currentMap.lines.Add(new SaveElementLine());
            currentMap.lines[i].list.Add(new SaveElement(Element.Type.SQUARE, 1, 1));
            currentMap.lines[i].list.Add(new SaveElement(Element.Type.TRIANGLE_180, 3, 1));
            currentMap.lines[i].list.Add(new SaveElement(Element.Type.TRIANGLE_90, 4, 1));
                        
            currentMap.lines[i].list.Add(new SaveElement(Element.Type.POWERUP_BOTH_LINE, 1, 2));
        }
        currentIndex = 0;
    }
    */
    #endregion
}
