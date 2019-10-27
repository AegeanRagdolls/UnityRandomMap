using System.Collections.Generic;
using UnityEngine;

namespace ZK.Tool.Map
{
    public class CreateMapAsset : CreateMapBase<MapAssetConfig>
    {
        bool[,] tmpMap;
        Dictionary<MapConfig, List<GameObject>> dicTmpData;
        public override void Create(List<MapAssetConfig> config, int[,] map)
        {
            this.tmpMap = new bool[map.GetLength(0), map.GetLength(1)];
            this.dicTmpData = new Dictionary<MapConfig, List<GameObject>>();
            var typeList = Classify(map);
            foreach (var item in config)
            {
                switch (item.mapAssetAllowRefreshPosition)
                {
                    case MapAssetAllowRefreshPosition.Mountain:
                        RandomAsset(item, typeList[0], map);
                        break;
                    case MapAssetAllowRefreshPosition.Water:
                        RandomAsset(item, typeList[1], map);
                        break;
                    case MapAssetAllowRefreshPosition.Road:
                        RandomAsset(item, typeList[2], map);
                        break;
                    case MapAssetAllowRefreshPosition.All:
                        break;
                }
            }
        }

        private List<List<Vector2Int>> Classify(int[,] map)
        {
            List<List<Vector2Int>> tmp = new List<List<Vector2Int>>();
            var mountainList = new List<Vector2Int>();
            var waterList = new List<Vector2Int>();
            var roadList = new List<Vector2Int>();
            tmp.Add(mountainList);
            tmp.Add(waterList);
            tmp.Add(roadList);
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    var tmpPoint = map[x, y];
                    if (tmpPoint >= 0)
                    {
                        roadList.Add(new Vector2Int(x, y));
                    }
                    else if (tmpPoint < 0 && tmpPoint > -1000)
                    {
                        waterList.Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        mountainList.Add(new Vector2Int(x, y));
                    }
                }
            }

            return tmp;
        }


        private void RandomAsset(MapAssetConfig config, List<Vector2Int> map, int[,] baseMap)
        {
            int loopNumber = 0;
            bool isOK = true;
            if (!dicTmpData.TryGetValue(config, out List<GameObject> objs))
            {
                objs = new List<GameObject>();
                dicTmpData.Add(config, objs);
            }
            int quantity = config.generateNumber;
            while (isOK)
            {
                if (map.Count == 0 || quantity == 0)
                {
                    return;
                }
                var randomPoint = Random.Range(0, map.Count);
                var mapPoint = map[randomPoint];
                if (!this.tmpMap[mapPoint.x, mapPoint.y])
                {
                    bool isCreate = true;
                    foreach (var item in objs)
                    {
                        if (Vector3.Distance(item.transform.position, new Vector3(mapPoint.x, 1, mapPoint.y)) < config.minDistance)
                        {
                            isCreate = false;
                            continue;
                        }
                    }
                    if (isCreate)
                    {
                        var h = RandomMapHelper.AnalysisAssetHighly(baseMap[mapPoint.x, mapPoint.y]);
                        var obj = RandomMapHelper.CreateMapQuickly(config, new Vector3(mapPoint.x, h, mapPoint.y));
                        objs.Add(obj);
                        this.tmpMap[mapPoint.x, mapPoint.y] = true;
                        quantity--;
                    }
                }
                loopNumber++;
                if (quantity <= 0 || map.Count < objs.Count * config.minDistance || loopNumber > 1000)
                {
                    isOK = false;
                }

            }

        }



    }
}
