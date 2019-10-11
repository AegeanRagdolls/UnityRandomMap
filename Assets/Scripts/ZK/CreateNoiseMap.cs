using UnityEngine;
using Random = UnityEngine.Random;

namespace ZK.Tool.Map
{
    /// <summary>
    /// 创建噪声贴图
    /// </summary>
    public class CreateNoiseMap
    {
        #region 构造
        /// <summary>
        /// 创建噪点图
        /// </summary>
        /// <param name="randomSood"></param>
        /// <param name="mapSize"></param>
        public CreateNoiseMap(Vector2 randomSood, Vector2Int mapSize)
        {
            this.randomSood = randomSood;
            this.mapSize = mapSize;
        }
        public CreateNoiseMap(Vector2Int mapSize)
        {
            this.mapSize = mapSize;
        }
        public CreateNoiseMap()
        {

        }
        #endregion

        /// <summary>
        /// 获取高度图大小
        /// </summary>
        public Vector2Int GetMapSize { get => this.mapSize; }

        /// <summary>
        /// 获取当前随机数种子
        /// </summary>
        public Vector2 GetRandomSood { get => this.randomSood; }


        /// <summary>
        /// 随机数种子
        /// </summary>
        private Vector2 randomSood = new Vector2(100f, 100f);
        /// <summary>
        /// 地图大小
        /// </summary>
        private Vector2Int mapSize = new Vector2Int(15, 15);

        /// <summary>
        /// 最大高度
        /// </summary>
        public int maxHeight = 10;

        /// <summary>
        /// 是否平滑
        /// </summary>
        public bool IsSmoothness { get; set; } = false;

        public float Relief { get; set; } = 15;
        private float soodX = 0;
        private float soodY = 0;

        /// <summary>
        /// 获取噪点图
        /// </summary>
        /// <returns></returns>
        public float[,] GetNoisMap()
        {
            float[,] buffer = new float[this.mapSize.x, this.mapSize.y];
            this.soodX = Random.value * this.randomSood.x;
            this.soodY = Random.value * this.randomSood.y;
            for (int x = 0; x < this.mapSize.x; x++)
            {
                for (int y = 0; y < this.mapSize.y; y++)
                {
                    buffer[x, y] = this.GetHeight(new Vector2(x, y));
                }
            }
            return buffer;
        }

        /// <summary>
        ///  获取高度
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float GetHeight(Vector2 pos)
        {
            float xSample = (pos.x + this.soodX) / this.Relief;
            float ySample = (pos.y + this.soodY) / this.Relief;
            float noise = Mathf.PerlinNoise(xSample, ySample);
            float y = this.maxHeight * noise;

            if (!this.IsSmoothness)
            {
                y = Mathf.Round(y);
            }
            return y;
        }
    }
}
