using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastRowWarning : MonoBehaviour
{
    public GameObject warningLine;
    public GameObject warningPanel;

    private GameObject[] warningLines;

    private Image warningPanelImage;

    private float minAlpha = .3f;
    private float lerper = 1f;

    private bool isGoingUp = false;


    /// <summary>
    /// The warning "line" section is obsolete.
    /// Uncomment the line in Start(), and the 2 function at the bottom to revert it to a functional state.
    /// The currently used version is the panel, and the panel only.
    /// </summary>


    void Start()
    {
        //SetupWarningLines();

        warningPanelImage = warningPanel.GetComponent<Image>();

        Show(false);
    }

    void Update()
    {
        float currentChange = isGoingUp ? Time.deltaTime : -Time.deltaTime;
        lerper += currentChange;

        warningPanelImage.color = new Color(1, 1, 1, Mathf.Lerp(minAlpha, 1f, lerper));

        if(lerper > 1f)
        {
            isGoingUp = false;
        }
        else if(lerper < minAlpha)
        {
            isGoingUp = true;
        }
    }

    public void Show(bool value)
    {
        warningPanel.SetActive(value);
        lerper = 1f;
        isGoingUp = false;
    }

    /*
    private void SetupWarningLines()
    {
        Vector2[] positions = CameraBoundaries.main.GetScreenMiddlePoints();
        warningLines = new GameObject[positions.Length];

        int i = 0;
        foreach (Vector2 pos in positions)
        {
            warningLines[i++] = Instantiate(warningLine, pos, i % 2 == 0 ? Quaternion.identity : Quaternion.identity * Quaternion.Euler(0, 0, 90f));
        }

        TurnOnWarningLines(false);
    }

    private void TurnOnWarningLines(bool value)
    {
        foreach(GameObject g in warningLines)
        {
            g.SetActive(value);
        }
    }
    */
}
