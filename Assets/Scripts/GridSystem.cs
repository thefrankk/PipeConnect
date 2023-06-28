using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GridSystem<T> where T : UnityEngine.Object
{

    private int height;
    private int width;
    private float spacing;
    private int cellSize;

    private T[,] gridArray;

    public GridSystem(float spacing, int h, int w, T obj, Transform parent, Action<Vector2, T> posCallback)
    {
        this.spacing = spacing;
        height = h;
        width= w;
        gridArray = new T[h, w];

        CreateGrid(h, w, obj, parent, posCallback);
    }

    public void CreateGrid(int height, int width, T obj, Transform parent, Action<Vector2, T> posCallback) 
    {
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                T _obj = GameObject.Instantiate(obj, new Vector3(x * spacing, y * spacing, 0), Quaternion.identity, parent);
                gridArray[x, y] = _obj;
                posCallback?.Invoke(new Vector2(x, y), _obj);
                //_obj.GetComponent<T>().SetPosition(new Vector2(x, y));
                
            }
        }
    }


    public T GetGridObject(int x, int y) => gridArray[x, y]; 

    public T[,] GetGridArray() => gridArray;


    public List<BoxNeigbours<T>> GetNeighborsUnlimited((int, int) pos)
    {

        int x;
        int y;
        x = pos.Item1;
        y = pos.Item2;

        List<BoxNeigbours<T>> neighbors = new List<BoxNeigbours<T>>();

        // Check the surrounding cells
        if (x > 0)
        {
            neighbors.Add(new BoxNeigbours<T>((x - 1, y), GetGridObject(x - 1, y), BoxNeigbours<T>.Orientation.Left));
        }
        if (y > 0) //1
        {
            int dir = y - 1;
            neighbors.Add(new BoxNeigbours<T>((x, dir), GetGridObject(x, dir), BoxNeigbours<T>.Orientation.Bottom));
        }
        if (x < width - 1)
        {
            int dir = x + 1;
            neighbors.Add(new BoxNeigbours<T>((dir, y), GetGridObject(dir, y), BoxNeigbours<T>.Orientation.Right));

        }

        if (y < height - 1)  // 2
        {
            int dir = y + 1;
            neighbors.Add(new BoxNeigbours<T>((x, dir), GetGridObject(x, dir), BoxNeigbours<T>.Orientation.Top));
        }

        return neighbors;
    }

    public List<BoxNeigbours<T>> GetNeighbors((int, int) pos)
    {
        int x;
        int y;
        x = pos.Item1;
        y = pos.Item2;

        List<BoxNeigbours<T>> neighbors = new List<BoxNeigbours<T>>();

        // Check the surrounding cells
        if (x > 0)
        {
                // Check left neighbor
                 neighbors.Add(new BoxNeigbours<T>((x - 1, y), GetGridObject(x - 1, y), BoxNeigbours<T>.Orientation.Left));
           // Debug.Log(x - 1 + "   " + y + " " + GetGridObject(x - 1, y));
        }
        if (y > 1) //1
        {
                int dir = y - 1;

            // Check top neighbor
             neighbors.Add(new BoxNeigbours<T>((x, dir), GetGridObject(x, dir), BoxNeigbours<T>.Orientation.Bottom));
            //Debug.Log(x  + "   " + dir + " " + GetGridObject(x, dir));


        }
        if (x < width - 1)
        {
                int dir = x + 1;

            // Check right neighbor
             neighbors.Add(new BoxNeigbours<T>((dir, y), GetGridObject(dir, y), BoxNeigbours<T>.Orientation.Right));
            //Debug.Log(dir + "   " + y + " " + GetGridObject(dir, y));

        }

        if (y < height - 1)
        {
            int dir = y + 1;

            PipeBox box = GetGridObject(x, dir) as PipeBox;

            if(box.GetPipe() is not null)
            {
                if(box.GetPipe().GetCurrentType() == Pipe.Type.End)
                {
                    neighbors.Add(new BoxNeigbours<T>((x, dir), GetGridObject(x, dir), BoxNeigbours<T>.Orientation.Top));
                }

            }

            // Check bottom neighbor
            // Debug.Log(x + "   " + dir + " " + GetGridObject(x, dir));
        }

        if (y < height - 2)  // 2
        {

            
            int dir = y + 1;

            // Check bottom neighbor
            neighbors.Add(new BoxNeigbours<T>((x, dir), GetGridObject(x, dir), BoxNeigbours<T>.Orientation.Top));
            // Debug.Log(x + "   " + dir + " " + GetGridObject(x, dir));
        }

        return neighbors;
    }


    public int GetHeight() => height;
    public int GetWidth() => width;
}


public class BoxNeigbours<T> where T : UnityEngine.Object
{
    (int, int) position;
    T box;

    public enum Orientation
    {
        Bottom,
        Right,
        Top,
        Left,
    }

    private Orientation currentOrientationRespectPrevNeigbhor;

    public BoxNeigbours((int, int) pos, T box, Orientation currentOrientationRespectPrevNeigbhor)
    {
        this.position = pos;
        this.box = box;
        this.currentOrientationRespectPrevNeigbhor = currentOrientationRespectPrevNeigbhor;
    }

    public T GetBox() => box;
    public (int, int) GetPosition() => position;

    public Orientation GetOrientation() => currentOrientationRespectPrevNeigbhor;


}