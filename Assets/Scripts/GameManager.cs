using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid Settings")]
    [Range(2, 20)]
    public int gridSize = 6;
    public GridManager GridManagerWithBorders;

    [Header("UI Start Message")]
    public TextMeshProUGUI startText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        // Mostrar mensaje inicial
        if(startText != null)
        {
            startText.gameObject.SetActive(true);

            for(int i = 5; i > 0; i--)
            {
                startText.text = $"Colócate en el centro del espacio...\nEmpieza en {i}";
                yield return new WaitForSeconds(1f);
            }

            startText.gameObject.SetActive(false);
        }

        // Generar grid
        GridManagerWithBorders.GenerateGrid(gridSize);
    }

    public void SetGridSize(int size)
    {
        gridSize = size;
        GridManagerWithBorders.GenerateGrid(gridSize);
    }
}