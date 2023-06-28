using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class PipeBox : Box
{
    protected Pipe currentPipe;
    protected bool isPartOfThePath;

    public enum BoxType
    {
        None,
        Start,
        End,
        Path,
    }

    public BoxType type;


    public void SetPipe(Pipe pipe)
    {
        this.currentPipe = pipe;

    }

    public Pipe GetPipe() => this.currentPipe;


    public void SetIsPartOfThePath(bool value) => isPartOfThePath = value;
    public bool GetIsPartOfThePath() => isPartOfThePath;
}

