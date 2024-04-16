using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Destroyable : Element
{
    public static event System.Action OnDestroyableBroken;
    public static event System.Action<float> OnDamaged;

    public SpriteRenderer elementBaseGraphics;
    public Color lastSetColor;
    protected Text text;

    private IEnumerator _currentHitEffectEnumerator;

    public float HP { get; protected set; } = 100;

    public byte ColorIndex { get; protected set; }

    public virtual bool IsAffectedByPowepUp { get; } = true;

    public IAttributeList Attributes { get; protected set; }

    private void OnValidate()
    {
        if (elementBaseGraphics == null)
            elementBaseGraphics = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.transform.GetComponent<Ball>();
        if (collision.gameObject.layer == 10 && ball != null)
        {
            CollidedWith(ball);
        }
    }

    protected virtual void OnDisable()
    {
        if (text != null)
        {
            text.gameObject.SetActive(false);
        }
    }

    protected virtual void OnEnable()
    {
        if (text != null)
        {
            text.gameObject.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (text != null)
        {
            Destroy(text.gameObject);
        }
    }

    public virtual void SetDefaultState(int hp, byte colorIndex)
    {
        SetMaxHp(hp);
        ResetTextPosition();
        SetColor(colorIndex);
        SetTextColor(dark: true);

        // TODO: AttributeList is empty on all destroyables.
        Attributes = new AttributeListWithBonuses();
    }

    public virtual void CollidedWith(Ball ball)
    {
        TakeDmg(ball.dmg);
    }

    public virtual void TakeDmg(float dmg, bool isPowerUpDmg = false)
    {
        if (!IsAffectedByPowepUp && isPowerUpDmg)
            return;

        HP -= dmg;
        OnDamaged?.Invoke(dmg < 0 ? 0 : dmg);
        if (HP <= 0)
        {
            Break();
            return;
        }
        CalculateAndSetHpText();
        PlayHitEffectIfNotPlayedAlready();
    }

    public virtual void SetColor(byte colorIndex)
    {
        this.ColorIndex = colorIndex;
        lastSetColor = Palette.GetColor(colorIndex);
        elementBaseGraphics.color = lastSetColor;
    }

    public void SetTextColor(bool dark)
    {
        text.color = dark ? Color.black : Color.white;
    }

    public virtual void SetMaxHp(int hp)
    {
        this.HP = hp;
        CalculateAndSetHpText();
    }

    public virtual void ResetTextPosition()
    {
        if (text != null && text.gameObject != null)
            TextGenerator.main.Move(text.gameObject, MiddlePoint);
    }

    public void SetTextObject(Text t)
    {
        text = t;
    }

    protected virtual void CalculateAndSetHpText()
    {
        if (text == null)
            return;
        text.text = Mathf.CeilToInt(HP).ToString();
    }

    public virtual void Break()
    {
        OnDestroyableBroken?.Invoke();
        SquareDataBase.Instance.AddScore();

        GetComponent<SubjectSquare>().OnDeathFunction();
        Destroy(gameObject);
    }

    public void ReleaseCurrentHitEnumerator()
    {
        if (_currentHitEffectEnumerator != null)
            StopCoroutine(_currentHitEffectEnumerator);
        _currentHitEffectEnumerator = null;
    }

    public void PlayHitEffectIfNotPlayedAlready()
    {
        if (_currentHitEffectEnumerator == null)
        {
            _currentHitEffectEnumerator = ElementHitEffectManager.Instance.PlayHitEffectOn(this);
            StartCoroutine(_currentHitEffectEnumerator);
        }
    }
}
