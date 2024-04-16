namespace UnityEngine.UI
{
    //  ########### Yox ########### 
    public class LoopScrollDataReceiver : MonoBehaviour
    {
        public virtual void ReceiveDataAndUpdate(int index)
        {
            name = "Cell | " + index.ToString();
        }
    }
    //  ########### Yox ########### 
}