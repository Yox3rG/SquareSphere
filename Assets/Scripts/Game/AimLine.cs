using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLine : MonoBehaviour
{
    public GameObject dot;
    
    private float distanceBetweenDots = .3f;

    private bool isDecreasingBallSizeActive = false;
    private float decreasePercentage = .8f;

    private Vector2 start;
    private Vector2 end;

    private List<GameObject> dots;

    void Awake()
    {
        // HACK List initialized with given capacity. (I wouldn't say it's a hack, but it's definitely not a todo...)
        dots = new List<GameObject>(15);

    }

    public float Draw(Vector2 _start, Vector2 _end, float firstDotOffset = 0f)
    {
        start = _start;
        end = _end;
        
        Vector2 targetVector = end - start;
        Vector2 normalizedTargetVector = targetVector.normalized;

        start += normalizedTargetVector * (distanceBetweenDots - firstDotOffset);
        targetVector = end - start;

        float length = targetVector.magnitude;

        int newDotCount = DotCountFromLength(length, out float leftOver);
        ChangeDotCountTo(newDotCount);

        RepositionDots(normalizedTargetVector * distanceBetweenDots);

        return leftOver;
    }

    public void Hide()
    {
        foreach(GameObject g in dots)
        {
            if(g != null)
            {
                Destroy(g);
            }
        }
        dots.Clear();
    }

    public void SetDecreasingSizeActive(bool value, float percentage = .8f)
    {
        isDecreasingBallSizeActive = value;
        decreasePercentage = percentage;
    }

    private void ChangeDotCountTo(int newDotCount)
    {
        for (int i = newDotCount; i < dots.Count; i++)
        {
            Destroy(dots[dots.Count - 1]);
            dots.RemoveAt(dots.Count - 1);
        }

        for (int i = newDotCount; i > dots.Count; i--)
        {
            dots.Add(Instantiate(dot, new Vector3(100, 100, 0), Quaternion.identity));
        }

        if (isDecreasingBallSizeActive)
        {
            Vector3 scale = dot.transform.localScale;
            for (int i = 0; i < dots.Count; i++)
            {
                dots[i].transform.localScale = scale;
                scale *= decreasePercentage;
            }
        }
    }

    private void RepositionDots(Vector2 offset)
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].transform.position = start + offset * i;
        }
    }

    private int DotCountFromLength(float length, out float leftOver)
    {
        leftOver = length % distanceBetweenDots;
        return Mathf.CeilToInt(length / distanceBetweenDots);
    }
}
