using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions  
{
    public static (int, int) GetTupleFromVector(this Vector2 vec)
    {
        return ((int)vec.x, (int)vec.y);
    }
}
