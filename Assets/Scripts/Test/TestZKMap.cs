using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZK.Tool.Map;

public class TestZKMap : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(50, 50);
    // 密集度
    public float chanceToStartAlive = 0.45f;

    // 激活
    public int birthLimit = 4;

    // 杀死
    public int deathLimit = 3;

    // 步数
    public int numberOfSteps = 2;

    public MapAssetInfo mapAssetInfo;


    ZK.Tool.Map.RandomMap randomMap = new ZK.Tool.Map.RandomMap();
    ZK.Tool.Map.OptimizeMap optimizeMap = ZK.Tool.Map.OptimizeMap.Instance;

    List<GameObject> objList = new List<GameObject>();
    bool[,] map;

    // Start is called before the first frame update
    void Start()
    {

    }


    private void OnGUI()
    {
        if (GUILayout.Button("生成种子地图"))
        {
            RandomMap();
            this.ChackMap(this.map);
        }

        if (GUILayout.Button("优化地图"))
        {
            Optimize();
            this.ChackMap(this.map);
        }

        if (GUILayout.Button("预览"))
        {
            RandomMap();
            Optimize();
            CreateMap();
        }
    }

    private void Optimize()
    {
        if (this.map == null) return;
        optimizeMap.LinkLand(this.map);

        foreach (var item in objList)
        {
            GameObject.Destroy(item);
        }

        OptimizeMap.Instance.MapAnalysis(map);

    }

    private void CreateMap()
    {
        CreateMapPiece.Instance.CreateMap(mapAssetInfo, this.map);
    }

    private void RandomMap()
    {
        foreach (var item in objList)
        {
            GameObject.Destroy(item);
        }

        this.randomMap.birthLimit = this.birthLimit;
        this.randomMap.deathLimit = this.deathLimit;
        this.randomMap.numberOfSteps = this.numberOfSteps;
        this.randomMap.chanceToStartAlive = this.chanceToStartAlive;
        this.randomMap.mapWidth = this.mapSize.x;
        this.randomMap.mapHeight = this.mapSize.y;

        this.map = this.randomMap.GenerateMap();
    }

    private void ChackMap(bool[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (!map[x, y])
                {
                    var tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    objList.Add(tmp);
                    tmp.transform.position = new Vector3(x, 0, y);
                }
            }
        }
    }



}
