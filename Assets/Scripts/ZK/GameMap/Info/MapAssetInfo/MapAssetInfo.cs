using System;
using System.Collections.Generic;
using UnityEngine;


namespace ZK.Tool.Map
{
    /// <summary>
    /// 障碍物类型
    /// </summary>
    public enum BarrierType
    {
        /// <summary>
        /// 山
        /// </summary>
        Mountain,
        /// <summary>
        /// 水
        /// </summary>
        Water,
        /// <summary>
        /// 其他
        /// </summary>
        Else,
        /// <summary>
        /// 基石
        /// </summary>
        Footstone
    }

    /// <summary>
    /// 允许刷新的位置
    /// </summary>
    public enum MapAssetAllowRefreshPosition
    {
        /// <summary>
        /// 山
        /// </summary>
        Mountain,
        /// <summary>
        /// 水
        /// </summary>
        Water,
        /// <summary>
        /// 道路
        /// </summary>
        Road,
        /// <summary>
        /// 全部
        /// </summary>
        All,
    }

    [CreateAssetMenu(fileName = "MapAssetInfo", menuName = "Config/Map/MapAssetInfo")]
    public class MapAssetInfo : ScriptableObject
    {
        /// <summary>
        /// 道路列表
        /// </summary>
        [Header("===========道路资源============")]
        public List<MapFloorConfig> mapFloorConfigs;
        /// <summary>
        /// 障碍物列表
        /// </summary>
        [Header("===========障碍物资源============")]
        public List<MapBarrierConfig> mapBarrierConfigs;
        /// <summary>
        /// 资源列表
        /// </summary>
        [Header("===========地图其他资源============")]
        public List<MapAssetConfig> mapAssetConfigs;
    }

    /// <summary>
    /// 地图资源设置
    /// </summary>
    [Serializable]
    public class MapAssetConfig : MapConfig
    {
        /// <summary>
        /// 生成数量
        /// </summary>
        public int generateNumber = 100;

        /// <summary>
        /// 最小间距
        /// </summary>
        public float minDistance = 10;

        /// <summary>
        /// 允许出生的位置
        /// </summary>
        public MapAssetAllowRefreshPosition mapAssetAllowRefreshPosition;

        /// <summary>
        /// 是否会变成障碍物
        /// </summary>
        public bool isBecomeObstacles = false;

    }

    /// <summary>
    /// 允许移动的道路
    /// </summary>
    [Serializable]
    public class MapFloorConfig : MapConfig
    {
        /// <summary>
        /// 是主要的
        /// </summary>
        public bool isPrimary;

    }


    /// <summary>
    /// 障碍物
    /// </summary>
    [Serializable]
    public class MapBarrierConfig : MapConfig
    {
        /// <summary>
        /// 深度
        /// </summary>
        public float depth;

        public BarrierType type = BarrierType.Water;
    }



    public class MapConfig
    {
        public ushort assetId;

        public string tag;

        public GameObject mapAsset;

        public List<DepthInfo> depthInfos;
    }

    [Serializable]
    public class DepthInfo
    {
        public int depth = 1;
        public MapAssetAllowRefreshPosition mapAssetAllowRefreshPosition = MapAssetAllowRefreshPosition.Road;
        [Range(1, 100)]
        public int refreshRate = 100;
    }




}

