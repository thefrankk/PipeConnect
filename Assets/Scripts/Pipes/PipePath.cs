using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PipePath : Pipe
{

    SpriteRenderer spriteRenderer;
    UnityEngine.Color color = UnityEngine.Color.white;

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;

    }

    private void OnEnable()
    {

        isConnectedToStart = false;
        checkSides();
    }

    protected override void disconnectPipe()
    {
        base.disconnectPipe();

        ChangeColor(UnityEngine.Color.white);
        isConnectedToStart = false;
    }

    public void ChangeColor(UnityEngine.Color color) => spriteRenderer.color = color;

    protected override void checkConnectedPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {
        pipe.SetNextConnectedPipe(this);
        base.checkConnectedPipeConnectedToStart(pipe, previusPipe);
        ChangeColor(UnityEngine.Color.green);

    }
    protected override void checkCurrentPipeConnectedToStart(Pipe pipe, Pipe previusPipe = null)
    {
        PipePath pipePath = pipe as PipePath;

        if (pipe.isConnectedToStart) return;

        base.checkCurrentPipeConnectedToStart(pipe, previusPipe);
        //pipePath.ChangeColor(UnityEngine.Color.green);

        pipe.DoConnectedLogic();



    }

    public override void DoConnectedLogic()
    {
        ChangeColor(UnityEngine.Color.green);
    }
}
