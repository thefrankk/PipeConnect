using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Box : MonoBehaviour
{
    protected Vector2 pos;

    Color color = Color.white;
    SpriteRenderer spriteRenderer;

    protected List<Box> neighboursBoxes = new List<Box>();

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;

    }


    public void SetPosition(Vector2 pos) => this.pos = pos;

    public void ChangeColor(Color color) => spriteRenderer.color = color;

    public Vector2 GetCurrentPosition() => pos;

}







