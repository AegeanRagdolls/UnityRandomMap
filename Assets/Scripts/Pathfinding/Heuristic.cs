using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 启发函数
/// </summary>
public class Heuristic
{

    /// <summary>
    /// 曼哈顿 距离
    /// </summary>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    public static float Manhattan(float dx, float dy)
    {
        return dx + dy;
    }

    /// <summary>
    /// 欧几里得 距离
    /// </summary>
    public static float Euclidean(float dx, float dy)
    {
        return Mathf.Sqrt(dx * dx + dy * dy);
    }


    /// <summary>
    /// octile 距离
    /// </summary>
    public static float Octile(float dx, float dy)
    {
        var f = Mathf.Sqrt(2);
        return (dx < dy) ? f * dx + dy : f * dy + dx;
    }


    public static float Chebyshev(float dx, float dy)
    {
        return Mathf.Max(dx, dy);
    }

}
