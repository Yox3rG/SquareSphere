using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StageIconDataReceiver : LoopScrollDataReceiver
{
    public static Action<int> OnClickAction = null;

    public StageIcon icon;

    private int index;

    private void Start()
    {
        if(OnClickAction == null)
            OnClickAction = Menu.main.LevelOnClick;

        GetComponent<Button>().onClick.AddListener( delegate { OnClickAction(index); });
    }

    public override void ReceiveDataAndUpdate(int index)
    {
        base.ReceiveDataAndUpdate(index);

        this.index = index;

        bool locked = true;
        byte stars = 0;
        int last = ProfileDataBase.main.GetLastPlayableMap();

        if(last >= index)
        {
            if(last != index)
            {
                stars = ProfileDataBase.main.GetStarsOnMap(index);
            }

            locked = false;
        }

        // Order matters. SetLocked Should be first.
        icon.SetLocked(locked);
        icon.SetState(StageLayout.GetMain().GetStage(index).type);
        icon.SetStageTextAndStars(index, stars);
    }
}