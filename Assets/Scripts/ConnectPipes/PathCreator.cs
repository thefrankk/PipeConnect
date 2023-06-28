using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class PathCreator : MonoBehaviour
{



    [SerializeField] private Button path;
    Vector2 startPos;
    Vector2 endPos;

    private List<PipeBox> boxesPath = new List<PipeBox>();
    private Stack<PipeBox> visitedBoxes = new Stack<PipeBox>();

    public static event Action<List<PipeBox>> OnPathCreated;
    GridSystem<PipeBox> gridSystem;


    public void StartPath()
    {
        gridSystem = Game.GetGridSystem();

        int minXPos = 0;
        int maxXPos = gridSystem.GetWidth() - 1;
        int minYPos = 0;


        int xPos = UnityEngine.Random.Range(minXPos, maxXPos);
        int yPos = minYPos;

        startPos = new Vector2(xPos, yPos);

        CreateStart();
        CreateEnd();

        path.onClick.AddListener(() => { DeletePath(); CreateStart(); CreateEnd(); createFirstBox(); CreatePath(new Vector2(startPos.x, startPos.y + 1)); });

        Debug.Log("start path");
    }

    // Start is called before the first frame update
    public void CreateStart()
    {
        int minXPos = 0;
        int maxXPos = gridSystem.GetWidth() - 1;
        int minYPos = 0;


        int xPos = UnityEngine.Random.Range(minXPos, maxXPos);
        int yPos = minYPos;

        startPos = new Vector2(xPos, yPos);

        PipeBox box = getGridObject((int)startPos.x, (int)startPos.y);
        visitedBoxes.Push(box);

        box.ChangeColor(Color.gray);
        box.type = PipeBox.BoxType.Start;



    }

    public void CreateEnd()
    {
        int minXPos = 0;
        int maxXPos = gridSystem.GetWidth() - 1;
        int minYPos = gridSystem.GetHeight() - 1;

        int xPos = UnityEngine.Random.Range(minXPos, maxXPos);
        int yPos = minYPos;

        endPos = new Vector2(xPos, yPos);

        PipeBox box = getGridObject((int)endPos.x, (int)endPos.y);

      
        box.ChangeColor(Color.gray);
        box.type = PipeBox.BoxType.End;
        visitedBoxes.Push(box);

    }

    private void createFirstBox()
    {
        PipeBox firstBox = getGridObject((int)startPos.x, (int)startPos.y + 1);
        visitedBoxes.Push(firstBox);

      //  firstBox.ChangeColor(Color.green);
        firstBox.SetIsPartOfThePath(true);
        firstBox.type = PipeBox.BoxType.Path;

    }


    public void DeletePath()
    {
        visitedBoxes.Clear();

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int y = 0; y < gridSystem.GetHeight(); y++)
            {
                PipeBox box = gridSystem.GetGridArray()[x, y];
                box.ChangeColor(Color.white);

                if (box.GetPipe() != null)
                    Destroy(box.GetPipe().gameObject);

                box.SetIsPartOfThePath(false);
                box.SetPipe(null);
                box.type = PipeBox.BoxType.None;
            }
        }

    }

    private List<BoxNeigbours<PipeBox>> getNeighbours(Vector2 pos) => gridSystem.GetNeighbors(Extensions.GetTupleFromVector(pos));
    private PipeBox getGridObject(int x, int y) => gridSystem.GetGridObject(x, y);
    PipeBox impossiblePipe = null;

    public void CreatePath(Vector2 pos)
    {
        (int, int) randNeighbour = (0, 0);
        int counter = 0;
        List<(int, int)> neighbours = getNeighbours(pos).Select(x => x.GetPosition())
                                                        .ToList();
                                                        

        counter = neighbours.Count;
        PipeBox box = null;
        Pipe pipe = null;

        foreach (var item in neighbours)
        {
            try
            {
                if (getGridObject(item.Item1, item.Item2 + 1) != null)
                {
                    PipeBox lastBox = getGridObject(item.Item1, (item.Item2 + 1));
                    PipeBox currentBox = getGridObject(item.Item1, (item.Item2));
                    if (lastBox.type == PipeBox.BoxType.End)
                    {
                       // Debug.Log(lastBox.type);
                        currentBox.SetIsPartOfThePath(true);
                       // currentBox.ChangeColor(Color.green);
                        visitedBoxes.Push(currentBox);
                       // Debug.Log("Has llegado");

                        boxesPath = visitedBoxes.ToList();

                        OnPathCreated?.Invoke(boxesPath);



                        return;
                    }
                }
            }
            catch (System.IndexOutOfRangeException)
            {

                Debug.LogWarning("Impossible to reach Y + 1");
            }
            
          
        }

        do
        {
            if (counter <= 0) break;

            int randNumber = UnityEngine.Random.Range(0, neighbours.Count());
            randNeighbour = neighbours[randNumber];
            box = getGridObject(randNeighbour.Item1, randNeighbour.Item2);
            neighbours.RemoveAt(randNumber);

            if (!box.GetIsPartOfThePath())
            {
                visitedBoxes.Push(box);
                if(impossiblePipe != null)
                {
                   // impossiblePipe.ChangeColor(Color.green);
                    visitedBoxes.Push(impossiblePipe);
                    impossiblePipe.SetIsPartOfThePath(true);
                    impossiblePipe.type = PipeBox.BoxType.Path;

                    impossiblePipe = null;


                }
            }

            counter--;

        } while (box.GetIsPartOfThePath() && counter > 0);


        if(counter <= 0 && box.GetIsPartOfThePath())
        {
            impossiblePipe = visitedBoxes.Pop();
           // impossiblePipe.ChangeColor(Color.red);
        
            CreatePath(impossiblePipe.GetCurrentPosition());
            return;
        }

       // box.ChangeColor(Color.green);
        box.SetIsPartOfThePath(true);
        box.type = PipeBox.BoxType.Path;


        //  await Task.Delay(100);
        CreatePath(new Vector2(randNeighbour.Item1, randNeighbour.Item2));

    }
}
