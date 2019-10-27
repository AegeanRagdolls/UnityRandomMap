using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

namespace ZK.Tool.Map
{
    public class MapInfoManager
    {
        private static MapInfoManager instance;
        public static MapInfoManager Instance { get => instance ?? (instance = new MapInfoManager()); }
        private MapInfoManager() { }


        List<MapElementInfo> tmpElementInfos = new List<MapElementInfo>();

        MapInfo tmpMapInfo;

        public void AddMapElementInfo(MapConfig mapConfig, GameObject obj)
        {
            MapElementInfo tmpInfo = new MapElementInfo();
            tmpInfo.assetID = mapConfig.assetId;
            tmpInfo.x = (short)obj.transform.position.x;
            tmpInfo.y = (short)obj.transform.position.y;
            tmpInfo.z = (short)obj.transform.position.z;
            tmpElementInfos.Add(tmpInfo);
        }

        public MapInfo SetMapInfo(bool[,] map)
        {
            this.tmpMapInfo = new MapInfo();
            this.tmpMapInfo.map = map;
            this.tmpMapInfo.mapInfoList = this.tmpElementInfos;
            return this.tmpMapInfo;
        }
        public void ClearTmpList()
        {
            tmpElementInfos.Clear();
        }

        public string GetMapInfoJson()
        {
            var json = LitJson.JsonMapper.ToJson(tmpMapInfo);
            return json;
        }

        public void SaveMapInfoByByte(string path, string fileName)
        {
            path += "/" + fileName;
            var buf = ByteBuffer.Allocate(512);
            buf.WriteInt(this.tmpMapInfo.mapInfoList.Count);
            Debug.Log(this.tmpMapInfo.mapInfoList.Count);
            foreach (var item in this.tmpMapInfo.mapInfoList)
            {
                buf.WriteUshort(item.assetID);
                buf.WriteShort(item.x);
                buf.WriteShort(item.y);
                buf.WriteShort(item.z);
                buf.WriteByte(item.objectType);
                buf.WriteBoolean(item.isAllowDestruction);
                buf.WriteBoolean(item.isBaseElmenet);
            }
            buf.WriteInt(this.tmpMapInfo.map.GetLength(0));
            buf.WriteInt(this.tmpMapInfo.map.GetLength(1));
            for (int x = 0; x < this.tmpMapInfo.map.GetLength(0); x++)
            {
                for (int y = 0; y < this.tmpMapInfo.map.GetLength(1); y++)
                {
                    buf.WriteBoolean(this.tmpMapInfo.map[x, y]);
                }
            }
            var data = RandomMapHelper.CompressGZip(buf.ToArray());
            using (FileStream s = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                s.Write(data, 0, data.Length);
            }

            Debug.Log(buf.ToArray().Length);
            Debug.Log(RandomMapHelper.CompressGZip(buf.ToArray()).Length);
        }

    }
}
