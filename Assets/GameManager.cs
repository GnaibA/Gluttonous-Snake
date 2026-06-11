using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private PlayerInput input;

    [Header("…Ë÷√")]
    [SerializeField] public int width;
    [SerializeField] public int height;

    private float tickTimer;
    [SerializeField] private float secondPerTick = .3f;

    [SerializeField] private int initialBodyLen = 3;

    [Header("◊ ‘¥")]
    [SerializeField] private Tilemap map;
    [SerializeField] private Tilemap snakeTile;

    [Header("Õﬂ∆¨Àÿ≤ƒ")]
    [SerializeField] private TileBase apple;
    [SerializeField] private TileBase ground;
    [SerializeField] private TileBase boundary;
    [SerializeField] private TileBase snakeBody;
    [SerializeField] private TileBase snakeHead;

    private List<Vector2Int> snake = new List<Vector2Int>();

    private Vector2Int currentInputDirection;
    private Vector2Int lastMoveDirection;

    void Start()
    {
        input = new PlayerInput();
        input.NormalInput.Enable();
        input.NormalInput.Up.performed += HandleInput;
        input.NormalInput.Down.performed += HandleInput;
        input.NormalInput.Left.performed += HandleInput;
        input.NormalInput.Right.performed += HandleInput;

        currentInputDirection = Vector2Int.right;
        lastMoveDirection = Vector2Int.right;
        InitTilemap();
        InitSnake();

        tickTimer = secondPerTick;
    }


    void Update()
    {
        tickTimer -= Time.deltaTime;
        if(tickTimer < 0)
        {
            tickTimer = secondPerTick;
            Tick();
        }
    }

    private void Tick()
    {
        // “∆∂Ø≈–∂œ
        Vector2Int targetPos = snake[0] + currentInputDirection;
        if (IsOutOfBoundary(targetPos))
            return;
        lastMoveDirection = currentInputDirection;
        
        // “∆∂Ø‰÷»æ
        snakeTile.ClearAllTiles();
        for(int i = snake.Count-1; i > 0; i--)    // …ﬂ…Ì
        {
            if (snake[i] == snake[i - 1]) continue;

            snake[i] = snake[i - 1];
            snakeTile.SetTile((Vector3Int)snake[i], snakeBody);
        }
        snake[0] = targetPos;   // …ﬂÕ∑
        snakeTile.SetTile((Vector3Int)snake[0], snakeHead);
    }

    private void OnDisable()
    {
        input.NormalInput.Disable();
    }

    private bool IsOutOfBoundary(Vector2Int position)
    {
        return position.x < 0 || position.x >= width || position.y < 0 || position.y >= height;
    }

    private void InitTilemap()
    {
        map.ClearAllTiles();

        // ÃÓ≥‰±ﬂΩÁ
        TileBase[] boundaryTiles = new TileBase[(width + 2) * (height + 2)];
        Array.Fill(boundaryTiles, boundary);
        map.SetTilesBlock(new BoundsInt(-1, -1, 0, width + 2, height + 2, 1), boundaryTiles);

        // ÃÓ≥‰÷˜ÃÂ
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
        if (snake.Count > 1 && lastMoveDirection == -dir)
            return;
        currentInputDirection = dir;
    }

    private void InitSnake()
    {
        Vector2Int snakeSpawnPoint = new Vector2Int(width / 2, height / 2);

        snakeTile.SetTile((Vector3Int)snakeSpawnPoint, snakeHead);
        snake.Add(snakeSpawnPoint);

        for(int i = 0; i < initialBodyLen; i++)
        {
            snakeTile.SetTile((Vector3Int)snakeSpawnPoint, snakeBody);
            snake.Add(snakeSpawnPoint);
        }
    }

    private void EatApple()
    {

    }
}
