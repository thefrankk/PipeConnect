using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class MemoryBox : Box
{

    Queue<int> orderList = new Queue<int>();

    private bool isMarked;

    public Action<MemoryBox> OnMarkedBox;


   
    private async void OnMouseDown()
    {
        if (MemoryGameCreator.IsShowingBoxes) return; 

        ChangeColor(Color.green);

        OnMarkedBox?.Invoke(this);

        await Task.Delay(200);

        ChangeColor(Color.white);


    }

    public void ResetBox()
    {
        orderList.Clear();

        orderList.Enqueue(-1);
    }

    public void SetCurrentOrder(int currentOrder)
    {
        if (orderList.Contains(-1))
        {
            orderList.Dequeue();
        }
        orderList.Enqueue(currentOrder);
    }
    public int GetCurrentOrder() => orderList.Dequeue();
   

    public bool GetIfMarked()=> isMarked;
    public void SetIfMarked(bool isMarked) => this.isMarked = isMarked;
}
