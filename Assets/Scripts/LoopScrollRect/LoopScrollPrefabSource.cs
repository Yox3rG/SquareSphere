using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabObject;
        public int poolSize = 5;

        private bool inited = false;
        public virtual GameObject GetObject()
        {
            if(!inited)
            {
                SG.ResourceManager.Instance.InitPool(prefabObject, poolSize);
                inited = true;
            }
            return SG.ResourceManager.Instance.GetObjectFromPool(prefabObject);
        }

        public virtual void ReturnObject(Transform go)
        {
            //  ########### Yox ########### 
            //  ########### Yox ########### 
            SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
        }
    }
}
