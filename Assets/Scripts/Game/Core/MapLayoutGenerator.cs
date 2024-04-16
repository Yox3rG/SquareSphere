using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLayoutGenerator
{
    private Map lastMap = new Map();

    // This is a percentage in an integer, measured against highestSpawnInt (spawnChance / highestSpawnInt).
    // For example, a chance of (float)0.555f (=55.5%) is equvivalent to (int)555 in this case.
    // They are cumulative, so with 100, 200, 500, we have 10% + 10% + 30%
    private int powerupSpawnRange = 30;
    private int specialElementRange = 80;
    private int triangleSpawnRange = 200;  
    private int squareSpawnRange = 500;          
    private int highestSpawnInt = 1000;

    private bool hasTriangles = false;
    private bool hasPowerUps = false;
    private bool hasSpecialElements = false;

    private int minHp = 10;
    private int maxHp = 30;
    private int colorSetStart = 0;
    private int colorSetLength = 3;

    // Database parts.
    private Random.State elementState;
    private Random.State hpState;
    private Random.State colorState;

    private int maxRow, maxCol;

    public MapLayoutGenerator(int maxRows, int maxCols)
    {
        maxRow = maxRows;
        maxCol = maxCols;
    }

    public void SetHpPool(int min, int max)
    {
        minHp = min;
        maxHp = max;
    }

    public Map CreateMap(int seed)
    {
        lastMap = new Map();

        hasTriangles = StageLayout.GetMain().HasTriangles(seed);
        hasPowerUps = StageLayout.GetMain().HasPowerUps(seed);
        hasSpecialElements = StageLayout.GetMain().HasPowerUps(seed);

        Random.InitState(seed);
        elementState = Random.state;
        hpState = Random.state;
        colorState = Random.state;

        // Modification to Random.state after this is lost

        // +1 to make it count instead of index
        // +1 to make it inclusive random generation.
        colorSetStart = Random.Range(0, Palette.LastColorIndex + 2 - colorSetLength);

        for (int i = 0; i < maxRow; i++)
        {
            GenerateNewLastRow();
        }

        return lastMap;
    }

    private void GenerateNewLastRow()
    {
        Random.state = elementState;

        lastMap.lines.Add(new SaveElementLine());
        int currentLine = lastMap.lines.Count - 1;

        for (int j = 0; j < maxCol; j++)
        {
            int random = Random.Range(0, highestSpawnInt);
            SaveElement tempSaveElement = null;
            if (hasPowerUps && random < powerupSpawnRange)
            {
                tempSaveElement = GenerateRandomPowerUpAt(currentLine, j);
            }
            else if (hasSpecialElements && random < specialElementRange)
            {
                tempSaveElement = GenerateRandomSpecialElement(currentLine, j);
            }
            else if (hasTriangles && random < triangleSpawnRange)
            {
                tempSaveElement = GenerateRandomTriangleAt(currentLine, j);
            }
            else if (random < squareSpawnRange)
            {
                tempSaveElement = GenerateRandomSquareAt(currentLine, j);
            }
            else
            {
                continue;
            }

            lastMap.lines[currentLine].list.Add(tempSaveElement);
            if (tempSaveElement.IsDestroyable)
            {
                lastMap.destroyableCount++;
            }
        }
        elementState = Random.state;
    }
    
    #region NewRandomElementGenerationBackEnd
    private SaveElement GenerateRandomPowerUpAt(int row, int col)
    {
        Element.ElementType type = ElementHandler.Instance.GeneratePowerUpTypeRandomly();
        return new SaveElement(type, row, col);
    }

    private SaveElement GenerateRandomSpecialElement(int row, int col)
    {
        Element.ElementType type = ElementHandler.Instance.GenerateSpecialElementRandomly();
        return new SaveElement(type, row, col, new SaveDestroyable(GenerateRandomHp(), GenerateRandomColor()));
    }

    private SaveElement GenerateRandomTriangleAt(int row, int col)
    {
        Element.ElementType type = ElementHandler.Instance.GenerateTriangleTypeRandomly();
        return new SaveElement(type, row, col, new SaveDestroyable(GenerateRandomHp(), GenerateRandomColor()));
    }

    private SaveElement GenerateRandomSquareAt(int row, int col)
    {
        return new SaveElement(Element.ElementType.SQUARE, row, col, new SaveDestroyable(GenerateRandomHp(), GenerateRandomColor()));
    }

    private int GenerateRandomHp()
    {
        // Element generation should not be interrupted.
        Random.State oldState = Random.state;

        Random.state = hpState;
        int temp = Random.Range(minHp, maxHp + 1);
        hpState = Random.state;

        Random.state = oldState;
        return temp;
    }

    private byte GenerateRandomColor()
    {
        // Element generation should not be interrupted.
        Random.State oldState = Random.state;

        Random.state = colorState;
        byte temp = (byte)(colorSetStart + Random.Range(0, colorSetLength));
        colorState = Random.state;

        Random.state = oldState;
        return temp;
    }
    #endregion


    //private int GenerateFakeRow()
    //{

    //    int destroyableCount = 0;

    //    for (int j = 0; j < cols; j++)
    //    {
    //        if (Random.Range(0, highestSpawnInt) < spawnChance)
    //        {
    //            if (Random.Range(0, highestSpawnInt) < powerupSpawnChance)
    //            {
    //                GenerateFake();
    //            }
    //            else if (hasTriangles && Random.Range(0, highestSpawnInt) < triangleSpawnChance)
    //            {
    //                GenerateFake();
    //                destroyableCount++;
    //            }
    //            else
    //            {
    //                destroyableCount++;
    //            }
    //        }
    //        else
    //        {
    //        }
    //    }

    //    Debug.Log(destroyableCount);
    //    return destroyableCount;
    //}

    //private int GenerateFake()
    //{
    //    Random.InitState(mapSeed);

    //    int fake = Random.Range(0, 1);
    //    return fake;
    //}

    //private int CountAllRandomDestroyables()
    //{
    //    int count = 0;

    //    for (int i = 0; i < rows; i++)
    //    {
    //        count += GenerateFakeRow();
    //    }

    //    return count;
    //}
}
