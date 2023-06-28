using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{

    // public static GridSystem<T> gridSystem;

    [SerializeField] private float spacing = 0.58f;
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private Transform parent;
  

    public GridSystem<T> CreateGrid<T>(T boxRef) where T : UnityEngine.Object
    {
        GridSystem<T> gridSystem = new GridSystem<T>(spacing, height, width, boxRef, parent, (pos, obj) =>
        {
            Box boxRef = obj as Box;
            boxRef.SetPosition(pos);

        });

        return gridSystem;
    }

    public int GetHeight() => height;
    public int GetWidth() => width;

 


}
