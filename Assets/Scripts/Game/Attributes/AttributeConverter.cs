using UnityEngine;


public class AttributeConverter : MonoBehaviour
{
    public static AttributeConverter Instance = null;

    private System.Random randomGenerator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        randomGenerator = new System.Random();
    }

    public int Get(Attribute.Type type)
    {
        ValueAttribute attribute = AttributeDataBase.Instance.GetValueAttribute(type);

        return FloatToInt(attribute.CurrentValue);
    }

    public int ApplyBonusToNumberOfBalls(int stageBalls)
    {
        ValueAttribute attribute = AttributeDataBase.Instance.GetValueAttribute(Attribute.Type.NUMBER_OF_BALLS);

        return FloatToInt(attribute.GetBonus().GetResultWhenAppliedTo(stageBalls));
    }

    private int FloatToInt(float value)
    {
        float percent = value - Mathf.Floor(value);
        
        int result = percent < randomGenerator.NextDouble() ? Mathf.FloorToInt(value) : Mathf.CeilToInt(value);

        return result;
    }
}