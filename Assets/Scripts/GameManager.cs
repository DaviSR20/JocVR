using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid Settings")]
    [Range(2, 20)]
    public int gridSize = 6;
    public GridManager gridManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gridManager.GenerateGrid(gridSize);
    }

    public void SetGridSize(int size)
    {
        gridSize = size;
        gridManager.GenerateGrid(gridSize);
    }
}