using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameType<T> where T : Box
{
    public T GetGridSystem { get; set; }
   
}
