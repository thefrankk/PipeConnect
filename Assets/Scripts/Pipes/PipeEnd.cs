using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeEnd : Pipe
{

    public Action OnEndReached;

    private void OnEnable()
    {
        isConnectedToStart = false;

        checkSides();
    }


    protected override void checkConnectedPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {
        pipe.SetNextConnectedPipe(this);
        OnEndReached?.Invoke();
        base.checkConnectedPipeConnectedToStart(pipe, previusPipe);

     
    }
    protected override void checkCurrentPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {
        PipeEnd pipePath = pipe as PipeEnd;

      

        if (pipe.isConnectedToStart) return;

        

        base.checkCurrentPipeConnectedToStart(pipe, previusPipe);


        pipe.DoConnectedLogic();

       
    }
    protected override void disconnectPipe()
    {
        base.disconnectPipe();

        isConnectedToStart = false;
    }
    public override void DoConnectedLogic()
    {
        OnEndReached?.Invoke();
    }
}
