using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegresoCoin : MonoBehaviour
{
    [SerializeField] private float destinoX = 0f;
    [SerializeField] private float destinoY = 0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = new Vector2(destinoX, destinoY);
        }
    }
}
