using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZK.Tool.Map
{
    public class CreateMapPiece
    {
        private static CreateMapPiece instance;
        public static CreateMapPiece Instance { get => instance ?? (instance = new CreateMapPiece()); }

        private int mapHeight;
        private int mapWidth;

        private int[,] newMap;

        public void CreateMap(MapAssetInfo mapAssetInfo, bool[,] map)
        {
            RandomMapHelper.ClearTmpObjectBuffer();
            this.DisposeTestObject();

            this.mapWidth = map.GetLength(0);
            this.mapHeight = map.GetLength(1);
            this.newMap = new int[this.mapWidth, this.mapHeight];

            OptimizeMap.Instance.MapAnalysis(map);

            List<List<Vector2Int>> floor = new List<List<Vector2Int>>();
            List<List<Vector2Int>> barriers = new List<List<Vector2Int>>();

            OptimizeMap.Instance.FindAllLandOrBarrier(map, floor);
            OptimizeMap.Instance.FindAllLandOrBarrier(map, barriers, false);

            this.AnalysisMap(floor, barriers);

            CreateMapFloor cmf = new CreateMapFloor();
            cmf.Create(mapAssetInfo.mapFloorConfigs, this.newMap);

            CreateMapBarrier cmb = new CreateMapBarrier();
            cmb.Create(mapAssetInfo.mapBarrierConfigs, this.newMap);

            CreateMapAsset cma = new CreateMapAsset();
            cma.Create(mapAssetInfo.mapAssetConfigs, this.newMap);
            //this.CreateFloor(mapAssetInfo, map, floor);
            //this.CreateBarriers(mapAssetInfo, map, barriers);
            //this.CreateAssets(mapAssetInfo, map, floor, barriers);

       
        }


        private void AnalysisMap(List<List<Vector2Int>> floor, List<List<Vector2Int>> barriers)
        {
            this.AnalysisBarriers(barriers);
            this.AnalysisMapDepth(floor, 10);
        }


        /// <summary>
        /// 分析障碍物
        /// </summary>
        /// <param name="map"></param>
        private void AnalysisBarriers(List<List<Vector2Int>> map)
        {
            CreateNoiseMap createNoiseMap = new CreateNoiseMap(new Vector2Int(this.mapWidth, this.mapHeight));
            createNoiseMap.maxHeight = 3;
            var hMap = createNoiseMap.GetNoisMap();
            foreach (var item in map)
            {
                var tmp = item.Where((v) => { return v.x < 2 || v.y < 2 || v.y > this.mapHeight - 2 || v.x > this.mapWidth - 2; });
                if (tmp.Count() > 0)
                {
                    foreach (var element in item)
                    {
                        var h = hMap[element.x, element.y];
                        if (h == 0)
                        {
                            h = 1;
                        }
                        this.newMap[element.x, element.y] = -1 * (int)h;
                        TestText(element, this.newMap[element.x, element.y]);
                    }
                }
                else
                {
                    foreach (var element in item)
                    {
                        var h = hMap[element.x, element.y];
                        if (h == 0)
                        {
                            h = 1;
                        }
                        this.newMap[element.x, element.y] = -1000 * (int)h;
                        TestText(element, this.newMap[element.x, element.y]);
                    }
                }
            }
        }

        private bool IsNside(int x, int y)
        {
            return (x >= 0 && x < this.mapWidth) && (y >= 0 && y < this.mapHeight);
        }


        /// <summary>
        /// 分析地图深度
        /// </summary>
        /// <param name="map"></param>
        public void AnalysisMapDepth(List<List<Vector2Int>> terrainMap, int maxDepth = 5, bool isEightDirection = false)
        {
            for (int i = 0; i < maxDepth; i++)
            {
                foreach (var item in terrainMap)
                {
                    foreach (var e in item)
                    {
                        var x = e.x;
                        var y = e.y;
                        var tmp = this.newMap[x, y];
                        if (tmp > 0) continue;
                        int marginType = 0;
                        if (this.IsNside(x + 1, y))
                        {
                            if (this.newMap[x + 1, y] != tmp && this.newMap[x + 1, y] != i + 1)
                            {
                                marginType++;
                            }
                        }
                        if (this.IsNside(x - 1, y))
                        {
                            if (this.newMap[x - 1, y] != tmp && this.newMap[x - 1, y] != i + 1)
                            {
                                marginType++;
                            }
                        }
                        if (this.IsNside(x, y + 1))
                        {
                            if (this.newMap[x, y + 1] != tmp && this.newMap[x, y + 1] != i + 1)
                            {
                                marginType++;
                            }
                        }
                        if (this.IsNside(x, y - 1))
                        {
                            if (this.newMap[x, y - 1] != tmp && this.newMap[x, y - 1] != i + 1)
                            {
                                marginType++;
                            }
                        }
                        if (isEightDirection)
                        {
                            if (this.IsNside(x + 1, y + 1))
                            {
                                if (this.newMap[x + 1, y + 1] != tmp && this.newMap[x + 1, y + 1] != i + 1)
                                {
                                    marginType++;
                                }
                            }
                            if (this.IsNside(x - 1, y - 1))
                            {
                                if (this.newMap[x - 1, y - 1] != tmp && this.newMap[x - 1, y - 1] != i + 1)
                                {
                                    marginType++;
                                }
                            }
                            if (this.IsNside(x + 1, y - 1))
                            {
                                if (this.newMap[x + 1, y - 1] != tmp && this.newMap[x + 1, y - 1] != i + 1)
                                {
                                    marginType++;
                                }

                            }
                            if (this.IsNside(x - 1, y + 1))
                            {
                                if (this.newMap[x - 1, y + 1] != tmp && this.newMap[x - 1, y + 1] != i + 1)
                                {
                                    marginType++;
                                }
                            }
                        }

                        if (marginType != 0)
                        {
                            this.newMap[x, y] = i + 1;
                            TestText(new Vector2(x, y), this.newMap[x, y]);
                        }
                    }
                }
            }


        }

        List<GameObject> testObjects = new List<GameObject>();

        private void DisposeTestObject()
        {
            foreach (var item in testObjects)
            {
                GameObject.Destroy(item);
            }
            testObjects.Clear();
        }

        private void Text(int x, int y, Color color)
        {
            var t = GameObject.CreatePrimitive(PrimitiveType.Cube);
            t.transform.position = new Vector3(x, 0, y);
            t.GetComponent<MeshRenderer>().material.color = color;
            testObjects.Add(t);
        }

        private void TestText(Vector2 pos, int text)
        {
            return;
            GameObject gameObject = new GameObject();
            gameObject.transform.position = new Vector3(pos.x, 2, pos.y);
            gameObject.transform.localEulerAngles = new Vector3(90, 0, 0);
            gameObject.AddComponent<MeshRenderer>();
            var t = gameObject.AddComponent<TextMesh>();
            t.text = text.ToString();
            t.characterSize = 0.3F;
            if (text < 0 && text > -1000)
            {
                t.color = Color.red;
            }
            else if (text <= -1000)
            {
                t.color = Color.blue;
            }
            testObjects.Add(gameObject);
        }

    }
}
