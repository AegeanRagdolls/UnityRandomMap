using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZK.Tool.Map
{
    /// <summary>
    /// 优化地图
    /// </summary>
    public class OptimizeMap
    {
        private static OptimizeMap instance;
        public static OptimizeMap Instance { get => instance ?? (instance = new OptimizeMap()); }

        private OptimizeMap()
        {

        }

        private int mapWidth;
        private int mapHeight;

        /// <summary>
        /// 陆地map
        /// </summary>
        public Dictionary<string, Vector2Int> dicLand = new Dictionary<string, Vector2Int>();
        /// <summary>
        /// 障碍物Map
        /// </summary>
        public Dictionary<string, Vector2Int> dicBarrier = new Dictionary<string, Vector2Int>();


        /// <summary>
        /// 连接陆地
        /// </summary>
        /// <param name="map"></param>
        public void LinkLand(bool[,] map, int deleteLessValueArea = 0)
        {
            this.mapWidth = map.GetLength(0);
            this.mapHeight = map.GetLength(1);
            List<List<Vector2Int>> lands = new List<List<Vector2Int>>();
            MapAnalysis(map);
            FindAllLandOrBarrier(map, lands);

            if (lands.Count == 1) return;

            for (int i = 1; i < lands.Count; i++)
            {
                if (lands[0].Count < lands[i].Count)
                {
                    var tmpList = lands[0];
                    lands[0] = lands[i];
                    lands[i] = tmpList;
                }
            }

            var mainLands = lands[0];

            for (int i = 1; i < lands.Count; i++)
            {
                var tmpLands = lands[i];
                if (tmpLands.Count < deleteLessValueArea)
                {
                    Debug.Log("删除岛屿");
                    DeleteLand(map, tmpLands);
                    continue;
                }

                float dir = 0;

                Vector2Int mainPoint = Vector2Int.zero;
                Vector2Int tmpPoint = Vector2Int.zero;
                foreach (var item in mainLands)
                {
                    foreach (var element in tmpLands)
                    {
                        var tmpDir = Vector2Int.Distance(new Vector2Int(element.x, element.y), new Vector2Int(item.x, item.y));
                        if (dir == 0) dir = tmpDir;
                        else if (tmpDir <= dir)
                        {
                            dir = tmpDir;
                            mainPoint = new Vector2Int(item.x, item.y);
                            tmpPoint = new Vector2Int(element.x, element.y);
                        }
                    }
                }
                Link(map, mainPoint, tmpPoint, dir);
            }
            lands.Clear();
            MapAnalysis(map);
            FindAllLandOrBarrier(map, lands);
            if (lands.Count > 1)
            {
                this.LinkLand(map, deleteLessValueArea);
            }
        }


        /// <summary>
        /// 解析地图
        /// </summary>
        /// <param name="map"></param>
        public void MapAnalysis(bool[,] map)
        {
            dicLand.Clear();
            dicBarrier.Clear();
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    var key = $"{x}:{y}";
                    if (map[x, y])
                    {
                        this.dicBarrier.Add(key, new Vector2Int(x, y));
                    }
                    else
                    {
                        this.dicLand.Add(key, new Vector2Int(x, y));
                    }
                }
            }
        }

        /// <summary>
        /// 发现所有陆地或者障碍物
        /// </summary>
        /// <param name="lands"></param>
        public void FindAllLandOrBarrier(bool[,] map, List<List<Vector2Int>> lands, bool isLand = true)
        {

            Vector2Int startPoint;
            if (isLand)
            {
                if (this.dicLand.Count == 0)
                    return;
                startPoint = this.dicLand.Values.First(); ;
                var tmpList = FindRegion(map, startPoint.x, startPoint.y, false);
                lands.Add(tmpList);
            }
            else
            {
                if (this.dicBarrier.Count == 0)
                    return;
                startPoint = this.dicBarrier.Values.First(); ;
                var tmpList = FindRegion(map, startPoint.x, startPoint.y, true);
                lands.Add(tmpList);
            }
            FindAllLandOrBarrier(map, lands, isLand);
        }

        /// <summary>
        /// 查找地区
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="findPoint"></param>
        /// <returns></returns>
        private List<Vector2Int> FindRegion(bool[,] map, int x, int y, bool findPoint)
        {
            var mapCopy = (bool[,])map.Clone();
            List<Vector2Int> region = new List<Vector2Int>();
            FloodFill(region, mapCopy, x, y, findPoint);
            return region;
        }

        /// <summary>
        /// 洪水填充
        /// </summary>
        /// <param name="deposit"></param>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="findPoint"></param>
        private void FloodFill(List<Vector2Int> deposit, bool[,] map, int x, int y, bool findPoint)
        {
            if (x < 0 || x >= this.mapWidth || y < 0 || y >= this.mapHeight) return;
            if (map[x, y] != findPoint)
            {
                return;
            }
            map[x, y] = !findPoint;
            deposit.Add(new Vector2Int(x, y));
            FloodFill(deposit, map, x + 1, y, findPoint);
            FloodFill(deposit, map, x - 1, y, findPoint);
            FloodFill(deposit, map, x, y - 1, findPoint);
            FloodFill(deposit, map, x, y + 1, findPoint);

            var key = $"{x}:{y}";
            if (findPoint)
            {
                FloodFill(deposit, map, x + 1, y + 1, findPoint);
                FloodFill(deposit, map, x + 1, y - 1, findPoint);
                FloodFill(deposit, map, x - 1, y + 1, findPoint);
                FloodFill(deposit, map, x - 1, y - 1, findPoint);
                dicBarrier.Remove(key);
            }
            else
            {
                dicLand.Remove(key);
            }
        }

        private void DeleteLand(bool[,] map, List<Vector2Int> v)
        {
            foreach (var item in v)
            {
                map[item.x, item.y] = true;
            }
        }

        private void Link(bool[,] map, Vector2Int mainPoint, Vector2Int tmpPoint, float dir)
        {

            Vector2Int tmp = tmpPoint;
            for (int i = 0; i < Mathf.CeilToInt(dir); i++)
            {
                var f = ((Vector2)(mainPoint - tmp)).normalized;

                if (f.x > 0)
                {
                    tmp.x += Mathf.CeilToInt(Mathf.Abs(f.x));
                }
                else
                {
                    tmp.x -= Mathf.CeilToInt(Mathf.Abs(f.x));
                }
                if (f.y > 0)
                {
                    tmp.y += Mathf.CeilToInt(Mathf.Abs(f.y));

                }
                else
                {
                    tmp.y -= Mathf.CeilToInt(Mathf.Abs(f.y));
                }

                map[tmp.x, tmp.y] = false;
                if (f.y == 0)
                {
                    if (tmp.y > 0)
                    {
                        map[tmp.x, tmp.y - 1] = false;
                    }
                    if (tmp.y + 1 < this.mapHeight)
                    {
                        map[tmp.x, tmp.y + 1] = false;
                    }
                }
                else
                {
                    if (tmp.x + 1 < this.mapWidth)
                    {
                        map[tmp.x + 1, tmp.y] = false;
                    }
                    if (tmp.x - 1 >= 0)
                    {
                        map[tmp.x - 1, tmp.y] = false;
                    }
                }
            }
        }
    }
}
