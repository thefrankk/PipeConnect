using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

public abstract class Pipe : MonoBehaviour
{
    public int[] sides;

    public enum Type
    {
        Start,
        End,
        Corner,
        Horizontal
    }

    [SerializeField] protected Type currentType;


    protected Dictionary<Pipe, BoxNeigbours<PipeBox>.Orientation> neigboursPipes = new Dictionary<Pipe, BoxNeigbours<PipeBox>.Orientation>();
    protected List<Pipe> currentConnectedPipes = new List<Pipe>();
    protected List<Pipe> latestConnectedPipes = new List<Pipe>();

    [SerializeField] protected Pipe prevConnectedPipe;
    [SerializeField] protected Pipe nextConnectedPipe;

    protected Vector2 currentPosition;
    protected Vector2 currentPositionInRealWorld;

    public bool isConnectedToStart = false;

    protected void OnMouseDown()
    {
        rotateObject();
    }
    protected void rotateObject()
    {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -90));

        int[] newSides = new int[4];

        transform.rotation = Quaternion.Slerp(fromAngle, toAngle, 1);

        for (int i = 0; i < sides.Length; i++)
        {
            if (sides[i] == 1)
            {

                if (i < sides.Length - 1)
                {
                    newSides[i + 1] = 1;
                }
                else
                {
                    newSides[0] = 1;

                }

            }
        }
        sides = newSides;

        checkSides();
    }

    protected void checkSides(Pipe pipe = null)
    {

        currentConnectedPipes.Clear();

        foreach (var pipeNeigbours in neigboursPipes)
        {

           switch(pipeNeigbours.Value) //Orientations
            {
                case BoxNeigbours<PipeBox>.Orientation.Bottom:
                    checkTopSideFromBottomPipe(pipeNeigbours.Key);
                    break;
                case BoxNeigbours<PipeBox>.Orientation.Top:
                    checkBottomSideFromTopPipe(pipeNeigbours.Key);
                    break;
                case BoxNeigbours<PipeBox>.Orientation.Right:
                    checkLeftSideFromRightPipe(pipeNeigbours.Key);
                    break;
                case BoxNeigbours<PipeBox>.Orientation.Left:
                    checkRightSideFromLeftPipe(pipeNeigbours.Key);
                    break;
            }
        }


        TryConnectOrDisconnect(pipe);
    }
    private void checkTopSideFromBottomPipe(Pipe pipeToCheck)
    {
        int[] pipeToCheckSides = pipeToCheck.sides;
        (int, int) pairs = getPairs(sides, pipeToCheckSides, BoxNeigbours<PipeBox>.Orientation.Bottom);

        int firstOdd = pairs.Item1;
        int secondOdd = pairs.Item2; 


        if (pipeToCheckSides[firstOdd] == 1 && sides[secondOdd] == 1)
        {
            
            AddPipeInList(pipeToCheck);
        }
    }
    private void checkBottomSideFromTopPipe(Pipe pipeToCheck)
    {
        int[] pipeToCheckSides = pipeToCheck.sides;
        (int, int) pairs = getPairs(sides, pipeToCheckSides, BoxNeigbours<PipeBox>.Orientation.Top);

        int firstOdd = pairs.Item1;
        int secondOdd = pairs.Item2;


        if (pipeToCheckSides[secondOdd] == 1 && sides[firstOdd] == 1)
        {
            AddPipeInList(pipeToCheck);
        }
    }
    private void checkLeftSideFromRightPipe(Pipe pipeToCheck)
    {

        int[] pipeToCheckSides = pipeToCheck.sides;
        (int, int) pairs = getPairs(sides, pipeToCheckSides, BoxNeigbours<PipeBox>.Orientation.Right);

        int firstOdd = pairs.Item1; 
        int secondOdd = pairs.Item2; 


        if (pipeToCheckSides[secondOdd] == 1 && sides[firstOdd] == 1)
        {
            AddPipeInList(pipeToCheck);

        }

    }
    private void checkRightSideFromLeftPipe(Pipe pipeToCheck) 
    {
        int[] pipeToCheckSides = pipeToCheck.sides;
        (int, int) pairs = getPairs(sides, pipeToCheckSides, BoxNeigbours<PipeBox>.Orientation.Left);

        int firstOdd = pairs.Item1; 
        int secondOdd = pairs.Item2; 
        

        if (pipeToCheckSides[firstOdd] == 1 && sides[secondOdd] == 1)
        {
            AddPipeInList(pipeToCheck);
        }

    }

    private (int, int) getPairs(int[] pipeSides, int[] nextPipeSides, BoxNeigbours<PipeBox>.Orientation orientation)
    {
        int oddOrEven = (orientation == BoxNeigbours<PipeBox>.Orientation.Left || orientation == BoxNeigbours<PipeBox>.Orientation.Right) ? 1 : 0;


        int first = GetFirstSide(pipeSides, oddOrEven);
        int second = GetSecondSide(pipeSides, oddOrEven);
      
        return (first, second);   
    }

    private int GetFirstSide(int[] sides, int oddOrEven)
    {
        for (int i = 0; i < sides.Length; i++)
        {
            if (i % 2 == oddOrEven)
            {
                return i;
            }
        }
        return -1; // or throw an exception if desired
    }

    private int GetSecondSide(int[] sides, int oddOrEven)
    {
        int value = 0;
        for (int i = 0; i < sides.Length; i++)
        {
            if (i % 2 == oddOrEven)
            {
                value = i;
            }
        }
        return value; // or throw an exception if desired
    }
    private void TryConnectOrDisconnect(Pipe previusPipe = null)
    {

        List<Pipe> pipesNotConnected = latestConnectedPipes.Except(currentConnectedPipes).ToList();

        foreach (var pipe in pipesNotConnected)
        {
            if (pipe.isConnectedToStart)
            {
                disconnectPipe();
            }
            else
            {
                pipe.disconnectPipe();
            }
        }

        Pipe firstPipeToCheck = null;

        for (int i = 0; i < currentConnectedPipes.Count; i++)
        {
            if (previusPipe != null)
                if (currentConnectedPipes[i].GetInstanceID().Equals(previusPipe.GetInstanceID()))
                {
                    SetPrevConnectedPipe(previusPipe);
                    continue;
                }

            if (isConnectedToStart)
            {
                checkCurrentPipeConnectedToStart(currentConnectedPipes[i], previusPipe);

            }
            else if (currentConnectedPipes[i].isConnectedToStart)
            {
                checkConnectedPipeConnectedToStart(currentConnectedPipes[i], previusPipe);
            }


            if (currentConnectedPipes.Count > 1 && i >= currentConnectedPipes.Count - 1)
            {
                firstPipeToCheck = currentConnectedPipes[0];

                if (isConnectedToStart)
                {
                    checkCurrentPipeConnectedToStart(firstPipeToCheck, previusPipe);

                }
                else if(firstPipeToCheck.isConnectedToStart)
                {
                    checkConnectedPipeConnectedToStart(firstPipeToCheck, previusPipe);
                }
            }
        }
      
        latestConnectedPipes.Clear();
        latestConnectedPipes.AddRange(currentConnectedPipes);
    }

    protected virtual void checkCurrentPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {

        if(previusPipe != null)
            SetPrevConnectedPipe(previusPipe);

            SetNextConnectedPipe(pipe);

            pipe.isConnectedToStart = true;

            GetNextConnectedPipe().checkSides(this);
    }
    protected virtual void checkConnectedPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {
        SetPrevConnectedPipe(pipe);
        GetPrevConnectedPipe().checkSides(this);
        isConnectedToStart = true;
    }


    public abstract void DoConnectedLogic();
   
   
    protected virtual void disconnectPipe()
    {
        if (GetNextConnectedPipe() != null)
            GetNextConnectedPipe().disconnectPipe();

        if (GetPrevConnectedPipe() != null)
            GetPrevConnectedPipe().SetNextConnectedPipe(null);

        SetPrevConnectedPipe(null);
        SetNextConnectedPipe(null);



    }

    protected void AddPipeInList(Pipe pipe)
    {

        if (latestConnectedPipes.Count > 0)
        {
            currentConnectedPipes.Add(pipe);
        }
        else
        {
            currentConnectedPipes.Add(pipe);
            latestConnectedPipes.Add(pipe);
        }
    }

    
    

    public Type GetCurrentType() => currentType;
    public void SetCurrentPosition(Vector2 pos) => currentPosition = pos;
    public void SetCurrentPositionInRealWorld(Vector2 pos) => currentPositionInRealWorld = pos;

    public void SetPipeNeigbours(Dictionary<Pipe, BoxNeigbours<PipeBox>.Orientation> pipes)
    {
        neigboursPipes = pipes;
    }

    public void SetNextConnectedPipe(Pipe pipe) => nextConnectedPipe = pipe;
    public void SetPrevConnectedPipe(Pipe pipe) => prevConnectedPipe = pipe;

    public Pipe GetNextConnectedPipe() => nextConnectedPipe;
    public Pipe GetPrevConnectedPipe() => prevConnectedPipe;    


}

