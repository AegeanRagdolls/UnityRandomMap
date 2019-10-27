using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZK.Tool.Map;

public class Test : MonoBehaviour
{
    public GameObject obj;

    public void Awake()
    {
        var objSize = RandomMapHelper.GetObjectMehsSizeToInt(obj);
        Debug.Log(objSize);
        obj.transform.position = Vector3.zero;
        var startXPoint = objSize.x / 2F;
        var startYPoint = objSize.y / 2F;
        var startZPoint = objSize.z / 2F;
        for (int x = 0; x < objSize.x; x++)
        {
            for (int y = 0; y < objSize.y; y++)
            {
                for (int z = 0; z < objSize.z; z++)
                {
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.transform.position = new Vector3(obj.transform.position.x - startXPoint + x, obj.transform.position.y + y, obj.transform.position.z - startZPoint + z);
                }
            }
        }



    }



}
