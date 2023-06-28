using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[RequireComponent(typeof(PipesInGame))]
public class PipesInPathCreator : MonoBehaviour
{
    [SerializeField] private Pipe[] pipes;


    [SerializeField] private Pipe startPipe;
    [SerializeField] private Pipe endPipe;


    private List<PipeBox> boxesInPath = new List<PipeBox>();
    private List<Pipe> allPipes = new List<Pipe>();

    private LinkedList<PipeBox> boxesInPathLinkedList = new LinkedList<PipeBox>();

    private LinkedListNode<PipeBox> node;

    GridSystem<PipeBox> gridSystem;


    public Action<List<Pipe>> OnPipesCreated;
    private void OnEnable()
    {
        PathCreator.OnPathCreated += AssignBoxesInPath;
    }

    private void OnDisable()
    {
        PathCreator.OnPathCreated -= AssignBoxesInPath;
    }

  
    private void AssignBoxesInPath(List<PipeBox> boxesInPath)
    {
        Debug.Log("No se va");
        gridSystem = Game.GetGridSystem();


        boxesInPathLinkedList.Clear();
        allPipes.Clear();

        this.boxesInPath = boxesInPath;

        

        CreateStart();
        CreateEnd();

        PlacePipes();
        placeOtherPipes();

     
    }

    private void placeOtherPipes()
    {
        PipeBox[,] allBoxes = gridSystem.GetGridArray();

        for (int x = 0; x < allBoxes.GetLength(0); x++)
        {
            for (int y = 0; y < allBoxes.GetLength(1); y++)
            {
                if (allBoxes[x, y].GetPipe() == null)
                {
                    Pipe pipe = Instantiate(pipes[UnityEngine.Random.Range(0, pipes.Length)], allBoxes[x, y].transform.position, Quaternion.identity, allBoxes[x, y].transform);
                    pipe.SetCurrentPosition(allBoxes[x, y].GetCurrentPosition());
                    allBoxes[x, y].SetPipe(pipe);
                    allPipes.Add(pipe);
                }
            }
        }

        OnPipesCreated?.Invoke(allPipes);


        

    }

    public void CreateStart()
    {
        PipeBox firstBox = boxesInPath[boxesInPath.Count - 1];
        Pipe pipe = Instantiate(startPipe, firstBox.transform.position, startPipe.transform.rotation, firstBox.transform);
        firstBox.SetPipe(pipe);
        allPipes.Add(pipe);
    }

    public void CreateEnd()
    {
        PipeBox lastBox = boxesInPath[boxesInPath.Count - 2 ];
        Pipe pipe = Instantiate(endPipe, lastBox.transform.position, endPipe.transform.rotation, lastBox.transform);
        lastBox.SetPipe(pipe);
    }

    

    private void PlacePipes()
    {
        boxesInPath.Reverse();
        PipeBox lastBox = null;

        foreach (var box in boxesInPath)
        {
            if (box.GetPipe() != null)
            {
                if(box.GetPipe().GetCurrentType() == Pipe.Type.End)
                {
                    lastBox = box;
                    allPipes.Add(box.GetPipe());

                    continue;
                }

                boxesInPathLinkedList.AddLast(box);
                continue;
            }
            Pipe pipe = Instantiate(pipes[UnityEngine.Random.Range(0, pipes.Length)], box.transform.position, Quaternion.identity, box.transform);
            box.SetPipe(pipe);
            pipe.SetCurrentPositionInRealWorld(box.transform.position);
            pipe.SetCurrentPosition(box.GetCurrentPosition());
            
            allPipes.Add(pipe);
            boxesInPathLinkedList.AddLast(box);
          //  await Task.Delay(100);

        }
        boxesInPathLinkedList.AddLast(lastBox);
        node = boxesInPathLinkedList.First;

       OrderPipes(node.Next);

    }


    private void OrderPipes(LinkedListNode<PipeBox> boxNode)
    {
        if(boxNode.Next == null) return;

        //from IZQ to DER == 1 impar to 2do impar
        //From DER TO IZQ == 2 impar to 1do impar
        //From top to Down == 2 par to 1er par
        //from down to Top == 1 par to 2do par

        int sides = 4;


        LinkedListNode<PipeBox> nextBoxNode = boxNode.Next;
        LinkedListNode<PipeBox> prevBoxNode = boxNode.Previous;

        bool firstOrientationPar;
        bool secondOrientationPar;


        BoxNeigbours<PipeBox>.Orientation nextOrientation = getOrientation(boxNode, nextBoxNode);
        BoxNeigbours<PipeBox>.Orientation prevOrientation = getOrientation(prevBoxNode, boxNode);

        firstOrientationPar = (int)nextOrientation % 2 == 0 ? true : false;
        secondOrientationPar = (int)prevOrientation % 2 == 0 ? true : false;


        if(firstOrientationPar.Equals(secondOrientationPar))
        {
            if (boxNode.Value.GetPipe().GetCurrentType() == Pipe.Type.Corner)
            {
                Pipe newPipe = changePipe(boxNode.Value.GetPipe());
                Destroy(boxNode.Value.GetPipe().gameObject);
                Pipe pipeInstance = Instantiate(newPipe, boxNode.Value.transform.position, Quaternion.identity, boxNode.Value.transform);
                boxNode.Value.SetPipe(pipeInstance);

              // await Task.Delay(50);

            }
        }
        else
        {
            if (boxNode.Value.GetPipe().GetCurrentType() == Pipe.Type.Horizontal)
            {
                Pipe newPipe = changePipe(boxNode.Value.GetPipe());
                Pipe pipeInstance = Instantiate(newPipe, boxNode.Value.transform.position, Quaternion.identity, boxNode.Value.transform);
                Destroy(boxNode.Value.GetPipe().gameObject);
                boxNode.Value.SetPipe(pipeInstance);
               // await Task.Delay(50);

            }
        }


        OrderPipes(nextBoxNode);

    } 

  
    private Pipe changePipe(Pipe currentPipe)
    {
        Pipe pipeToReturn;
        return pipeToReturn = currentPipe.GetCurrentType() == Pipe.Type.Corner ? pipes[1] : pipes[0];
    }

    //Get where is orientated the next pipe
    private BoxNeigbours<PipeBox>.Orientation getOrientation(LinkedListNode<PipeBox> currentBox, LinkedListNode<PipeBox> nextBox)
    {
        List<BoxNeigbours<PipeBox>> boxNeigbours = gridSystem.GetNeighbors(Extensions.GetTupleFromVector(currentBox.Value.GetCurrentPosition()));

        BoxNeigbours<PipeBox>.Orientation orientation = BoxNeigbours<PipeBox>.Orientation.Left; //Place holder..
        foreach (var item in boxNeigbours)
        {

           
            if(item.GetBox().GetCurrentPosition().Equals(nextBox.Value.GetCurrentPosition()))
            {
                orientation = item.GetOrientation();
                break;
            }
        }

        return orientation;
    }


    public List<Pipe> GetAllPipesInBoard() => allPipes;
    public Pipe GetEndPipe() => endPipe;
   
}
