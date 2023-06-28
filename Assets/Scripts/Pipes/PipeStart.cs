using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeStart : Pipe
{
    private void Start()
    {
        isConnectedToStart = true;
        checkSides();
    }


    protected override void checkConnectedPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {
        pipe.SetNextConnectedPipe(this);
        base.checkConnectedPipeConnectedToStart(pipe, previusPipe);

    }
    protected override void checkCurrentPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {
        PipeStart pipePath = pipe as PipeStart;

        if (pipe.isConnectedToStart) return;

        base.checkCurrentPipeConnectedToStart(pipe, previusPipe);
        pipe.DoConnectedLogic();

    }
    protected override void disconnectPipe()
    {
        base.disconnectPipe();
    }
    public override void DoConnectedLogic()
    {
        //Do nothing..
    }
}
