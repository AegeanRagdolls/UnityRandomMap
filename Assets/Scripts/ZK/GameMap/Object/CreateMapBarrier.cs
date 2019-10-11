using System.Collections.Generic;
using UnityEngine;
namespace ZK.Tool.Map
{
    public class CreateMapBarrier : CreateMapBase<MapBarrierConfig>
    {
        public override void Create(List<MapBarrierConfig> config, int[,] map)
        {
            foreach (var item in config)
            {
                switch (item.type)
                {
                    case BarrierType.Mountain:
                        this.CreateMountain(item, map);
                        break;
                    case BarrierType.Water:
                        this.CreateWater(item, map);
                        break;
                    case BarrierType.Else:
                        break;
                    case BarrierType.Footstone:
                        CreateFootstone(item, map);
                        break;
                }
            }
        }
        private void CreateFootstone(MapBarrierConfig config, int[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    var tmp = map[x, y];
                    if (tmp <= -1000)
                    {
                        var h = Mathf.Abs(tmp) / 1000;
                        for (int i = 0; i < h; i++)
                        {
                            RandomMapHelper.CreateMapQuickly(config.mapAsset, new UnityEngine.Vector3(x, i, y));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 创建山
        /// </summary>
        private void CreateMountain(MapBarrierConfig config, int[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    var tmp = map[x, y];
                    if (tmp <= -1000)
                    {
                        var h = Mathf.Abs(tmp) / 1000;
                        RandomMapHelper.CreateMapQuickly(config.mapAsset, new UnityEngine.Vector3(x, h, y));
                    }
                }
            }

        }

        /// <summary>
        /// 创建水
        /// </summary>
        private void CreateWater(MapBarrierConfig config, int[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    var tmp = map[x, y];
                    if (tmp < 0 && tmp > -1000)
                    {
                        RandomMapHelper.CreateMapQuickly(config.mapAsset, new UnityEngine.Vector3(x, 0, y));
                    }
                }
            }
        }





    }
}
