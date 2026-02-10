using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid Settings")]
    [Range(2, 20)]
    public int gridSize = 6;
    public GridManagerWithBorders GridManagerWithBorders;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GridManagerWithBorders.GenerateGrid(gridSize);
    }

    public void SetGridSize(int size)
    {
        gridSize = size;
        GridManagerWithBorders.GenerateGrid(gridSize);
    }
}