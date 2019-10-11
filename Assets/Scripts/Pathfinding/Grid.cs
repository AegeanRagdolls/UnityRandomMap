using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Grid
{
    /// <summary>
    /// 地图的宽度
    /// </summary>
    public int Width { get => this.nodes.GetLength(0); }

    /// <summary>
    /// 地图的高度
    /// </summary>
    public int Height { get => this.nodes.GetLength(1); }

    private NodeInfo[,] nodes;
    List<NodeInfo> neighbors = new List<NodeInfo>();

    public Grid(bool[,] mapInfo)
    {
        this.nodes = new NodeInfo[mapInfo.GetLength(0), mapInfo.GetLength(1)];
        for (int x = 0; x < mapInfo.GetLength(0); x++)
        {
            for (int y = 0; y < mapInfo.GetLength(1); y++)
            {
                this.nodes[y, x] = new NodeInfo() { x = x, y = y, walkable = !mapInfo[x, y] };
            }
        }
    }

    public NodeInfo GetNodeAt(Vector2 point)
    {
        return this.GetNodeAt((int)point.x, (int)point.y);
    }

    public NodeInfo GetNodeAt(int x, int y)
    {
        return this.nodes[y, x];
    }

    /// <summary>
    /// *确定位置是否在网格内。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsNside(int x, int y)
    {
        return (x >= 0 && x < this.Width) && (y >= 0 && y < this.Height);
    }

    /// <summary>
    /// *确定给定位置的节点是否可行走。
    /// *(如果位置在网格之外，也返回false。)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsWalkableAt(int x, int y)
    {
        return this.IsNside(x, y) && this.nodes[y, x].walkable;
    }

    /// <summary>
    /// 设置是否是障碍物
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="walkable"></param>
    public void SetWalkableAt(int x, int y, bool walkable)
    {
        this.nodes[y, x].walkable = walkable;
    }

    public List<NodeInfo> GetNeighbors(NodeInfo node, DiagonalMovement diagonalMovement)
    {
        bool s0 = false, s1 = false, s2 = false, s3 = false,
           d0 = false, d1 = false, d2 = false, d3 = false;
        int x = node.x, y = node.y;
        neighbors.Clear();



        // ↑
        if (this.IsWalkableAt(x, y - 1))
        {
            neighbors.Add(nodes[y - 1, x]);
            s0 = true;
        }
        // →
        if (this.IsWalkableAt(x + 1, y))
        {
            neighbors.Add(nodes[y, x + 1]);
            s1 = true;
        }
        // ↓
        if (this.IsWalkableAt(x, y + 1))
        {
            neighbors.Add(nodes[y + 1, x]);
            s2 = true;
        }
        // ←
        if (this.IsWalkableAt(x - 1, y))
        {
            neighbors.Add(nodes[y, x - 1]);
            s3 = true;
        }

        if (diagonalMovement == DiagonalMovement.Never)
        {
            return neighbors;
        }

        if (diagonalMovement == DiagonalMovement.OnlyWhenNoObstacles)
        {
            d0 = s3 && s0;
            d1 = s0 && s1;
            d2 = s1 && s2;
            d3 = s2 && s3;
        }
        else if (diagonalMovement == DiagonalMovement.IfAtMostOneObstacle)
        {
            d0 = s3 || s0;
            d1 = s0 || s1;
            d2 = s1 || s2;
            d3 = s2 || s3;
        }
        else if (diagonalMovement == DiagonalMovement.Always)
        {
            d0 = true;
            d1 = true;
            d2 = true;
            d3 = true;
        }
        else
        {
            throw new Exception("Incorrect value of diagonalMovement");
        }

        // ↖
        if (d0 && this.IsWalkableAt(x - 1, y - 1))
        {
            neighbors.Add(nodes[y - 1, x - 1]);
        }
        // ↗
        if (d1 && this.IsWalkableAt(x + 1, y - 1))
        {
            neighbors.Add(nodes[y - 1, x + 1]);
        }
        // ↘
        if (d2 && this.IsWalkableAt(x + 1, y + 1))
        {
            neighbors.Add(nodes[y + 1, x + 1]);
        }
        // ↙
        if (d3 && this.IsWalkableAt(x - 1, y + 1))
        {
            neighbors.Add(nodes[y + 1, x - 1]);
        }

        return neighbors;
    }



}



public class NodeInfo
{
    // 当前点 x;
    public int x;
    // 当前点 Y;
    public int y;

    public float g;

    public float h;

    public float f;

    public NodeInfo parent;

    public bool opened;

    // 是否是障碍
    public bool walkable;

    public bool closed;
    void Dispost()
    {
        this.parent = null;
    }
}
