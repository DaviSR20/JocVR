using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Materiales de ejemplo")]
    public Material Apagat; // Material por defecto (tile apagado)
    public Material Verd;   // Material para un tile activo o pulsado
    public Material Blau;   // Material temporal para otros tiles

    // Diccionario para guardar todos los tiles de la escena usando su ID como clave
    private Dictionary<string, Tile_test> tiles = new Dictionary<string, Tile_test>();

    private void Start()
    {
        // 1 Buscar todos los tiles de la escena
        Tile_test[] todosLosTiles = FindObjectsOfType<Tile_test>();

        foreach (var tile in todosLosTiles)
        {
            // 22 Generamos una clave única usando la ID del tile
            string key = tile.id.ToString();

            // 3 Guardamos el tile en el diccionario si no está ya
            if (!tiles.ContainsKey(key))
                tiles.Add(key, tile);

            // 4 Inicialmente todos los tiles tienen el material por defecto (Apagat)
            tile.SetMaterial(Apagat);
        }
    }

    // ==============================
    // Devuelve el material que debe usar un tile según su ID
    // Por defecto todos empiezan con Apagat
    // Este método se puede ampliar para reglas de colores más complejas
    // ==============================
    public Material GetMaterialForTile(Tile_test.TokenID id)
    {
        // Aquí podríamos aplicar reglas según la ID
        // Ejemplo: si id == (0,0) devolver Verd, si id == (1,1) devolver Blau, etc.
        return Apagat;
    }

    // ==============================
    // Método que se llama cuando un tile es pulsado por el Player
    // tile: referencia al tile que ha sido pulsado
    // ==============================
    public void TilePressed(Tile_test.TokenID id, Tile_test tile)
    {
        Debug.Log($"GameManager: Tile pulsado {id}");

        // Ejemplo de regla simple:
        // Si el tile pulsado es (0,0), hacemos algo especial
        if (id.x == 0 && id.y == 0)
        {
            // Cambiamos el tile pulsado a Verd mientras se mantiene el Player encima
            tile.SetMaterial(Verd);

            // Cambiamos temporalmente otros tiles a Blau (ejemplo)
            Tile_test.TokenID[] otros = new Tile_test.TokenID[]
            {
                new Tile_test.TokenID(0,1),
                new Tile_test.TokenID(1,0),
                new Tile_test.TokenID(1,1)
            };

            foreach (var t in otros)
            {
                if (tiles.ContainsKey(t.ToString()))
                    tiles[t.ToString()].SetMaterial(Blau);
            }

            // Después de 1 segundo, esos tiles vuelven al color por defecto (Apagat)
            StartCoroutine(ResetTilesAfterDelay(otros, 1f));
        }

        // Aquí se pueden añadir más reglas según otras IDs o colores
    }

    // ==============================
    // Método que se llama cuando el Player deja de pulsar un tile
    // Por ejemplo, para restaurar el color del tile pulsado o realizar acciones
    // ==============================
    public void TileReleased(Tile_test.TokenID id, Tile_test tile)
    {
        // Ejemplo: mantener Verd en el tile pulsado
        if (id.x == 0 && id.y == 0)
        {
            tile.SetMaterial(Verd);
        }

        // Aquí se pueden añadir más reglas según la ID
    }

    // ==============================
    // Coroutine para volver los tiles temporales a Apagat después de un tiempo
    // ==============================
    private IEnumerator ResetTilesAfterDelay(Tile_test.TokenID[] tilesAResetear, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var t in tilesAResetear)
        {
            if (tiles.ContainsKey(t.ToString()))
                tiles[t.ToString()].SetMaterial(Apagat);
        }
    }
}