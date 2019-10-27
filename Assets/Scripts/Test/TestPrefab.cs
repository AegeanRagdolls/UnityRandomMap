using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Security.Cryptography;
using ZK.Tool.Map;

public class TestPrefab : MonoBehaviour
{
    public List<GameObject> objs;

    private Dictionary<string, string> dicAssetMap = new Dictionary<string, string>();

    private List<GameAssetInfo> assetList = new List<GameAssetInfo>();

    private Dictionary<string, PrefabAseetInfo> dicPrefabAssetInfoMap = new Dictionary<string, PrefabAseetInfo>();

    private List<PrefabObject> prefabObjects = new List<PrefabObject>();

    private void OnGUI()
    {
        if (GUILayout.Button("Test"))
        {
            foreach (var obj in this.objs)
            {
                this.prefabObjects.Add(this.EximineObjectReference(obj));

            }
            TestInfo testInfo = new TestInfo();
            testInfo.assetList = this.assetList;
            testInfo.prefabAssetList = this.dicPrefabAssetInfoMap.Values.ToList();
            testInfo.prefabObjectList = this.prefabObjects;
            var data = LitJson.JsonMapper.ToJson(testInfo);
            //System.IO.File.WriteAllText(@"C:\Users\oream\Desktop\新建文本文档.json", data);
            using (System.IO.FileStream s = new System.IO.FileStream(@"C:\Users\oream\Desktop\testData.bin", System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var tmpBuffer = RandomMapHelper.CompressGZip(Encoding.UTF8.GetBytes(data));
                s.Write(tmpBuffer, 0, tmpBuffer.Length);
            }


            Debug.Log(data);
        }
    }

    public PrefabObject EximineObjectReference(GameObject obj)
    {
        PrefabObject mainPrefabObject = new PrefabObject();
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            var tmp = obj.transform.GetChild(i);
            var info = this.SetMeshAndTextureToData(tmp.gameObject);
            var prefabObject = this.EximineObjectReference(tmp.gameObject);
            if (info != null)
            {
                prefabObject.prefabID = info.id;
                Debug.Log("123123");
            }
            mainPrefabObject.child.Add(prefabObject);
            Debug.Log(mainPrefabObject.child.Count);
        }
        mainPrefabObject.posititon = new ObjectVector3(obj.transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
        mainPrefabObject.rotate = new ObjectVector3(obj.transform.localEulerAngles.x, obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.z);
        mainPrefabObject.scale = new ObjectVector3(obj.transform.localEulerAngles.x, obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.z);
        return mainPrefabObject;
    }

    public string EximineObjectPath(GameObject obj)
    {
        var tmpObj = PrefabUtility.GetCorrespondingObjectFromSource(obj);
        var path = AssetDatabase.GetAssetPath(obj);
        return path;
    }

    private PrefabAseetInfo SetMeshAndTextureToData(GameObject go)
    {
        var assetInfo = new PrefabAseetInfo();
        var meshFilter = go.GetComponent<MeshFilter>();
        StringBuilder stringBuilder = new StringBuilder();

        if (meshFilter != null)
        {
            var path = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
            if (!this.dicAssetMap.TryGetValue(path, out string meshID))
            {
                meshID = this.MD5Encrypt(path);
                this.dicAssetMap.Add(path, meshID);
                var gameAssetInfo = new GameAssetInfo();
                gameAssetInfo.appointPath = path;
                gameAssetInfo.assetID = MD5Encrypt(path);
                this.assetList.Add(gameAssetInfo);
            }
            stringBuilder.Append(path);
            assetInfo.meshID = meshID;
        }
        var meshRenderer = go.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            var mainT = meshRenderer.sharedMaterial.mainTexture;
            if (mainT != null)
            {
                var mainTexturePath = AssetDatabase.GetAssetPath(mainT);
                if (!this.dicAssetMap.TryGetValue(mainTexturePath, out string mainTID))
                {
                    mainTID = this.MD5Encrypt(mainTexturePath);
                    this.dicAssetMap.Add(mainTexturePath, mainTID);
                    var gameAssetInfo = new GameAssetInfo();
                    gameAssetInfo.appointPath = mainTexturePath;
                    gameAssetInfo.assetID = MD5Encrypt(mainTexturePath);
                    this.assetList.Add(gameAssetInfo);
                }
                assetInfo.mainTextureID = mainTID;
                stringBuilder.Append(mainTexturePath);
            }
        }
        var key = stringBuilder.ToString();
        if (string.IsNullOrEmpty(key)) return null;
        assetInfo.id = MD5Encrypt(key);
        if (!this.dicPrefabAssetInfoMap.TryGetValue(assetInfo.id, out PrefabAseetInfo prefabAseet))
        {
            this.dicPrefabAssetInfoMap.Add(assetInfo.id, assetInfo);
        }
        return assetInfo;
    }
    private string MD5Encrypt(string password)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes;
        hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));
        }
        return tmp.ToString();
    }

}

public class TestInfo
{
    public List<GameAssetInfo> assetList = new List<GameAssetInfo>();

    public List<PrefabAseetInfo> prefabAssetList = new List<PrefabAseetInfo>();

    public List<PrefabObject> prefabObjectList = new List<PrefabObject>();
}


/// <summary>
/// 预制体信息
/// </summary>
public class PrefabAseetInfo
{
    public string id;

    public string meshID;

    public string mainTextureID;

}

/// <summary>
/// 游戏资源信息
/// </summary>
public class GameAssetInfo
{
    /// <summary>
    /// 资源ID
    /// </summary>
    public string assetID;

    /// <summary>
    /// 资源路径
    /// </summary>
    public string appointPath;

}

public class PrefabObject
{
    public string prefabID;
    public List<PrefabObject> child = new List<PrefabObject>();
    public ObjectVector3 posititon;
    public ObjectVector3 rotate;
    public ObjectVector3 scale;
}


public class ObjectVector3
{
    public ObjectVector3(float x, float y, float z)
    {
        this.x = (double)x;
        this.y = (double)y;
        this.z = (double)z;
    }
    public double x;
    public double y;
    public double z;
}

