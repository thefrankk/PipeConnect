using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSize : MonoBehaviour
{

    private int gridWidth;
    private int gridHeight;

    private float boxSizeWidth;
    private float boxSizeHeight;

    private float gridSizeHeight;
    private float gridSizeWidth;

    Camera cam;

    float screenHeight;
    float screenWidth;


    GridSystem<PipeBox> gridSystem;

    private Vector3 gridPosition;
    private void Awake()
    {
        cam = Camera.main;
        screenHeight = Screen.height;
        screenWidth = Screen.width;

    }
    private void OnEnable()
    {
        Game.GameStarted += defineSize;
    }
    private void OnDisable()
    {
        Game.GameStarted -= defineSize;

    }

    private void Start()
    {
    }
    private void defineSize()
    {
        gridSystem = Game.GetGridSystem();

        PipeBox[,] box = gridSystem.GetGridArray();

        SpriteRenderer boxSpriteRenderer = box[0, 0].GetComponent<SpriteRenderer>();   

        gridWidth = box.GetLength(0);
        gridHeight = box.GetLength(1);

        Bounds spriteBounds = boxSpriteRenderer.bounds;

        Vector3 bottomLeft = Camera.main.WorldToScreenPoint(spriteBounds.min);
        Vector3 topRight = Camera.main.WorldToScreenPoint(spriteBounds.max);

        float spriteScreenHeight = (topRight.y - bottomLeft.y) * gridHeight;
        float spriteScreenWidth = (topRight.x - bottomLeft.x) * gridWidth;

        

        gridPosition = cam.WorldToScreenPoint(this.gameObject.transform.position);

        Debug.Log(screenWidth);
        Debug.Log(screenHeight);


        Debug.Log(spriteScreenWidth);
        Debug.Log(spriteScreenHeight);

        float xStartPosition = (screenWidth - spriteScreenWidth) / 2;
        float yStartPosition = (screenHeight - spriteScreenHeight) / 2;

        setPosition(xStartPosition, yStartPosition);
    }


    private void setPosition(float x, float y)
    {
        Vector2 worldPosition = new Vector2(x, y);
        Vector2 screenPosition = new Vector2();

        Debug.Log("world pos" + worldPosition);

        screenPosition = cam.ScreenToWorldPoint(worldPosition);

        Debug.Log("Scrween pos" + screenPosition);

        this.transform.position = screenPosition;


    }


}
