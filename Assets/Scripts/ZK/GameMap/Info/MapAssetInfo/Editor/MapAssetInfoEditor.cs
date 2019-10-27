using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System.Text;

namespace ZK.Tool.Map
{
    public enum SaveType
    {
        JsonFile,
        Binary
    }


    [CustomEditor(typeof(MapAssetInfo))]
    public class MapAssetInfoEditor : Editor
    {
        private MapAssetInfo mai;


        private ushort id = 0;
        private Dictionary<string, ushort> dicIdBuffer = new Dictionary<string, ushort>();

        private List<PrefabAssetData> prefabAssetList = new List<PrefabAssetData>();

        private List<PrefabData> prefabDataList = new List<PrefabData>();

        private Dictionary<string, PrefabData> tmpPrefabMap = new Dictionary<string, PrefabData>();

        public void OnEnable()
        {
            this.mai = (MapAssetInfo)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("SaveDependJson"))
            {
                this.AnalysisRely();
                this.SavePrefabDependInfo(SaveType.JsonFile);
            }
            if (GUILayout.Button("SaveDependBinary"))
            {
                this.AnalysisRely();
                this.SavePrefabDependInfo(SaveType.Binary);
            }
        }


        private PrefabData SetAssetData(GameObject asset, List<string> parentName = null)
        {
            if (asset == null) return null;

            return GetPrefabDepe(asset);
        }

        private void SetPrefabData(PrefabData pd, Transform transform)
        {
            pd.postionX = transform.localPosition.x;
            pd.postionY = transform.localPosition.y;
            pd.postionZ = transform.localPosition.z;

            pd.scaleX = transform.localScale.x;
            pd.scaleY = transform.localScale.y;
            pd.scaleZ = transform.localScale.z;

            pd.eulerX = transform.localEulerAngles.x;
            pd.eulerY = transform.localEulerAngles.y;
            pd.eulerZ = transform.localEulerAngles.z;
        }



        private ushort GetAssetID(Object item, PrefabData prefabData = null)
        {
            var path = AssetDatabase.GetAssetPath(item);
            var name = GetAssetName(path);
            string appointPath = null;
            if (item.GetType() == typeof(Texture2D))
            {
                appointPath = path.Substring(path.IndexOf("Textures"));
            }
            else if (item.GetType() == typeof(Mesh))
            {
                var index = path.IndexOf("Models");
                appointPath = path.Substring(index >= 0 ? index : 0);
            }
            else
            {
                appointPath = null;
                name = item.name + "_" + name;
            }

            if (!this.dicIdBuffer.TryGetValue(name, out ushort id))
            {
                this.id++;
                this.dicIdBuffer.Add(name, this.id);
                id = this.id;
                if (!string.IsNullOrEmpty(appointPath))
                {
                    var tmpIndex = appointPath.LastIndexOf('.');
                    appointPath = appointPath.Substring(0, tmpIndex >= 0 ? tmpIndex : 0);
                    this.prefabAssetList.Add(new PrefabAssetData() { assetID = id, name = name, appointPath = appointPath });
                }
                else
                {
                    prefabData.assetID = id;
                    this.prefabDataList.Add(prefabData);
                }
            }
            return id;
        }

        private string GetAssetName(string path)
        {
            var nameIndex = path.LastIndexOf('/');
            var name = path.Substring(nameIndex + 1);
            var tmpIndex = name.LastIndexOf('.');
            name = name.Substring(0, tmpIndex >= 0 ? tmpIndex : 0);
            return name;
        }
        #region Test
        private Dictionary<GameObject, PrefabData> tmpMap = new Dictionary<GameObject, PrefabData>();

        private Dictionary<string, PrefabData> dicTmpTestMap = new Dictionary<string, PrefabData>();
        private PrefabData test(GameObject go)
        {
            PrefabData mainData = new PrefabData(); // 创建主数据
            SetPrefabData(mainData, go.transform); // 初始化主主数据状态
            var key = GetProfabPath(go); // 为主数据添加Key
            if (!this.dicTmpTestMap.TryGetValue(key, out PrefabData data)) // 查看这个预制体是否被创建
            {
                this.SetMeshAndTextureToData(go, mainData);
                this.GetAssetID(go, mainData);
                this.dicTmpTestMap.Add(key, mainData);
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var obj = go.transform.GetChild(i).gameObject;
                    key = GetProfabPath(obj);
                    if (this.dicTmpTestMap.ContainsKey(key))
                    {
                        // 不是预制体
                        PrefabData tmpPrefabData = new PrefabData();
                        this.SetMeshAndTextureToData(obj, tmpPrefabData);
                        mainData.chileList.Add(tmpPrefabData);
                    }
                    else
                    {
                        // 是预制体
                        var prefabData = test(obj);
                        mainData.chileList.Add(prefabData);
                    }
                }
            }
            else
            {
                mainData.assetID = data.assetID;
            }
            return mainData;
        }
        #endregion

        /// <summary>
        /// 获取当前预制体的路径
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private string GetProfabPath(GameObject prefab)
        {
            var tmp = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefab);
            var path = AssetDatabase.GetAssetPath(tmp);
            return path;
        }


        /// <summary>
        /// 获取预制件依赖 
        /// </summary>
        /// <typeparam name="T">欲获取的类型</typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        private PrefabData GetPrefabDepe(GameObject go)
        {
            PrefabData mainPrefab = new PrefabData();
            SetPrefabData(mainPrefab, go.transform);

            if (!tmpMap.ContainsKey(PrefabUtility.GetCorrespondingObjectFromOriginalSource(go)))
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject dependObj = go.transform.GetChild(i).gameObject;
                    if (dependObj != null)
                    {
                        var tmp = PrefabUtility.GetCorrespondingObjectFromOriginalSource(dependObj);
                        PrefabData tmpPrefab = new PrefabData();
                        var path = AssetDatabase.GetAssetPath(tmp);
                        path += "." + dependObj.transform.childCount;
                        PrefabData prefabData = null;
                        if (!string.IsNullOrEmpty(path) && !this.tmpPrefabMap.TryGetValue(path, out prefabData))
                        {
                            this.tmpPrefabMap.Add(path, null);
                            prefabData = this.GetPrefabDepe(dependObj);
                            this.tmpPrefabMap[path] = prefabData;
                            tmpPrefab.assetID = prefabData.assetID;
                            this.SetPrefabData(tmpPrefab, dependObj.transform);
                            tmpMap.Add(tmp, prefabData);
                            Debug.Log("Prefab");
                        }
                        else
                        {
                            this.SetMeshAndTextureToData(dependObj, tmpPrefab);
                        }
                        mainPrefab.chileList.Add(tmpPrefab);
                    }
                }
                this.SetMeshAndTextureToData(go, mainPrefab);
                this.GetAssetID(go, mainPrefab);
            }
            return mainPrefab;
        }

        /// <summary>
        /// 设置网格和贴图到预支体信息
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tmpPrefab"></param>
        private void SetMeshAndTextureToData(GameObject go, PrefabData tmpPrefab)
        {
            SetPrefabData(tmpPrefab, go.transform);
            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var meshId = this.GetAssetID(meshFilter.sharedMesh);
                tmpPrefab.meshList.Add(meshId);
            }
            var meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Object[] r = new Object[] { meshRenderer.sharedMaterial };
                Object[] d = EditorUtility.CollectDependencies(r);
                foreach (var item in d)
                {
                    if (item != null && item.GetType() == typeof(Texture2D))
                    {
                        var t = (Texture2D)System.Convert.ChangeType(item, typeof(Texture2D));
                        var id = this.GetAssetID(t);
                        tmpPrefab.textureList.Add(id);
                    }
                }
            }

        }

        private void AnalysisRely()
        {
            this.dicIdBuffer.Clear();
            this.tmpPrefabMap.Clear();
            this.prefabAssetList.Clear();
            this.prefabDataList.Clear();
            this.tmpMap.Clear();
            this.dicTmpTestMap.Clear();
            this.id = 0;
            if (this.mai.mapBarrierConfigs != null)
            {
                foreach (var item in this.mai.mapBarrierConfigs)
                {
                    if (item.mapAsset == null) continue;
                    item.assetId = this.SetAssetData(item.mapAsset).assetID;
                }
            }
            if (this.mai.mapAssetConfigs != null)
            {
                foreach (var item in this.mai.mapAssetConfigs)
                {
                    if (item.mapAsset == null) continue;
                    item.assetId = this.SetAssetData(item.mapAsset).assetID;
                }
            }
            if (this.mai.mapFloorConfigs != null)
            {
                foreach (var item in this.mai.mapFloorConfigs)
                {
                    if (item.mapAsset == null) continue;
                    item.assetId = this.SetAssetData(item.mapAsset).assetID;
                }
            }

        }

        public void SavePrefabDependInfo(SaveType saveType)
        {
            MapPrefabData mapJsonData = new MapPrefabData();
            mapJsonData.prefabAssetList = this.prefabAssetList;
            mapJsonData.prefabDataList = this.prefabDataList;
            var json = LitJson.JsonMapper.ToJson(mapJsonData);
            string path = "";
            switch (saveType)
            {
                case SaveType.JsonFile:

                    path = EditorUtility.SaveFilePanel("请选择文件夹", Application.dataPath, "MapAssetData", "json");
                    if (path.Length > 0)
                    {
                        File.WriteAllText(path, json);
                    }
                    break;
                case SaveType.Binary:
                    path = EditorUtility.SaveFilePanel("请选择文件夹", Application.dataPath, "MapAssetData", "bin");
                    var bytes = Encoding.UTF8.GetBytes(json);
                    var data = RandomMapHelper.CompressGZip(bytes);
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(data, 0, data.Length);
                    }
                    break;
                default:
                    break;
            }


        }

    }

    public class MapPrefabData
    {
        public List<PrefabAssetData> prefabAssetList = new List<PrefabAssetData>();

        public List<PrefabData> prefabDataList = new List<PrefabData>();
    }
}