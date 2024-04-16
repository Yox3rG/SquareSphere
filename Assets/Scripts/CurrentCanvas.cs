using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentCanvas : MonoBehaviour
{
    private void OnEnable()
    {
        ValueCacher.Instance.SetCurrentCanvas(GetComponent<Canvas>());
    }
}
