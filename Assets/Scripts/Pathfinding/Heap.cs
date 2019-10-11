using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T>
{
    public int Size { get => this.buffer.Count; }

    public bool Empty { get => this.buffer.Count == 0; }

    private List<T> buffer;

    /// <summary>
    /// 比较函数
    /// </summary>
    private Func<T, T, float> compareFunction;

    private T tmpData;
    private T BufferPop
    {
        get
        {
            this.tmpData = this.buffer[this.buffer.Count - 1];
            this.buffer.Remove(this.tmpData);
            return this.tmpData;
        }
    }

    /// <summary>
    /// 二叉堆构造
    /// </summary>
    /// <param name="compare">比较函数</param>
    public Heap(Func<T, T, float> compare)
    {
        this.buffer = new List<T>();
        if (compare != null)
            this.compareFunction = compare;

    }

    public void Clear()
    {
        this.buffer.Clear();
    }


    public T Push(T item)
    {
        return this.Heappush(item);
    }

    public T Pop()
    {
        return this.Heappop();
    }

    public T Peek()
    {
        return this.buffer[0];
    }

    /// <summary>
    /// 更新给定项在堆中的位置。
    /// 每次修改项时都应调用此函数。
    /// </summary>
    public T UpdateItem(T item)
    {
        int pos;
        pos = this.buffer.IndexOf(item);
        if (pos == -1)
            return default(T);

        this.Siftdown(0, pos);
        return this.Siftup(pos);
    }


    //public void Contains()
    //{


    //}

    //public void Replace()
    //{


    //}


    //public void Pushpop()
    //{

    //}





    /*
        在列表a中插入项目x，假设a已排序，则保持它的排序。
        如果x已经在a中，把它插入最右边的x。
        可选的args lo(默认0)和hi(默认a.length)绑定了这个片
        一个待搜索的。
     */
    float[] Insort(float[] a, float x, float lo = 0, float hi = 0)
    {
        int mid = 0;

        if (lo < 0) throw new Exception("lo must be non-negative");

        hi = hi == 0 ? a.Length : hi;

        while (lo < hi)
        {
            mid = (int)Mathf.Floor((lo + hi) / 2);
            //  hi = this.compareFunction(x, mid) < 0 ? mid : mid + 1;
        }
        return (new float[0]);
    }

    T Heappush(T item)
    {
        this.buffer.Add(item);
        return this.Siftdown(0, this.buffer.Count - 1);
    }

    T Heappop()
    {
        T tmpBufferData, returnItem;
        tmpBufferData = this.BufferPop;
        if (this.buffer.Count > 0)
        {
            returnItem = this.buffer[0];
            this.buffer[0] = tmpBufferData;
            this.Siftup(0);
            return returnItem;
        }
        else
        {
            return tmpBufferData;
        }
    }
    /*
        弹出并返回当前最小值，并添加新项。
        这比heappop()后面跟着heappush()更有效，而且可以这样做
        更适合使用固定大小的堆。注意
        返回的可能比项目更大!这限制了合理使用
        这个例程除非作为条件替换的一部分编写:
        如果项目>数组[0]
        项目= heapreplace(数组，项目)
    */
    T heapreplace(T item)
    {
        T returnItem = this.buffer[0];
        this.buffer[0] = item;
        this.Siftup(0);
        return returnItem;
    }

    /// <summary>
    /// push And pop
    /// </summary>
    T Heappushpop(T item)
    {
        if (this.buffer.Count > 0 && this.compareFunction(this.buffer[0], item) < 0)
        {
            var _ref = (this.buffer[0], item);
            item = _ref.item;
            this.buffer[0] = _ref.Item1;
            this.Siftup(0);
        }
        return item;
    }



    ///// <summary>
    ///// 查找数据集中最大的n个元素。
    ///// </summary>
    //void Nlargest()
    //{

    //}

    ///// <summary>
    ///// 查找数据集中最小的n个元素。
    ///// </summary>
    //void Nsmallest()
    //{

    //}

    T Siftdown(int startPos, int pos)
    {
        T newItem, parent;
        int parentPos;
        newItem = this.buffer[pos];

        while (pos > startPos)
        {
            parentPos = (pos - 1) >> 1;
            parent = this.buffer[parentPos];
            if (this.compareFunction(newItem, parent) < 0)
            {
                this.buffer[pos] = parent;
                pos = parentPos;
                continue;
            }
            break;
        }

        return this.buffer[pos] = newItem;
    }

    T Siftup(int pos)
    {
        int childPos, endPos, rightPos, startPos;
        T newItem;

        endPos = this.buffer.Count;
        startPos = pos;
        newItem = this.buffer[pos];
        childPos = 2 * pos + 1;
        while (childPos < endPos)
        {
            rightPos = childPos + 1;
            if (rightPos < endPos && !(this.compareFunction(this.buffer[childPos], this.buffer[rightPos]) < 0))
            {
                childPos = rightPos;
            }
            this.buffer[pos] = this.buffer[childPos];
            pos = childPos;
            childPos = 2 * pos + 1;
        }
        this.buffer[pos] = newItem;
        return this.Siftdown(startPos, pos);
    }




}
