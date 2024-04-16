using System.Collections.Generic;
using UnityEngine;



public class DamageCalculator : MonoBehaviour
{
    public static DamageCalculator Instance = null;

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
        }
    }

    private void Start()
    { }

    public float CalculateBallDamage(IAttributeList attacker, IAttributeList target)
    {
        if (attacker == null)
            return 0f;

        Bonus allBonus = attacker.CombineWith(target, AttributeUserType.BALL);
        //Debug.Log(allBonus);
        float damage =  allBonus.GetResultWhenAppliedTo(
            AttributeDataBase.Instance.GetValueAttribute(
                Attribute.Type.BALL_DAMAGE).CurrentValue);
#if UNITY_EDITOR
        Debug.Log("BallDamage: " + damage);
#endif

        return damage;
    }

    public float CalculatePUDamage(IAttributeList attacker,
        IAttributeList target, Attribute.Type type)
    {
        if (attacker == null)
            return 0f;

        AttributeUserType temp = AttributeUserType.PU_MINILASER;
        switch (type)
        {
            case Attribute.Type.PU_DAMAGE_MINILASER:
                temp = AttributeUserType.PU_MINILASER;
                break;
            case Attribute.Type.PU_DAMAGE_LASER:
                temp = AttributeUserType.SE_LASER;
                break;
            case Attribute.Type.PU_DAMAGE_EXPLOSION:
                temp = AttributeUserType.SE_BOMB;
                break;
        }

        Bonus allBonus = attacker.CombineWith(target, temp);

        float damage = allBonus.GetResultWhenAppliedTo(
            AttributeDataBase.Instance.GetValueAttribute(type).CurrentValue);

#if UNITY_EDITOR
        Debug.Log("PUDamage: " + damage);
#endif

        return damage;
    }
}
