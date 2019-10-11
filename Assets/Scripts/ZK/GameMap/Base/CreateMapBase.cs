using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace ZK.Tool.Map
{
    public abstract class CreateMapBase<T> where T : MapConfig
    {
        private int mapWidth;
        private int mapHeight;

        public abstract void Create(List<T> config, int[,] map);

        private bool IsNside(int x, int y)
        {
            return (x >= 0 && x < this.mapWidth) && (y >= 0 && y < this.mapHeight);
        }


        /// <summary>
        /// 分析地形
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="map"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        protected MapAssetAllowRefreshPosition AnalyzeFloor(int x, int y, int[,] map, int depth)
        {
            this.mapWidth = map.GetLength(0);
            this.mapHeight = map.GetLength(1);

            MapAssetAllowRefreshPosition type = MapAssetAllowRefreshPosition.Road;

            if (this.IsNside(x + depth, y))
            {
                type = ChangeNodeType(x + depth, y, map, type);
            }
            if (this.IsNside(x - depth, y))
            {
                type = ChangeNodeType(x - depth, y, map, type);
            }
            if (this.IsNside(x, y + depth))
            {
                type = ChangeNodeType(x, y + depth, map, type);
            }
            if (this.IsNside(x, y - depth))
            {
                type = ChangeNodeType(x, y - depth, map, type);
            }


            if (this.IsNside(x + depth, y + depth))
            {
                type = ChangeNodeType(x + depth, y + depth, map, type);
            }
            if (this.IsNside(x - depth, y + depth))
            {
                type = ChangeNodeType(x - depth, y + depth, map, type);
            }
            if (this.IsNside(x + depth, y - depth))
            {
                type = ChangeNodeType(x + depth, y - depth, map, type);
            }
            if (this.IsNside(x - depth, y - depth))
            {
                type = ChangeNodeType(x - depth, y - depth, map, type);
            }

            return type;
        }

        private MapAssetAllowRefreshPosition ChangeNodeType(int x, int y, int[,] map, MapAssetAllowRefreshPosition type)
        {
            var tmp = map[x, y];
            if (tmp < 0 && tmp > -1000)
            {
                type = MapAssetAllowRefreshPosition.Water;
            }
            else
            if (tmp <= -1000)
            {
                type = MapAssetAllowRefreshPosition.Mountain;
            }

            return type;
        }

    }
}
