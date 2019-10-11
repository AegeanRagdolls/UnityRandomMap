using System.Collections.Generic;
using UnityEngine;

namespace ZK.Tool.Map
{
    public static class RandomMapHelper
    {

        private static List<GameObject> tmpObjectBuffer = new List<GameObject>();

        public static GameObject CreateMapQuickly(GameObject obj, Vector3 pos)
        {
            GameObject tmpObj = GameObject.Instantiate(obj);
            tmpObj.transform.position = pos;
            tmpObjectBuffer.Add(tmpObj);
            return tmpObj;
        }

        public static void ClearTmpObjectBuffer()
        {
            foreach (var item in tmpObjectBuffer)
            {
                GameObject.Destroy(item);
            }
            tmpObjectBuffer.Clear();
        }

    }
}
