using System.Collections.Generic;
using UnityEngine;

namespace ZK.Tool.Map
{
    public class CreateMapFloor : CreateMapBase<MapFloorConfig>
    {
        public override void Create(List<MapFloorConfig> config, int[,] map)
        {
            bool[,] tmpMap = new bool[map.GetLength(0), map.GetLength(1)];

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] > 0)
                    {
                        tmpMap[x, y] = true;
                    }
                }
            }

            foreach (var item in config)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        foreach (var e in item.depthInfos)
                        {
                            if (map[x, y] == e.depth && tmpMap[x, y] && this.AnalyzeFloor(x, y, map, e.depth) == e.mapAssetAllowRefreshPosition)
                            {
                                var tmp = Random.Range(1, 101);
                                if (tmp < e.refreshRate)
                                {
                                    RandomMapHelper.CreateMapQuickly(item.mapAsset, new Vector3(x, 0, y));
                                    tmpMap[x, y] = false;
                                }
                            }
                        }
                    }
                }
            }


            foreach (var item in config)
            {
                if (item.isPrimary)
                {
                    for (int x = 0; x < map.GetLength(0); x++)
                    {
                        for (int y = 0; y < map.GetLength(1); y++)
                        {
                            if (tmpMap[x, y])
                            {
                                RandomMapHelper.CreateMapQuickly(item.mapAsset, new Vector3(x, 0, y));
                            }
                        }
                    }
                }

            }
        }

    }
}
