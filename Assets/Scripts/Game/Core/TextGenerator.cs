using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGenerator : MonoBehaviour
{
    public static TextGenerator main { get; private set; } = null;


    public GameObject textPrefab;

    public GameObject canvas;
    public GameObject parent;

    void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }


    public Text Generate(Vector3 worldPosition)
    {
        GameObject text = Instantiate(textPrefab);
        text.transform.SetParent(parent.transform, false);
        Move(text, worldPosition);

        return text.GetComponent<Text>();
    }
    
    public Text Generate(Vector3 worldPosition, Color color)
    {
        Text temp = Generate(worldPosition);
        temp.color = color;

        return temp;
    }

    public void Move(GameObject text, Vector3 worldPosition)
    {
        RectTransform textTransform = text.GetComponent<RectTransform>();
        textTransform.anchorMin = Vector2.zero;
        textTransform.anchorMax = Vector2.zero;
        textTransform.pivot = new Vector2(.5f, .5f);
        Vector2 ratio = new Vector2(
            ValueCacher.Instance.CurrentCanvasSize.x / Screen.width,
            ValueCacher.Instance.CurrentCanvasSize.y / Screen.height);
        textTransform.anchoredPosition = ValueCacher.Instance.MainCamera.WorldToScreenPoint(worldPosition) * ratio;
    }
}
