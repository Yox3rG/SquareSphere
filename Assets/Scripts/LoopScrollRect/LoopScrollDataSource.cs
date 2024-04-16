using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public abstract class LoopScrollDataSource
    {
    //  ########### Yox ########### 
        public abstract void ProvideData(LoopScrollDataReceiver receiver, int idx);
    }

    public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
		public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

		LoopScrollSendIndexSource(){}

        public override void ProvideData(LoopScrollDataReceiver receiver, int idx)
        {
            if (receiver == null)
                return;

            receiver.ReceiveDataAndUpdate(idx);
        }
    }
    //  ########### Yox ########### 
}