using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class PipesInGame : MonoBehaviour
{
    private PipesInPathCreator pipesInPathCreatorSystem;
    private List<Pipe> allPipes;
    private List<PipeBox> allBoxes = new List<PipeBox>();


    public static Action<List<Pipe>> OnAllPipesPlaced;
    GridSystem<PipeBox> gridSystem;


    private void Awake()
    {
        pipesInPathCreatorSystem = GetComponent<PipesInPathCreator>();
    }

    private void OnEnable()
    {
        pipesInPathCreatorSystem.OnPipesCreated += configPipesInGame;
    }

    private void OnDisable()
    {
        pipesInPathCreatorSystem.OnPipesCreated -= configPipesInGame;

    }
  
    private void configPipesInGame(List<Pipe> pipeList)
    {
        gridSystem = Game.GetGridSystem();
        allPipes = pipeList;
        createAllBoxesList();
        addNeigboursForPipes();

        OnAllPipesPlaced?.Invoke(allPipes);

    }

    private void createAllBoxesList()
    {
        foreach (var item in gridSystem.GetGridArray())
        {
            allBoxes.Add(item);
        }
    }
    private void addNeigboursForPipes()
    {
        for (int i = 0; i < allBoxes.Count; i++)
        {
            PipeBox currentBox = allBoxes[i];
            List<BoxNeigbours<PipeBox>> boxNeigbours = gridSystem.GetNeighborsUnlimited(Extensions.GetTupleFromVector(currentBox.GetCurrentPosition()));

            List <Pipe> pipesNeigbours = boxNeigbours.Where(x => true)
                                                    .Select(x => x.GetBox())
                                                    .Select(x => x.GetPipe())
                                                    .ToList();



            List<BoxNeigbours<PipeBox>.Orientation> orientationsList = boxNeigbours.Where(x => true)
                                                                  .Select(x => x.GetOrientation())
                                                                  .ToList();

            Dictionary<Pipe, BoxNeigbours<PipeBox>.Orientation> neigboursPipes = pipesNeigbours
                                                                            .Zip(boxNeigbours, (pipe, boxNeigbour) => new { Pipe = pipe, Orientation = boxNeigbour.GetOrientation() })
                                                                            .ToDictionary(x => x.Pipe, x => x.Orientation);



            currentBox.GetPipe().SetPipeNeigbours(neigboursPipes);

        }
    }

    public List<Pipe> GetAllPipes() => allPipes;




}
