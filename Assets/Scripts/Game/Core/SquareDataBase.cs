using System;
using System.Collections.Generic;
using UnityEngine;

public class SquareDataBase : MonoBehaviour
{
    public static SquareDataBase Instance { get; private set; } = null;
    
    public static event Action OnEndOfRound;
    public static event Action OnLevelFailed;

    private static int squareLayer = -2;

    public LastRowWarning warning;
    public BossFactory bossFactory;

    private Boss boss = null;

    private IWinningMechanism winningMechanism;
    private ILoosingMechanism loosingMechanism;

    // Gameplay Properties.
    private int maxSpawnRoundCount = 5;
    private int currentRound = 0;

    private int destroyablesGenerated = 0;

    public const int rows = 13, cols = 10;
    private int emptyRows = 5;
    public int LastRowIndex { get { return rows - 1; } }
    public int LastSafeRowIndex { get { return LastRowIndex - 1; } }
    private int LowestNotEmptyIndex { get { return rows - emptyRows - 1; } }

    private float squareSize;
    private Vector3 startPosition;
    private Element[,] elements;

    private Stack<Element[]> elementsPushedOutOfScreen = new Stack<Element[]>();
    // Preallocated arrays, to prevent garbage collection.
    private List<Element> neighbourElements = new List<Element>(8);
    private Destroyable[] destroyablesInCircle = new Destroyable[36];
    private Collider2D[] collidersInCircle = new Collider2D[36];
    private Destroyable[] destroyablesInLine = new Destroyable[Math.Max(rows, cols)];
    private RaycastHit2D[] raycastHitsInLine = new RaycastHit2D[Math.Max(rows, cols)];

    private System.Random randomGenerator;

    private Score score;
    private readonly float[] scorePercentages = { 0.3f, 0.5f, 0.8f };

    // HACK: not final position
    private Character character = new Character();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Hardcoded version in use atm.
        //startPosition = new Vector3(-Camera.main.orthographicSize * Camera.main.aspect + squareSize / 2 + sideSize, Camera.main.orthographicSize - squareSize / 2 - sideSize, 0);

        elements = new Element[rows, cols];
    }

    void Start()
    {
    }

    public void InitializeSquareDataBase()
    {
        if (squareLayer <= -2)
            squareLayer = 1 << LayerMask.NameToLayer("Square");

        squareSize = ScreenRelativeSizeCalculator.Instance.GetSquareSizeFromScreen(numberOfSquares: cols, out Vector3 squareScale);
        startPosition = ScreenRelativeSizeCalculator.Instance.GetTopLeftSquarePositionFromScreen(squareSize, rows);

        randomGenerator = new System.Random();

        loosingMechanism = new LM_NormalMode(this);

        if (MapDataBase.main)
        {
            Stage resultStage;

            // If we launched the testing from the Map Creator.
            if (!MapDataBase.main.IsTesting)
            {
                if (Menu.main != null)
                {
                    Menu.main.IsNamedMap(out resultStage);
                }
                else
                {
                    resultStage = new SeededStage(Stage.LayoutType.GENERATED, 0);
                }

                MapDataBase.main.LoadMapFromStage(resultStage);

#if UNITY_EDITOR
                Debug.Log("TopMostRow: " + MapDataBase.main.TopMostRows);
#endif

                if (resultStage is BossStage)
                {
                    boss = SpawnBoss((resultStage as BossStage).BossType);
                    winningMechanism = new WM_BossMode(this, boss);
                }
                else
                    winningMechanism = new WM_FiniteMode(this);
            }
            else
            {
                winningMechanism = new WM_FiniteMode(this);
            }

            GenerateSquaresFromData();
            GenerateSpecialMechanicIfBoss();
        }

        score = new Score(CalculateScoreRequirementsForStars());
        GamePlayMenu.main.SetMedalValues(score.requirements);
    }

    private void GenerateSpecialMechanicIfBoss()
    {
        if (boss != null)
        {
            if(boss.GetType().IsAssignableFrom(typeof(SquidBoss)))
                SpawnGooRandomly(5);
            else if(boss.GetType().IsAssignableFrom(typeof(CactusBoss)))
                SpawnHeartRandomly();
        } 
    }

    #region PowerUp/SpecialElementRelated
    // Actual powerups.
    public void DamageRow(int row, Attribute.Type type)
    {
        float damage = DamageCalculator.Instance.CalculatePUDamage(
            character.Attributes, null, type);
        //Debug.Log(damage);
        int count = GetDestroyablesInRowRaycast(row);

        for (int i = 0; i < count; i++)
        {
            destroyablesInLine[i].TakeDmg(damage, isPowerUpDmg: true);
        }
    }

    public void DamageCol(int col, Attribute.Type type)
    {
        float damage = DamageCalculator.Instance.CalculatePUDamage(
            character.Attributes, null, type);

        int count = GetDestroyablesInColRaycast(col);

        for (int i = 0; i < count; i++)
        {
            destroyablesInLine[i].TakeDmg(damage, isPowerUpDmg: true);
        }
    }

    public void SpecialElementBomb(int row, int col)
    {
        float damage = DamageCalculator.Instance.CalculatePUDamage(
            character.Attributes, null, Attribute.Type.PU_DAMAGE_EXPLOSION);

        float radius = AttributeConverter.Instance.Get(Attribute.Type.EXPLOSION_RADIUS);

        //GetDestroyablesInCircleLoop(row, col);
        int amount = GetDestroyablesInOverlapCircle(GetPositionFromIndex(row, col), radius);

        for (int i = 0; i < amount; i++)
        {
            destroyablesInCircle[i].TakeDmg(damage, isPowerUpDmg: true);
        }
    }

    public void SpecialElementLaserHorizontal(int row)
    {
        DamageRow(row, Attribute.Type.PU_DAMAGE_LASER);
    }

    public void SpecialElementLaserVertical(int col)
    {
        DamageCol(col, Attribute.Type.PU_DAMAGE_LASER);
    }

    // Powerup behaviour.
    // HACK: not optimal destroying of powerups
    private void DestroyUsedPowerUps()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if(elements[i, j] is PowerUp p)
                {
                    if(p != null && p.isDestroyedNextTurn)
                    {
                        Destroy(p.gameObject);
                    }
                }
            }
        }
    }
    #endregion

    #region HelperFunctions
    private void SpawnHeartRandomly()
    {
        int col = UnityEngine.Random.Range(0, cols);
        int row = UnityEngine.Random.Range(GetTopMostRowIndex(col), LowestNotEmptyIndex);
        SpawnHeartAt(row, col);
    }

    private void SpawnHeartAt(int row, int col)
    {
        SaveElement heart = new SaveElement(Element.ElementType.CACTUS_HEART,
            row, col, new SaveDestroyable(Mathf.CeilToInt(boss.HP), boss.ColorIndex));
        Vector3 pos = GetPositionFromIndex(row, col);
        Element heartElement = PutElementToArray(
            ElementHandler.Instance.DeserializeAt(heart, pos), row, col, destroyExisting: true);
        
        CactusHeart cactusHeart = heartElement.GetComponent<CactusHeart>();
        CactusBoss cactusBoss = (boss as CactusBoss);
        cactusHeart.Owner = cactusBoss;
        cactusBoss.Heart = cactusHeart;
    }

    private void GetHeartToSafetyIfExists(int dangerousRowCount)
    {
        if (!(boss is CactusBoss))
            return;

        int col = UnityEngine.Random.Range(dangerousRowCount, cols - dangerousRowCount);
        int row = UnityEngine.Random.Range(
            Math.Max(dangerousRowCount, GetTopMostRowIndex(col)), LowestNotEmptyIndex);

        CactusBoss cactusBoss = (boss as CactusBoss);
        SwapArrayPositions(cactusBoss.Heart.row, cactusBoss.Heart.col, row, col);
    }

    private Destroyable[] GetRandomDestroyable(int amount)
    {
        List<Destroyable> destroyables = GetAllDestroyablesExceptBoss();
        Destroyable[] chosenDestroyables = new Destroyable[amount];

        if (destroyables.Count == 0)
            return chosenDestroyables;

        for (int i = 0; i < amount; i++)
        {
            chosenDestroyables[i] = destroyables[UnityEngine.Random.Range(0, destroyables.Count)];
        }
        return chosenDestroyables;
    }

    private List<Destroyable> GetAllDestroyablesExceptBoss()
    {
        List<Destroyable> destroyables = new List<Destroyable>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (IsDestroyableAndNotNull(elements[i, j]))
                    destroyables.Add(DestroyableFrom(elements[i, j]));
            }
        }
        return destroyables;
    }

    private int GetDestroyablesInRowRaycast(int row)
    {
        int count = Physics2D.RaycastNonAlloc(GetPositionFromIndex(row, 0), Vector2.right, raycastHitsInLine, 50f, squareLayer);
        int destroyableCount = ConvertRaycastLineToDestroyables(count);
        return destroyableCount;
    }

    private int GetDestroyablesInColRaycast(int col)
    {
        int count = Physics2D.RaycastNonAlloc(GetPositionFromIndex(0, col), Vector2.down, raycastHitsInLine, 50f, squareLayer);
        int destroyableCount = ConvertRaycastLineToDestroyables(count);
        return destroyableCount;
    }

    private int ConvertRaycastLineToDestroyables(int count)
    {
        int lastIndex = 0;
        for (int i = 0; i < count; i++)
        {
            Destroyable d = raycastHitsInLine[i].collider.GetComponent<Destroyable>();
            if (d != null)
                destroyablesInLine[lastIndex++] = d;
        }

        return lastIndex;
    }

    private int GetDestroyablesInOverlapCircle(Vector3 center, float radius)
    {
        int lastIndex = 0;
        radius *= .5f;
        int count = Physics2D.OverlapCircleNonAlloc(center, radius, collidersInCircle, squareLayer);
#if UNITY_EDITOR
        Debug.DrawLine(center, new Vector3(center.x, center.y + radius, 0), Color.red, 5f);
#endif

        for (int i = 0; i < count; i++)
        {
            Destroyable destroyable = collidersInCircle[i].GetComponent<Destroyable>();
            if (destroyable != null)
                destroyablesInCircle[lastIndex++] = destroyable;
        }
        return lastIndex;
    }

    private int GetDestroyablesInCircleLoop(int row, int col)
    {
        int lastIndex = 0;

        int startRow = Mathf.Clamp(row - 1, 0, rows - 1);
        int endRow = Mathf.Clamp(row + 1, 0, rows - 1);
        int startCol = Mathf.Clamp(col - 1, 0, cols - 1);
        int endCol = Mathf.Clamp(col + 1, 0, cols - 1);

        for (int i = startRow; i <= endRow; i++)
        {
            for (int j = startCol; j <= endCol; j++)
            {
                if (lastIndex >= destroyablesInCircle.Length)
                    break;
                if (IsDestroyableAndNotNull(elements[i, j]))
                    destroyablesInCircle[lastIndex++] = elements[i, j] as Destroyable;
            }
        }

        return lastIndex;
    }

    //private void ClearOldCircleOfDestroyablesAndColliders()
    //{
    //    for (int i = 0; i < destroyablesInCircle.Length; i++)
    //    {
    //        collidersInCircle[i] = null;
    //        destroyablesInCircle[i] = null;
    //    }
    //}

    public void SwapArrayElementWithRandom(Element e)
    {
        SwapArrayPositionWithRandom(e.row, e.col);
    }

    public void SwapArrayPositionWithRandom(int row, int col)
    {
        int newCol = UnityEngine.Random.Range(0, cols);
        int newRow = UnityEngine.Random.Range(GetTopMostRowIndex(newCol), LowestNotEmptyIndex + 1);

        SwapArrayPositions(row, col, newRow, newCol);
    }

    private void SwapArrayPositions(int row1, int col1, int row2, int col2)
    {
        Element temp = elements[row1, col1];
        PutElementToArrayAndPosition(elements[row2, col2], row1, col1);
        PutElementToArrayAndPosition(temp, row2, col2);
    }
    #endregion

    #region SpecialAbilities
    public void SpecialAbilityMoveUp(int disctance)
    {
        for (int i = 0; i < 2; i++)
        {
            LiftRowsUp();
        }
    }

    public void SpecialAbilityBreakSides(int count)
    {
        if (count < 0)
            return;

        GetHeartToSafetyIfExists(count);
        HideWarning();

        score.IsDeathIncreasingNext = false;
        BreakSideCols(count);
        BreakSideRows(count);
        score.IsDeathIncreasingNext = true;
    }

    private void BreakSideRows(int count)
    {
        for (int i = 0; i < count; i++)
        {
            BreakRow(i);
        }
        for (int i = rows - count - 1; i < rows - 1; i++)
        {
            BreakRow(i);
        }
    }

    private void BreakSideCols(int count)
    {
        for (int i = 0; i < count; i++)
        {
            BreakCol(i);
        }
        for (int i = cols - count; i < cols; i++)
        {
            BreakCol(i);
        }
    }

    private void BreakCol(int index)
    {
        for (int i = 0; i < rows; i++)
        {
            BreakIfPossible(elements[i, index]);
        }
    }

    private void BreakRow(int index)
    {
        for (int i = 0; i < cols; i++)
        {
            BreakIfPossible(elements[index, i]);
        }
    }
    #endregion

    #region Goo
    private void SpawnGooRandomly(int amount)
    {
        Destroyable[] chosenDestroyables = GetRandomDestroyable(amount);
        for (int i = 0; i < amount; i++)
        {
            PlaceGoo(chosenDestroyables[i], (byte)UnityEngine.Random.Range(1, 3 + 1));
        }
    }

    public void PlaceGoo(Destroyable target, byte level = 1)
    {
        if (target == null)
            return;

        Goo goo = GooFactory.Instance.GetGoo();
        goo.Level = level;
        ConvertToNormalIfSpecialElement(target);
        goo.Attach(target);
    }

    public void ReproduceGooFrom(int row, int col, bool makeNewIfPossible)
    {
        LoadNeighboursOf(row, col);

        if (neighbourElements.Count == 0)
            return;
        
        if (makeNewIfPossible && CanPlaceNewGoo())
        {
            RemoveExistingGoosFromNeighbourElements();
        }

        Goo goo = GooFactory.Instance.GetGoo();
        Destroyable target = (Destroyable)neighbourElements[randomGenerator.Next(neighbourElements.Count)];

        target = ConvertToNormalIfSpecialElement(target);
        goo.Attach(target);
    }

    private void RemoveExistingGoosFromNeighbourElements()
    {
        int i = 0;
        while (i < neighbourElements.Count)
        {
            if (neighbourElements[i].GetComponentInChildren<Goo>() != null)
                neighbourElements.RemoveAt(i);
            else
                i++;
        }
    }

    private bool CanPlaceNewGoo()
    {
        foreach (Element e in neighbourElements)
        {
            if (e.GetComponentInChildren<Goo>() == null)
                return true;
        }
        return false;
    }

    private Destroyable ConvertToNormalIfSpecialElement(Destroyable target)
    {
        if (!Element.IsSpecial(target.type))
            return target;

        SaveElement copy = new SaveElement(Element.ElementType.SQUARE, target.row, target.col,
            new SaveDestroyable(Mathf.CeilToInt(target.HP), target.ColorIndex));
        elements[copy.row, copy.col] = ElementHandler.Instance.DeserializeAt(copy, target.transform.position);

        Destroy(target.gameObject);
        return (Destroyable)elements[copy.row, copy.col];
    }

    public void LoadNeighboursOf(int row, int col)//, int spreadDistance = 1)
    {
        neighbourElements.Clear();

        // Important to set both to zero here.
        Vector2Int direction = Vector2Int.zero;
        Element element = GetDestroyableElementSafe(row, col);

        // Triangles are handled differently.
        if (element is Triangle)
        {
            Triangle middleTriangle = element as Triangle;
            direction.x = middleTriangle.Offset.x;
            AnalyzeAndLoadCorrectNeighbour(direction);
            direction.x = 0;
            direction.y = middleTriangle.Offset.y;
            AnalyzeAndLoadCorrectNeighbour(direction);
        }
        else
        {
            AnalyzeAllDirectionNonAlloc(direction);
        }

        void AnalyzeAllDirectionNonAlloc(Vector2Int directionLocal)
        {
            directionLocal.x = 1;
            AnalyzeAndLoadCorrectNeighbour(directionLocal);
            directionLocal.x = -1;
            AnalyzeAndLoadCorrectNeighbour(directionLocal);
            directionLocal.x = 0;
            directionLocal.y = 1;
            AnalyzeAndLoadCorrectNeighbour(directionLocal);
            directionLocal.y = -1;
            AnalyzeAndLoadCorrectNeighbour(directionLocal);
        }

        void AnalyzeAndLoadCorrectNeighbour(Vector2Int offset)
        {
            // Map is upside down.
            element = GetDestroyableElementSafe(row + offset.y * -1, col + offset.x);
            if (IsGooAttachable(element))
            {
                if (element is Triangle)
                {
                    if (IsFlatSideOfTriangleConnected(offset, element as Triangle))
                        neighbourElements.Add(element);

                }
                else
                {
                    neighbourElements.Add(element);
                }
            }
        }

        bool IsFlatSideOfTriangleConnected(Vector2Int offset, Triangle triangle)
        {
            bool isHorizontal = offset.y == 0;
            int originalObjectOffset = isHorizontal ? offset.x : offset.y;
            int otherObjectOffset = isHorizontal ? triangle.Offset.x : triangle.Offset.y;

            return originalObjectOffset * -1 == otherObjectOffset;
        }
    }

    private bool IsGooAttachable(Element element)
    {
        if (element == null)
            return false;

        Goo existingGoo = element.GetComponentInChildren<Goo>();
        if (existingGoo != null && !existingGoo.CanGrow)
            return false;

        return true;
    }

    private Element GetDestroyableElementSafe(int row, int col)
    {
        if (row < 0 || row >= rows)
            return null;
        if (col < 0 || col >= cols)
            return null;

        if (!IsDestroyableAndNotNull(elements[row, col]))
            return null;

        return elements[row, col];
    }
    #endregion

    #region GeneratingTheMap
    private Boss SpawnBoss(Type bossType)
    {
        GameObject bossGO = bossFactory.GetBoss(bossType);
        // HACK, position is constant.
        bossGO.transform.position = startPosition + new Vector3(4.5f, -1.5f, 0) * squareSize;
        Boss boss = bossGO.GetComponent<Boss>();
        // HACK, random values.
        boss.SetDefaultState(MapDataBase.main.BossHp, Palette.DefaultColorIndex);
        return boss;
    }

    private void GenerateSquaresFromData()
    {
        maxSpawnRoundCount = MapDataBase.main.GetMapLength() - rows + emptyRows;

        for (int i = LowestNotEmptyIndex; (i >= 0) && (i >= -maxSpawnRoundCount); i--)
        {
            GenerateRowFromData(i, MapDataBase.main.GetNextLine());
        }
    }

    private void GenerateRowFromData(int row, List<SaveElement> saveElements)
    {
        foreach (SaveElement s in saveElements)
        {
            int realRow = row + GetTopMostRowIndex(s.col);
            if (realRow > LowestNotEmptyIndex)
                continue;

            Vector3 pos = GetPositionFromIndex(realRow, s.col);

            PutElementToArray(ElementHandler.Instance.DeserializeAt(s, pos), realRow, s.col);

            if(elements[realRow, s.col].IsDestroyable())
            {
                destroyablesGenerated++;
            }
        }
    }

    // ATM Just pops elements from the stack that went out of screen.
    private void GenerateRowFromData(int row, Element[] rowElements)
    {
        foreach (Element e in rowElements)
        {
            if (e != null)
            {
                int realRow = row + GetTopMostRowIndex(e.col);
                if (realRow > LowestNotEmptyIndex)
                    continue;

                PutElementToArrayAndPosition(e, realRow, e.col);

                e.gameObject.SetActive(true);
            }
        }
    }

    private Element PutElementToArrayAndPosition(Element e, int row, int col, bool destroyExisting = false)
    {
        if (e != null)
        {
            e.transform.position = GetPositionFromIndex(row, col);
            if (e.IsDestroyable())
                DestroyableFrom(e).ResetTextPosition();
        }

        return PutElementToArray(e, row, col, destroyExisting);
    }

    private Element PutElementToArray(Element e, int row, int col, bool destroyExisting = false)
    {
        if (destroyExisting && elements[row, col] != null)
            Destroy(elements[row, col].gameObject);

        elements[row, col] = e;
        if(e != null)
            e.SetRowAndCol(row, col);
        return e;
    }

    private int GetTopMostRowIndex(int col)
    {
        return MapDataBase.main.TopMostRows.Get(col);
    }

    public Vector3 GetPositionFromIndex(int row, int col)
    {
        // The starting point is the left top point of grid.
        Vector3 pos = startPosition + new Vector3(col, -row) * squareSize;
        return pos;
    }
    #endregion

    #region ScoreRelated
    private int[] CalculateScoreRequirementsForStars()
    {
        int[] reqs = new int[Score.GetMedalCount()];

        int maxRoundCount = 20;
        int destroyableCount = 60;

        if (MapDataBase.main != null)
        {
            maxRoundCount = MapDataBase.main.GetMapLength() + emptyRows - 1;
            destroyableCount = MapDataBase.main.GetNumberOfDestroyables();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("MapDataBase not present at score calculation");
#endif
        }

        float minBlockPerRound = (float)destroyableCount / maxRoundCount;

        
        //Debug.Log("Max rounds: " + maxRoundCount);
        //Debug.Log("Destroyable count: " + destroyableCount);
        //Debug.Log("Block/Round: " + minBlockPerRound);
        

        int bronzeValue = Mathf.FloorToInt((((minBlockPerRound + 1) * minBlockPerRound) / 2) * maxRoundCount) * 10;

        int medalIndex = (int)Score.MedalType.BRONZE;
        reqs[medalIndex] = bronzeValue;

        reqs[++medalIndex] =
            Mathf.FloorToInt((bronzeValue / scorePercentages[(int)Score.MedalType.BRONZE] * scorePercentages[medalIndex]) / 10) * 10;
        reqs[++medalIndex] =
            Mathf.FloorToInt((bronzeValue / scorePercentages[(int)Score.MedalType.BRONZE] * scorePercentages[medalIndex]) / 10) * 10;

        return reqs;
    }

    public void AddScore()
    {
        score.AddNext();

        GamePlayMenu.main.UpdateTargetScore(score.currentValue);
    }

    public int GetCurrentNumberOfStars()
    {
        return score.currentNumberOfStars;
    }
    #endregion

    #region CoreGamePlay

    public int GetVisibleDestroyableCount()
    {
        return destroyablesGenerated;
    }

    public void NextRound()
    {
        OnEndOfRound?.Invoke();

        if (winningMechanism.CheckIfWon())
        {
            HideWarning();
            Victory();
        }
        else
        {
            score.ResetNextToDefault();
            DestroyUsedPowerUps();
            LiftRowsDown();
            if (maxSpawnRoundCount > currentRound)
            {
                if(elementsPushedOutOfScreen.Count != 0)
                {
                    GenerateRowFromData(0, elementsPushedOutOfScreen.Pop());
                }
                else if (MapDataBase.main != null)
                {
                    GenerateRowFromData(0, MapDataBase.main.GetNextLine());
                }
            }
            // HACK: small performance issue, turning on and off warning when defeat.
            if (loosingMechanism.CheckIfLost())
                Defeat();
            else
                ShowWarningIfNeeded();

            currentRound++;
        }
    }

    private void LiftRowsDown()
    {
        int lastSafeRow = LastSafeRowIndex;

        for (int i = lastSafeRow; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                if (elements[i, j] != null)
                {
                    PutElementToArrayAndPosition(elements[i, j], i + 1, j);
                    elements[i, j] = null;
                }
            }
        }
    }

    private void LiftRowsUp()
    {
        HideWarning();

        maxSpawnRoundCount++;
        Element[] upmostElements = new Element[cols];
        for (int j = 0; j < cols; j++)
        {
            int row = GetTopMostRowIndex(j);
            if (elements[row, j] != null)
            {
                upmostElements[j] = elements[row, j];
                upmostElements[j].gameObject.SetActive(false);

                elements[row, j] = null;
            }
        }
        elementsPushedOutOfScreen.Push(upmostElements);

        for (int i = 1; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (elements[i, j] != null)
                {
                    PutElementToArrayAndPosition(elements[i, j], i - 1, j);
                    elements[i, j] = null;
                }
            }
        }
    }

    private bool IsDestroyableAndNotNull(Element element)
    {
        if(element != null && element.IsDestroyable())
        {
            return true;
        }
        return false;
    }

    private Destroyable DestroyableFrom(Element element)
    {
        return (Destroyable)element;
    }

    private bool BreakIfPossible(Element element)
    {
        if (IsDestroyableAndNotNull(element))
        {
            ((Destroyable)element).Break();
            return true;
        }
        return false;
    }

    private bool DamageIfPossible(Element element, int damage = 1)
    {
        if (IsDestroyableAndNotNull(element))
        {
            ((Destroyable)element).TakeDmg(damage);
            return true;
        }
        return false;
    }

    public void ShowWarningIfNeeded()
    {
        if (IsDestroyableInRow(LastSafeRowIndex))
            ShowWarning();
        else
            HideWarning();
    }

    public void ShowWarning()
    {
        warning.Show(true);
    }

    public void HideWarning()
    {
        warning.Show(false);
    }
    #endregion

    #region GameState
    public int GetCurrentRoundNumber()
    {
        return currentRound;
    }

    public int GetMaxSpwanRoundNumber()
    {
        return maxSpawnRoundCount;
    }

    public int DestroyablesLeft()
    {
        int count = 0;
        foreach (Element e in elements)
        {
            if (IsDestroyableAndNotNull(e))
            {
                count++;
            }
        }

        return count;
    }

    public bool IsDestroyableInRow(int index)
    {
        for (int j = 0; j < cols; j++)
        {
            //if (elements[index, j] != null && elements[index, j].IsDestroyable())
            if (IsDestroyableAndNotNull(elements[index, j]))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsDestroyableInCol(int index)
    {
        for (int j = 0; j < rows; j++)
        {
            //if (elements[index, j] != null && elements[index, j].IsDestroyable())
            if (IsDestroyableAndNotNull(elements[j, index]))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsElementInRow(int index)
    {
        for (int j = 0; j < cols; j++)
        {
            if (elements[index, j] != null)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    private void Victory()
    {
        Debug.Log("You won!");

        if (MapDataBase.main.IsTesting)
        {
            ReturnToMapEditor();
        }
        else
        {
            if (ProfileDataBase.main != null)
            {
                if (Menu.main.GetCurrentLvl() == ProfileDataBase.main.GetLastPlayableMap())
                {                                                      // #unintendedboobs
                    ProfileDataBase.main.CompleteLevel(Menu.main.GetCurrentLvl(), score.currentNumberOfStars);
                    Menu.main.RefreshLastShownLevelButtons();
                }
                else
                {
                    ProfileDataBase.main.CompleteLevel(Menu.main.GetCurrentLvl(), score.currentNumberOfStars);
                    Menu.main.UpdateStarAmountOnCurrentLevelButton();
                }

                if (ProfileDataBase.main.GetHighestScore() < score.currentValue)
                {
                    ProfileDataBase.main.StatSetHighestScore(score.currentValue);
                }
                ProfileDataBase.main.SaveData();
            }

            GamePlayMenu.main.GameOverPanel(true);
        }
    }

    private void Defeat()
    {
        Debug.Log("You lost.");

        if (MapDataBase.main.IsTesting)
        {
            ReturnToMapEditor();
        }
        else
        {
            OnLevelFailed?.Invoke();
            GamePlayMenu.main.GameOverPanel(false);
        }
    }

    private void ReturnToMapEditor()
    {
        Menu.main.SetMapEditorScreen();
    }
}
