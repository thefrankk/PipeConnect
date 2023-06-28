using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    PipeEnd endPipe;
    [SerializeField] Grid grid;
    [SerializeField] PathCreator pathCreator;
    [SerializeField] private PipeBox box;

    private static GridSystem<PipeBox> gridSystem;

    public static Action GameStarted;
    private void OnEnable()
    {
        PipesInGame.OnAllPipesPlaced += configEndPipe;
    }

    private void OnDisable()
    {
        PipesInGame.OnAllPipesPlaced -= configEndPipe;
    }

    private void Start()
    {
        gridSystem = grid.CreateGrid<PipeBox>(box);

        pathCreator.StartPath();
    }
    private void configEndPipe(List<Pipe> pipes)
    {
        endPipe = pipes.Where(x => x.GetCurrentType() == Pipe.Type.End)
                       .FirstOrDefault() as PipeEnd;

        endPipe.OnEndReached += win;

        GameStarted?.Invoke();
    }


    private void win()
    {
        Debug.Log("Win!");
    }

    public static GridSystem<PipeBox> GetGridSystem() => gridSystem;



}
