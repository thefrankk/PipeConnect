using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class MemoryGameCreator : MonoBehaviour
{

    private LinkedList<MemoryBox> memoryBoxList;
    private LinkedListNode<MemoryBox> memoryBoxNode;

    [SerializeField] private SpriteRenderer[] lights;


    public static bool IsShowingBoxes;
    private int currentOrder;
    [SerializeField] private int gameSize = 4;

    private MemoryBox[,] grid;
    public void CreateGame(MemoryBox[,] grid)
    {
        this.grid = grid;
        memoryBoxList = new LinkedList<MemoryBox>();

        connectAllBoxes(grid);

        setBoxesInOrder(gameSize, grid);

        memoryBoxNode = memoryBoxList.First;

        showBoxes(memoryBoxNode);

        currentOrder = 0;
    }

    private void connectAllBoxes(MemoryBox[,] grid)
    {
        foreach (var item in grid)
        {
            item.OnMarkedBox += checkGame;
            item.ResetBox();
        }
    }

    private void setBoxesInOrder(int size, MemoryBox[,] grid)
    {
        for (int i = 0; i < size; i++)
        {

            LinkedListNode<MemoryBox> node;
            (int, int) randNumber = getRandomPosition(grid.GetLength(0), grid.GetLength(1));
            node = memoryBoxList.AddLast(grid[randNumber.Item1, randNumber.Item2]);
            
            node.Value.SetCurrentOrder(i);
           
        }
    }

    private async void showBoxes(LinkedListNode<MemoryBox> node)
    {
        IsShowingBoxes = true;

        node.Value.ChangeColor(Color.green);

        await Task.Delay(500);

        node.Value.ChangeColor(Color.white);

        await Task.Delay(500);

        if (node.Next == null) { IsShowingBoxes = false; return; } 

        showBoxes(node.Next);
    }

    private void repeatSequence()
    {
        currentOrder = 0;
        LinkedListNode<MemoryBox> node = memoryBoxList.First;

        foreach (var box in grid)
        {
            box.ResetBox();
        }

        for (int i = 0; node != null; i++)
        {
            node.Value.SetCurrentOrder(i);
            node = node.Next;
        }
        node = memoryBoxList.First;

        showBoxes(node);
    }
    private async void checkGame(MemoryBox box)
    {

        int order = box.GetCurrentOrder();
        Debug.Log(currentOrder);
        Debug.Log(order);

        if (currentOrder == order)
        {
            lights[currentOrder].color = Color.green;
            box.ChangeColor(Color.green);
            currentOrder++;
            Debug.LogWarning(currentOrder);

            if (currentOrder >= gameSize) Debug.LogWarning("EMDED");
        }
        else
        {
            foreach (var light in lights)
            {
                light.color = Color.red;
            }

            box.ChangeColor(Color.red);

            await Task.Delay(500);
            repeatSequence();

        }
    }
    private (int, int) getRandomPosition(int rows, int cols)
    {
        return (UnityEngine.Random.Range(0, rows), UnityEngine.Random.Range(0, cols));
    }
}
