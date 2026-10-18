using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Opcional pero útil: buscar automáticamente al jugador si no se asignó en el Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Seguridad por si el jugador se destruye o no está asignado
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            if(direction.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if(direction.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            movement = new Vector2(direction.x, 0);
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        animator.SetBool("enMovimiento", enMovimiento);
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // CAMBIO: Asegúrate de que se llame 'RecibeDanio' con R y D mayúsculas
            playerController.RecibeDanio(transform.position, 1);
        }
    }
}
}