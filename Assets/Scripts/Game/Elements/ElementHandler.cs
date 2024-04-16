using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHandler : MonoBehaviour
{
    public static ElementHandler Instance { get; private set; } = null;

    public GameObject squarePrefab;
    public GameObject trianglePrefab;
    public GameObject powerUpPrefab;
    public GameObject spikyPrefab;
    public GameObject bombPrefab;
    public GameObject laserPrefab;
    public GameObject vasePrefab;
    public GameObject shieldPrefab;

    public GameObject cactusHeartPrefab;

    public Dictionary<Element.ElementType, GameObject> destroyablePrefabs = new Dictionary<Element.ElementType, GameObject>();

    public float DefaultSquareSize { get; protected set; } = .55f;
    public float DefaultSideSize { get; protected set; } = .125f / 2;
    public float SquareSize 
    {
        get
        {
            if (useDifferentSquareScale)
                return DefaultSquareSize * differentSquareScale.x;
            return DefaultSquareSize;
        } 
    }
    public float SquareScaleFactor { get { return SquareSize / DefaultSquareSize; } }

    private Vector3 differentSquareScale = Vector3.one;
    private bool useDifferentSquareScale = false;

    private int currentId = 0; // Used in GenerateBlock.
    private Transform _elementParent;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _elementParent = new GameObject("ELEMENTS").transform;

        if (squarePrefab.GetComponent<BoxCollider2D>())
        {
            DefaultSquareSize = squarePrefab.GetComponent<BoxCollider2D>().size.x;
        }
        currentId = 0;

        destroyablePrefabs[Element.ElementType.SQUARE          ] = squarePrefab;
        destroyablePrefabs[Element.ElementType.TRIANGLE_0      ] = trianglePrefab;
        destroyablePrefabs[Element.ElementType.TRIANGLE_90     ] = trianglePrefab;
        destroyablePrefabs[Element.ElementType.TRIANGLE_180    ] = trianglePrefab;
        destroyablePrefabs[Element.ElementType.TRIANGLE_270    ] = trianglePrefab;
        destroyablePrefabs[Element.ElementType.SPIKY           ] = spikyPrefab;
        destroyablePrefabs[Element.ElementType.BOMB            ] = bombPrefab;
        destroyablePrefabs[Element.ElementType.LASER_HORIZONTAL] = laserPrefab;
        destroyablePrefabs[Element.ElementType.LASER_VERTICAL  ] = laserPrefab;
        destroyablePrefabs[Element.ElementType.LASER_BOTH      ] = laserPrefab;
        destroyablePrefabs[Element.ElementType.VASE            ] = vasePrefab;
        destroyablePrefabs[Element.ElementType.SHIELD_ODD      ] = shieldPrefab;
        destroyablePrefabs[Element.ElementType.SHIELD_EVEN     ] = shieldPrefab;
        // Boss related.
        destroyablePrefabs[Element.ElementType.CACTUS_HEART    ] = cactusHeartPrefab;
    }

    private void OnEnable()
    {
        ScreenRelativeSizeCalculator.OnCalculatingSquareScale += SetDifferentScale;
    }

    private void OnDisable()
    {
        ScreenRelativeSizeCalculator.OnCalculatingSquareScale -= SetDifferentScale;
    }

    private void SetDifferentScale(Vector3 scale)
    {
        differentSquareScale = scale;
        useDifferentSquareScale = true;
    }

    public void ResetDifferentScale()
    {
        differentSquareScale = Vector3.one;
        useDifferentSquareScale = false;
    }

    #region Square Serialization
    public Element DeserializeAt(SaveElement saveElement, Vector3 position)
    {
        Element temp = Deserialize(saveElement);
        if (temp == null)
            return null;

        temp.transform.position = position;
        if (Element.IsDestroyable(saveElement.type))
        {
            ((Destroyable)temp).ResetTextPosition();
        }
        return temp;
    }

    public Element Deserialize(SaveElement saveElement)
    {
        if(!Element.IsExistingType(saveElement.type))
        {
            Debug.LogError($"Trying to spawn element of unknown type [{saveElement.type}].");
            return null;
        }
        
        GameObject g = null;
        Element e = null;
        if (Element.IsDestroyable(saveElement.type))
        {
            g = GenerateDestroyable(saveElement.type);
            if (g == null)
                return null;

            Destroyable s = g.GetComponent<Destroyable>();

            SetDefaultDestroyableState(saveElement, s);

            e = s;
        }
        else if (Element.IsPowerUp(saveElement.type))
        {
            g = GeneratePowerUp(saveElement.type);

            if (g == null)
                return null;

            e = g.GetComponent<Element>();

            SetDefaultState(saveElement, e);
        }

        g.transform.SetParent(_elementParent);
        return e;
    }

    private void SetDefaultDestroyableState(SaveElement saveElement, Destroyable destroyable)
    {
        SetDefaultState(saveElement, element: destroyable);

        destroyable.SetDefaultState(saveElement.destroyable.maxHp, saveElement.destroyable.colorIndex);
    }

    private void SetDefaultState(SaveElement saveElement, Element element)
    {
        element.SetRowAndCol(saveElement.row, saveElement.col);
        element.SetType(saveElement.type);

        if (useDifferentSquareScale)
        {
            element.transform.localScale = differentSquareScale;
        }
    }

    public SaveElement Serialize(Element element)
    {
        SaveElement saveElement = new SaveElement(element.type, element.row, element.col);
        if(element.IsDestroyable())
        {
            Destroyable s = (Destroyable)element;
            saveElement.destroyable = new SaveDestroyable(Mathf.CeilToInt(s.HP), s.ColorIndex);
        }
        return saveElement;
    }
    #endregion

    public Element.ElementType GenerateTriangleTypeRandomly()
    {
        int orientation = Random.Range((int)Element.ElementType.TRIANGLE_0, (int)Element.ElementType.TRIANGLE_270 + 1);
        Element.ElementType type = (Element.ElementType)orientation;

        return type;
    }

    public Element.ElementType GenerateLaserTypeRandomly()
    {
        int orientation = Random.Range((int)Element.ElementType.LASER_HORIZONTAL, (int)Element.ElementType.LASER_BOTH + 1);
        Element.ElementType type = (Element.ElementType)orientation;

        return type;
    }

    public Element.ElementType GenerateSpecialElementRandomly()
    {
        Element.ElementType type = (Element.ElementType)Random.Range(Element.specialElementStart, Element.specialElementEnd);
        return type;
    }

    public Element.ElementType GeneratePowerUpTypeRandomly()
    {
        Element.ElementType type = (Element.ElementType)Random.Range(Element.powerupStart, Element.powerupEnd);
        return type;
    }

    #region GeneratePrefab
    private GameObject GenerateDestroyable(Element.ElementType type)
    {
        if (!destroyablePrefabs.ContainsKey(type))
            return null;

        GameObject g = GeneratePrefabWithHP(destroyablePrefabs[type]);

        return g;
    }

    private GameObject GeneratePrefabWithHP(GameObject prefab)
    {
        GameObject g = Instantiate(prefab, Vector3.zero, Quaternion.identity);
#if UNITY_EDITOR
        g.name = prefab.name + "_" + currentId++;
#endif

        Text text = TextGenerator.main.Generate(g.transform.position);
        g.GetComponent<Destroyable>().SetTextObject(text.GetComponent<Text>());

        return g;
    }

    private GameObject GeneratePowerUp(Element.ElementType type)
    {
        if (!Element.IsPowerUp(type))
            return null;

        GameObject g = Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity);
        return g;
    }
    #endregion
}
