using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    [Header("Map Size")]
    private int width = 20;
    private int height = 15;
    [SerializeField] private float cellSize = 1f;

    [Header("Boundary")]
    [SerializeField] private int boundarySize = 1;  // 边界占几格

    void Start()
    {
        width = gm.width;
        height = gm.height;

        FitCamera();
    }

    public void FitCamera()
    {
        Camera camera = GetComponent<Camera>();
        if (camera == null) camera = Camera.main;
        if (camera == null) return;

        float totalW = (width + boundarySize * 2) * cellSize;
        float totalH = (height + boundarySize * 2) * cellSize;

        transform.position = new Vector3(width / 2f, height / 2f, -10f);

        float screenAspect = (float)Screen.width / Screen.height;
        float mapAspect = totalW / totalH;

        if (screenAspect >= mapAspect)
            camera.orthographicSize = totalH / 2f;
        else
            camera.orthographicSize = totalW / (2f * screenAspect);
    }
}
