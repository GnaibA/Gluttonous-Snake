using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private PlayerInput input;

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private Tilemap map;
    [SerializeField] private Tilemap snakeTile;

    [Header("ÍßÆ¬ËØ²Ä")]
    [SerializeField] private TileBase apple;
    [SerializeField] private TileBase ground;
    [SerializeField] private TileBase boundary;
    [SerializeField] private TileBase snakeBody;
    [SerializeField] private TileBase snakeHead;

    private List<Vector2Int> snake = new List<Vector2Int>();

    private Vector2Int currentDirection;

    void Start()
    {
        input = new PlayerInput();
        input.NormalInput.Enable();
        input.NormalInput.Up.performed += HandleInput;
        input.NormalInput.Down.performed += HandleInput;
        input.NormalInput.Left.performed += HandleInput;
        input.NormalInput.Right.performed += HandleInput;

        currentDirection = Vector2Int.right;
        InitTilemap();
        InitSnake();
    }


    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // ÒÆ¶¯ÅÐ¶Ï
        Vector2Int targetPos = snake[0] + currentDirection;
        if (IsOutOfBoundary(targetPos))
            return;
        
        // ÒÆ¶¯äÖÈ¾
        snake[0] = targetPos;
        snakeTile.ClearAllTiles();
        snakeTile.SetTile((Vector3Int)snake[0], snakeHead);
    }

    private void OnDisable()
    {
        input.NormalInput.Disable();
    }

    private bool IsOutOfBoundary(Vector2Int position)
    {
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
            return true;
        return false;
    }

    private void InitTilemap()
    {
        map.ClearAllTiles();

        // Ìî³ä±ß½ç
        TileBase[] boundaryTiles = new TileBase[(width + 2) * (height + 2)];
        Array.Fill(boundaryTiles, boundary);
        map.SetTilesBlock(new BoundsInt(-1, -1, 0, width + 2, height + 2, 1), boundaryTiles);

        // Ìî³äÖ÷Ìå
        TileBase[] groundTiles = new TileBase[width * height];
        Array.Fill(groundTiles, ground);
        map.SetTilesBlock(new BoundsInt(0, 0, 0, width, height, 1), groundTiles);
    }

    private void HandleInput(InputAction.CallbackContext ctx)
    {
        Vector2Int dir;

        if (ctx.action == input.NormalInput.Up) dir = Vector2Int.up;
        else if (ctx.action == input.NormalInput.Down) dir = Vector2Int.down;
        else if (ctx.action == input.NormalInput.Left) dir = Vector2Int.left;
        else dir = Vector2Int.right;

        TryChangeDirection(dir);
    }

    private void TryChangeDirection(Vector2Int dir)
    {
        if (snake.Count > 1 && currentDirection == -dir)
            return;
        currentDirection = dir;
    }

    private void InitSnake()
    {
        Vector2Int snakeSpawnPoint = new Vector2Int(width / 2, height / 2);

        snakeTile.SetTile((Vector3Int)snakeSpawnPoint, snakeHead);
        snake.Add(snakeSpawnPoint);
    }
}
