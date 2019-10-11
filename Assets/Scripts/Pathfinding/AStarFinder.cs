using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AStarFinder
{
    private Grid grid = null;

    private const float SQRT2 = 1.4142135623730951F;

    public AStarFinder(bool[,] map, Opt opt)
    {
        grid = new Grid(map);
        this.Start(opt);
    }

    Opt opt = null;

    private void Start(Opt opt)
    {
        #region 是否允许对角移动

        //   ---------------  如果允许对角移动   ---------------------- 

        if (!opt.allowDiagonal)
        {
            opt.diagonalMovement = DiagonalMovement.Never;
        }
        else
        {
            if (opt.dontCrossCorners)
            {
                opt.diagonalMovement = DiagonalMovement.OnlyWhenNoObstacles;
            }
            else
            {
                opt.diagonalMovement = DiagonalMovement.IfAtMostOneObstacle;
            }
        }

        if (opt.diagonalMovement == DiagonalMovement.Never)
        {
            opt.heuristic = Heuristic.Manhattan;
        }
        else
        {
            opt.heuristic = Heuristic.Octile;
        }


        // ------------------------------------------------------------ 
        #endregion

        this.opt = opt;

    }


    private int Round(float num)
    {
        var tmp = Mathf.RoundToInt(num);
        return tmp < 0 ? 0 : tmp;
    }

    //openList;

    public List<Vector2> FindPath(Vector2 startPoint, Vector2 endPoint)
    {
        startPoint = new Vector2(this.Round(startPoint.x), this.Round(startPoint.y));
        endPoint = new Vector2(this.Round(endPoint.x), this.Round(endPoint.y));
        Debug.LogWarning($"开始点 : {startPoint} ; 结束点 : {endPoint}");
        var openList = new Heap<NodeInfo>((nodeA, nodeB) =>
        {
            return nodeA.f - nodeB.f;
        });

        var startNode = grid.GetNodeAt(startPoint);
        var endNode = grid.GetNodeAt(endPoint);
        NodeInfo node, neighbor;
        List<NodeInfo> neightbors;
        int i, l, x, y;
        float ng;
        startNode.g = 0;
        startNode.f = 0;

        openList.Push(startNode);
        startNode.opened = true;

        while (!openList.Empty)
        {
            node = openList.Pop();
            node.closed = true;

            if (node == endNode)
            {
                return Util.Backtrace(endNode);
            }

            neightbors = grid.GetNeighbors(node, this.opt.diagonalMovement);

            for (i = 0, l = neightbors.Count; i < l; i++)
            {
                neighbor = neightbors[i];

                if (neighbor.closed) continue;

                x = neighbor.x;
                y = neighbor.y;

                ng = node.g + ((x - node.x == 0 || y - node.y == 0) ? 1 : SQRT2);


                if (!neighbor.opened || ng < neighbor.g)
                {
                    neighbor.g = ng;
                    neighbor.h = opt.weight * opt.heuristic(Mathf.Abs(x - endPoint.x), Mathf.Abs(y - endPoint.y));
                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.parent = node;

                    if (!neighbor.opened)
                    {
                        openList.Push(neighbor);
                        neighbor.opened = true;
                    }
                    else
                    {
                        openList.UpdateItem(neighbor);
                    }

                }


            }

        }

        return null;
    }

}


public class Opt
{
    /// <summary>
    /// 是否允许对角线移动 (不推荐使用对角线移动)
    /// </summary>
    public bool allowDiagonal = true;

    /// <summary>
    /// 不允许穿过墙角
    /// </summary>
    public bool dontCrossCorners = true;

    /// <summary>
    /// 移动类型
    /// </summary>
    public DiagonalMovement diagonalMovement;

    /// <summary>
    /// 启发函数估算距离
    /// </summary>
    public Func<float, float, float> heuristic;

    /// <summary>
    /// 权重
    /// </summary>
    public float weight;
}


public enum DiagonalMovement
{
    /// <summary>
    /// 总是
    /// </summary>
    Always = 1,
    /// <summary>
    /// 从不
    /// </summary>
    Never = 2,
    /// <summary>
    /// 如果最多一个障碍
    /// </summary>
    IfAtMostOneObstacle = 3,
    /// <summary>
    /// 只有在没有障碍的时候
    /// </summary>
    OnlyWhenNoObstacles = 4

}