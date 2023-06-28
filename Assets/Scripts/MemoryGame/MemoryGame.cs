using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGame : Game
{
    [SerializeField] Grid grid;
    [SerializeField] private MemoryBox box;
    [SerializeField] private MemoryGameCreator memoryGameCreator;
    private static GridSystem<MemoryBox> gridSystem;

    // Start is called before the first frame update
    void Start()
    {
        gridSystem = grid.CreateGrid<MemoryBox>(box);

        memoryGameCreator.CreateGame(gridSystem.GetGridArray());

    }

    public static GridSystem<MemoryBox> GetGridSystem() => gridSystem;

}
