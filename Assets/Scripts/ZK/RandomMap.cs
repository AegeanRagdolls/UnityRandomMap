using Random = UnityEngine.Random;

namespace ZK.Tool.Map
{
    /// <summary>
    /// 随机地图
    /// </summary>
    public class RandomMap
    {
        /// <summary>
        /// 地图高
        /// </summary>
        public int mapHeight = 50;
        /// <summary>
        /// 地图宽
        /// </summary>
        public int mapWidth = 50;

        /// <summary>
        /// 激活几率
        /// </summary>
        public float chanceToStartAlive = 0.45f;

        /// <summary>
        /// 附近超过多少激活
        /// </summary>
        public int birthLimit = 4;

        /// <summary>
        /// 附近超过多少冻结
        /// </summary>
        public int deathLimit = 3;

        /// <summary>
        /// 迭代次数
        /// </summary>
        public float numberOfSteps = 2;

        /// <summary>
        /// 生成地图
        /// </summary>
        /// <returns></returns>
        public bool[,] GenerateMap()
        {
            bool[,] cellmap = new bool[this.mapWidth, this.mapHeight];
            cellmap = this.InitMap(cellmap);

            for (int i = 0; i < this.numberOfSteps; i++)
            {
                cellmap = this.DoSiimulationStep(cellmap);
            }

            return cellmap;
        }

        /// <summary>
        /// 初始化地图细胞
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private bool[,] InitMap(bool[,] map)
        {
            for (int x = 0; x < this.mapWidth; x++)
            {
                for (int y = 0; y < this.mapHeight; y++)
                {
                    if (Random.Range((float)0, (float)1) < this.chanceToStartAlive)
                    {
                        map[x, y] = true;
                    }
                }
            }
            return map;
        }

        /// <summary>
        /// 迭代地图
        /// </summary>
        /// <param name="oldMap"></param>
        /// <returns></returns>
        private bool[,] DoSiimulationStep(bool[,] oldMap)
        {
            bool[,] newMap = new bool[this.mapWidth, this.mapHeight];

            for (int x = 0; x < oldMap.GetLength(0); x++)
            {
                for (int y = 0; y < oldMap.GetLength(1); y++)
                {
                    int nbs = this.CountAliveNeighbours(oldMap, x, y);

                    if (oldMap[x, y])
                    {
                        if (nbs < this.deathLimit)
                        {
                            newMap[x, y] = false;
                        }
                        else
                        {
                            newMap[x, y] = true;
                        }
                    }
                    else
                    {
                        if (nbs > this.birthLimit)
                        {
                            newMap[x, y] = true;
                        }
                        else
                        {
                            newMap[x, y] = false;
                        }
                    }
                }
            }
            return newMap;
        }

        /// <summary>
        /// 计算活着的邻居细胞
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CountAliveNeighbours(bool[,] map, int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int neighbour_x = x + i;
                    int neighbour_y = y + j;
                    if (i == 0 && j == 0)
                        continue;
                    else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(0) || neighbour_y >= map.GetLength(1))
                    {
                        count += 1;
                    }
                    else if (map[neighbour_x, neighbour_y])
                    {
                        count += 1;
                    }
                }
            }
            return count;
        }
    }
}
