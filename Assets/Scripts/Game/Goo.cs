using System;
using UnityEngine;

public class Goo : MonoBehaviour
{
    public static Action OnGooDestroyed;
    public static Action OnNewGooAttached;

    public SpriteRenderer sr_goo;

    private Destroyable target;
    private bool isTargetTriangle = false;

    private byte level = 1;
    private const byte maxLevel = 3;

    private bool isCapableOfAction = true;
    private bool isAttached = false;

    public byte Level { 
        get
        {
            return level;
        }
        set
        {
            bool oldBehaviour = CanGrow;

            level = (byte)Mathf.Clamp(value, 1, maxLevel);
            RefreshSprite();

            if (isAttached && oldBehaviour != CanGrow)
            {
                if (CanGrow)
                {
                    GooOverseer.OnGrow += Grow;
                    GooOverseer.OnReproduce -= Reproduce;
                }
                else
                {
                    GooOverseer.OnGrow -= Grow;
                    GooOverseer.OnReproduce += Reproduce;
                }
            }
        }
    }
    public bool CanGrow { get { return level < maxLevel; } }
    
    private void OnDestroy()
    {
        GooOverseer.OnGrow -= Grow;
        GooOverseer.OnReproduce -= Reproduce;
        GooOverseer.OnEndOfReproduction -= Rest;
        
        if(target != null)
            OnGooDestroyed?.Invoke();
    }

    public void Attach(Destroyable square)
    {
        if (square == null)
            Destroy(gameObject);

        Goo existingGoo = square.GetComponentInChildren<Goo>();
        if (existingGoo != null)
        {
            if (existingGoo.CanGrow)
                existingGoo.Grow();
            Destroy(gameObject);
        }
        else
        {
            target = square;
            transform.SetParent(square.transform, false);
            transform.position = square.transform.position; //square.MiddlePoint;
            square.SetTextColor(dark: false);

            if (target is Triangle)
                isTargetTriangle = true;
            RefreshSprite();

            GooOverseer.OnEndOfReproduction += Rest;
            if (CanGrow)
                GooOverseer.OnGrow += Grow;
            else
                GooOverseer.OnReproduce += Reproduce;

            OnNewGooAttached?.Invoke();
            isAttached = true;
        }
    }

    public void Grow()
    {
        if (!CanGrow || !isAttached)
            return;
        
        Level++;
        isCapableOfAction = false;
    }

    private void Reproduce()
    {
        if (!isCapableOfAction || CanGrow || !isAttached)
            return;

        target.TakeDmg(-5);

        SquareDataBase.Instance.ReproduceGooFrom(target.row, target.col, makeNewIfPossible: true);

        isCapableOfAction = false;
    }

    private void Rest()
    {
        isCapableOfAction = true;
    }

    private void RefreshSprite()
    {
        sr_goo.sprite = isTargetTriangle ?
                        GooFactory.Instance.GetTriangleGooSprite(level) :
                        GooFactory.Instance.GetGooSprite(level);
    }

}
