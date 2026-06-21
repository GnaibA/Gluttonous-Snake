using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private PlayerInput input;

    public enum GameState { Playing, GameOver };

    [Header("游戏状态")]
    public GameState state = GameState.Playing;
    [SerializeField] private float timeScale = 1f;

    public int score = 0;
    public int highScore = 0;

    public event Action OnGameOver;
    public event Action OnRestart;

    [Header("游戏设置")]
    [SerializeField] public int width;
    [SerializeField] public int height;
    private float tickTimer;
    [SerializeField] private float secondPerTick = .3f;
    [SerializeField] private int initialBodyLen = 3;

    [Header("资源")]
    [SerializeField] private Tilemap map;

    [Header("瓦片素材")]
    [SerializeField] private TileBase ground;
    [SerializeField] private TileBase boundary;
    [SerializeField] private GameObject apple;
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private GameObject snakeBody;

    private List<SnakeSegment> snake = new List<SnakeSegment>();
    private HashSet<Vector2Int> snakeSet = new HashSet<Vector2Int>();
    [SerializeField] private Vector2Int applePos;

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
        lastMoveDirection = currentInputDirection;

        InitTilemap();
        InitSnake();
        InitApple();

        tickTimer = secondPerTick;

        Time.timeScale = timeScale;
    }

    private void InitApple()
    {
        // 苹果动效
        apple.transform.DOScale(1.5f, 1f).SetLoops(-1, LoopType.Yoyo);

        apple.transform.rotation = Quaternion.Euler(0, 0, -30);
        apple.transform.DORotate(new Vector3(0, 0, 30), 1.5f).SetLoops(-1, LoopType.Yoyo);

        SummonApple();
    }

    private void SummonApple()
    {
        ResetApplePosition();
        DrawApple();
    }

    private void DrawApple()
    {
        apple.transform.position = (Vector3Int)applePos;
    }

    private void ResetApplePosition()
    {
        List<Vector2Int> freeCells = new List<Vector2Int>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2Int point = new Vector2Int(i, j);
                if (!snakeSet.Contains(point))
                    freeCells.Add(point);
            }
        }

        applePos = freeCells[UnityEngine.Random.Range(0, freeCells.Count)];
    }

    private void EatApple()
    {
        score++;
        SummonApple();
        AddBodyLength();
    }

    void Update()
    {
        if (state == GameState.GameOver) return;

        tickTimer -= Time.deltaTime;
        if (tickTimer < 0)
        {
            tickTimer = secondPerTick;
            Tick();
        }
    }

    private void Tick()
    {
        // 移动判断
        Vector2Int targetPos = snake[0].position + currentInputDirection;
        if (IsOutOfBoundary(targetPos) || snakeSet.Contains(targetPos))
        {
            GameOver();
            return;
        }
        lastMoveDirection = currentInputDirection;

        Vector2Int snakeTail = snake[snake.Count - 1].position;

        // 更新移动坐标
        snake[0].direction = Vector2ToAngle(lastMoveDirection);
        for (int i = snake.Count - 1; i > 0; i--)
        {
            snake[i].position = snake[i - 1].position;

            // TODO:修改角度
            if (Mathf.Abs(snake[i].direction - snake[i - 1].direction) == 90)
                snake[i].direction = snake[i - 1].direction * .7f;
            else
                snake[i].direction = snake[i - 1].direction;
        }
        snake[0].position = targetPos;

        // 根据坐标渲染
        for (int i = 0; i < snake.Count; i++)
        {
            snake[i].transform.DOMove((Vector3Int)snake[i].position, secondPerTick);
            snake[i].transform.DORotate(new Vector3(0, 0, snake[i].direction), secondPerTick,RotateMode.Fast);
        }

        // 更新snakeSet
        snakeSet.Add(targetPos);
        if (snake[snake.Count - 1].position != snakeTail)
            snakeSet.Remove(snakeTail);

        // 是否吃掉苹果
        if (targetPos == applePos)
            EatApple();
    }

    private void OnDisable()
    {
        input.NormalInput.Up.performed -= HandleInput;
        input.NormalInput.Down.performed -= HandleInput;
        input.NormalInput.Left.performed -= HandleInput;
        input.NormalInput.Right.performed -= HandleInput;
        input.NormalInput.Disable();
    }

    private bool IsOutOfBoundary(Vector2Int position)
    {
        return position.x < 0 || position.x >= width || position.y < 0 || position.y >= height;
    }

    private void InitTilemap()
    {
        map.ClearAllTiles();

        // 填充边界
        TileBase[] boundaryTiles = new TileBase[(width + 2) * (height + 2)];
        Array.Fill(boundaryTiles, boundary);
        map.SetTilesBlock(new BoundsInt(-1, -1, 0, width + 2, height + 2, 1), boundaryTiles);

        // 填充主体
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

        // 初始化蛇头和蛇身
        GameObject head = Instantiate(snakeHead);
        head.transform.position = (Vector3Int)snakeSpawnPoint;
        snake.Add(new SnakeSegment(snakeSpawnPoint, head.transform, Vector2ToAngle(lastMoveDirection)));

        for (int i = 0; i < initialBodyLen; i++)
        {
            GameObject body = Instantiate(snakeBody);
            body.transform.position = (Vector3Int)snakeSpawnPoint;
            snake.Add(new SnakeSegment(snakeSpawnPoint, body.transform, Vector2ToAngle(lastMoveDirection)));
        }

        snakeSet.Add(snakeSpawnPoint);
    }

    private void AddBodyLength()
    {
        // 添加一节身体
        SnakeSegment snakeTail = snake[snake.Count - 1];
        GameObject body = Instantiate(snakeBody);
        body.transform.position = (Vector3Int)snakeTail.position;
        snake.Add(new SnakeSegment(snakeTail.position, body.transform, snakeTail.direction));
    }

    private void GameOver()
    {
        state = GameState.GameOver;

        if (score > highScore) highScore = score;

        OnGameOver?.Invoke();
    }

    public void Restart()
    {
        if (state == GameState.Playing) return; // 防误触发 

        state = GameState.Playing;
        score = 0;

        currentInputDirection = Vector2Int.right;
        lastMoveDirection = Vector2Int.right;

        ClearSnakeList(snake);
        snakeSet.Clear();

        InitSnake();
        InitApple();

        tickTimer = secondPerTick;

        OnRestart?.Invoke();
    }

    private void ClearSnakeList(List<SnakeSegment> snake)
    {
        foreach (var obj in snake)
            Destroy(obj.transform.gameObject);

        snake.Clear();
    }

    private float Vector2ToAngle(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
