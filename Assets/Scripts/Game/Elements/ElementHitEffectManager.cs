using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementHitEffectManager : MonoBehaviour
{
    public static ElementHitEffectManager Instance { get; private set; } = null;

    public float hitEffectDuration;
    public AnimationCurve hitSizeChangeCurve;

    public bool useColor;
    public Color hitColorTarget;
    public AnimationCurve hitColorChangeCurve;

    public bool useSaturation;
    public float hitSaturationIncreasePercent;
    public AnimationCurve hitSaturationChangeCurve;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public IEnumerator PlayHitEffectOn(Destroyable destroyable)
    {
        Color.RGBToHSV(destroyable.lastSetColor, out float h, out float s, out float v);
        float sBase = s;
        float sTarget = s + hitSaturationIncreasePercent;

        for (float t = 0; t < 1f; t += Time.deltaTime / hitEffectDuration)
        {
            if (useColor)
                destroyable.elementBaseGraphics.color = Color.Lerp(destroyable.lastSetColor, hitColorTarget, hitColorChangeCurve.Evaluate(t));
            if (useSaturation)
            {
                s = Mathf.Lerp(sBase, sTarget, hitSaturationChangeCurve.Evaluate(t));
                destroyable.elementBaseGraphics.color = Color.HSVToRGB(h, s, v);
            }

            destroyable.elementBaseGraphics.transform.localScale = destroyable.DefaultScale * hitSizeChangeCurve.Evaluate(t);
            yield return null;
        }

        if (useColor)
            destroyable.elementBaseGraphics.color = destroyable.lastSetColor;
        if (useSaturation)
        {
            destroyable.elementBaseGraphics.color = Color.HSVToRGB(h, sBase, v);
        }

        destroyable.elementBaseGraphics.transform.localScale = destroyable.DefaultScale;

        destroyable.ReleaseCurrentHitEnumerator();
    }
}
