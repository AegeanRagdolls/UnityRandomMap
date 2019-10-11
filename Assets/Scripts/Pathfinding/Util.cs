using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Util
{
    /// <summary>
    /// 根据父记录回溯并返回路径。
    /// (包括开始节点和结束节点)
    /// </summary>
    public static List<Vector2> Backtrace(NodeInfo node)
    {
        var path = new List<Vector2>();
        path.Add(new Vector2(node.x, node.y));
        while (node.parent != null)
        {
            node = node.parent;
            path.Add(new Vector2(node.x, node.y));
        }
        path.Reverse();
        return path;
    }
}
