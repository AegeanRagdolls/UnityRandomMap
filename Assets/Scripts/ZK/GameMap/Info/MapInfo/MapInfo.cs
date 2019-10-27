using System.Collections.Generic;

namespace ZK.Tool.Map
{
    public class MapInfo
    {
        public List<MapElementInfo> mapInfoList;

        public bool[,] map;
    }

    /// <summary>
    /// 
    /// </summary>
    public class MapElementInfo
    {

        public ushort assetID;

        public short x;
        public short y;
        public short z;

        /// <summary>
        /// 对象类型
        /// </summary>
        public byte objectType;

        /// <summary>
        /// 是否为基础元素 (地板 , 水 , 山)
        /// </summary>
        public bool isBaseElmenet;

        /// <summary>
        /// 是否允许破坏
        /// </summary>
        public bool isAllowDestruction;
    }

    /// <summary>
    /// 资源预制体
    /// </summary>
    public class PrefabAssetData
    {
        /// <summary>
        /// 资源ID
        /// </summary>
        public ushort assetID;

        /// <summary>
        /// 资源名字
        /// </summary>
        public string name;

        /// <summary>
        /// 资源路径
        /// </summary>
        public string appointPath;

    }

    public class PrefabData
    {
        public ushort assetID;

        public List<ushort> meshList = new List<ushort>();

        public List<ushort> textureList = new List<ushort>();

        public List<PrefabData> chileList = new List<PrefabData>();

        public double postionX;
        public double postionY;
        public double postionZ;

        public double scaleX;
        public double scaleY;
        public double scaleZ;

        public double eulerX;
        public double eulerY;
        public double eulerZ;
    }


}
