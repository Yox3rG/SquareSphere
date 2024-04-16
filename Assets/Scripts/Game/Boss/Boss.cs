using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Destroyable
{
    [SerializeField] protected GameObject leftEye;
    [SerializeField] protected GameObject rightEye;
    [SerializeField] protected GameObject mouth;

    public static Action<System.Type> OnBossBroken;

    public virtual bool IsAlive { get { return HP > 0; } }
    public virtual Vector3 TextOffset { get { return Vector3.zero; } }
    public virtual Vector3 TextPosition { get { return transform.position + TextOffset; } }

    public override Vector3 DefaultScale => new Vector3(4f, 4f, 1f);

    public override void Break()
    {
        //ProfileDataBase.main?.StatIncreaseBlocksBroken();
        //SquareDataBase.main.AddScore();

        //GetComponent<SubjectSquare>().OnDeathFunction();

        OnBossBroken?.Invoke(this.GetType());

        Destroy(gameObject);
    }
}
