using ICSharpCode.SharpZipLib.GZip;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZK.Tool.Map
{
    public static class RandomMapHelper
    {

        private static List<GameObject> tmpObjectBuffer = new List<GameObject>();

        public static GameObject CreateMapQuickly(MapConfig mapConfig, Vector3 pos)
        {
            GameObject tmpObj = GameObject.Instantiate(mapConfig.mapAsset);
            tmpObj.transform.position = pos;
            tmpObjectBuffer.Add(tmpObj);
            MapInfoManager.Instance.AddMapElementInfo(mapConfig, tmpObj);
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

        public static int AnalysisAssetHighly(int mapPoint)
        {
            if (mapPoint >= 0)
            {
                return 1;
            }
            else if (mapPoint < 0 && mapPoint >= 1000)
            {
                return 1;
            }
            else
            {
                return (Mathf.Abs(mapPoint) / 1000) + 1;
            }
        }

        public static Vector3 GetObjectMehsSize(GameObject obj)
        {
            var tmpMeshSize = obj.GetComponent<MeshFilter>().mesh.bounds.size;

            Vector3 size = new Vector3();
            size.x = tmpMeshSize.x * obj.transform.localScale.x;
            size.y = tmpMeshSize.y * obj.transform.localScale.y;
            size.z = tmpMeshSize.z * obj.transform.localScale.z;

            return size;
        }

        public static Vector3Int GetObjectMehsSizeToInt(GameObject obj)
        {
            var tmp = GetObjectMehsSize(obj);
            return new Vector3Int(Mathf.CeilToInt(tmp.x), Mathf.CeilToInt(tmp.y), Mathf.CeilToInt(tmp.z));
        }


        /// <summary>
        /// 压缩二进制文件到GZip
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static byte[] CompressGZip(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipOutputStream compressedzipStream = new GZipOutputStream(ms);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }
    }
}
