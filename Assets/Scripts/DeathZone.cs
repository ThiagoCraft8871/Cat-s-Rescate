using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto que entra tiene la etiqueta "Player"
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Llama a la función de muerte por caída para que baje la vida y active el Game Over
                player.MorirPorCaida();
            }
        }
    }
}