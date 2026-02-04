using UnityEngine;

public class CubeTrigger : MonoBehaviour
{
    public GameManager gameManager;
    public CubeMaterialController cubeController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player ha entrat al cub {cubeController.id}");
            gameManager.PlayerHaEntrat(cubeController.id, true); // true = està sent pulsat
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player ha sortit del cub {cubeController.id}");
            gameManager.PlayerHaSortit(cubeController.id); // avisa que ha deixat de tocar-lo
        }
    }
}
