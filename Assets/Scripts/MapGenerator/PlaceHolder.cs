using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceHolder : MonoBehaviour
{
    [SerializeField]
    private int row = 0, col = 0;

    public SaveElement saveElement { get; private set; } = null;

    // Used when drawing multiple elements with one touch.
    // Not ever set inside this file.
    public bool isSetThisOperation { get; set; }

    // TODO: touch functionality (kinda fd up)
    /*
    void OnMouseDown()
    {
        //if (!EventSystem.current.IsPointerOverGameObject(-1) && (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 11))
        {
            if (MapGenerator.main.isTrashCan)
            {
                DeleteBlockAndLook();
            }
            else
            {
                SetBlockAndLook();
            }
        }
    }
    */

    public bool IsElementSet()
    {
        return saveElement != null;
    }

    public bool IsElementDestroyable()
    {
        return IsElementSet() ? Element.IsDestroyable(saveElement.type) : false;
    }

    public Element GetLook()
    {
        return GetComponentInChildren<Element>();
    }

    public void SetRowAndCol(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void SetHp(int hp)
    {
        if (IsElementDestroyable())
        {
            this.saveElement.destroyable.maxHp = hp;
            transform.GetComponentInChildren<Destroyable>().SetMaxHp(hp);
        }
    }

    public void SetColor(byte colorIndex)
    {
        if (IsElementDestroyable())
        {
            this.saveElement.destroyable.colorIndex = colorIndex;
            transform.GetComponentInChildren<Destroyable>().SetColor(colorIndex);
        }
    }

    public void DeleteElementAndVisuals()
    {
        saveElement = null;
        DeleteElementVisuals();
    }

    public void SetBlockAndLook()
    {
        SetElement();
        CreateElementVisuals();
    }

    private void SetElement()
    {
        saveElement = new SaveElement(MapGenerator.main.currentTool);
        saveElement.row = row;
        saveElement.col = col;
    }

    private void CreateElementVisuals()
    {
        DeleteElementVisuals();

        Element g = ElementHandler.Instance.DeserializeAt(saveElement, transform.position);
        if(g != null)
            g.transform.SetParent(transform);
    }

    private void DeleteElementVisuals()
    {
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
