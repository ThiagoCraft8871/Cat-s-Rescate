using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    private Rigidbody2D rb;
    private bool enMovimiento;
    private Animator animator;
    private bool recibiendoDanio;
    public float fuerzaRebote = 10f;
    [SerializeField] float duracionDanio = 0.4f;
    private float contadorDanio;
    private bool playerVivo = true;
    private bool muerto;
    public int vida = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Buscar automáticamente al jugador si no se asignó en el Inspector
        if (player == null)
        {
            playerVivo = true;
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
        // Si el jugador no existe en la escena, detenemos todo
        if (player == null) return;

        // Verificamos el estado de vida del jugador en tiempo real
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && playerController.muerto)
        {
            // Si el jugador está muerto, el enemigo se detiene y pasa a idle
            enMovimiento = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("enMovimiento", false);
            return; 
        }

        // Si sigue vivo y no está muerto, continúa con su lógica normal
        if (playerVivo && !muerto && !recibiendoDanio)
        {
            Movimiento();
        }
        
        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("muerto", muerto);
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            
            if(direction.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if(direction.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            // Usamos rb.velocity en lugar de MovePosition para respetar la gravedad y el suelo (Tilemap)
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
            enMovimiento = true;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            enMovimiento = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.RecibeDanio(transform.position, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ataque"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccionEnemigo, int cantDamage)
    {
        if (!recibiendoDanio)
        {
            vida -= cantDamage;
            recibiendoDanio = true;
            enMovimiento = false;

            if (vida <= 0)
            {
                muerto = true;
                rb.velocity = Vector2.zero;
            }
            else
            {
                contadorDanio = duracionDanio; 
                rb.velocity = Vector2.zero; 
                float direccionX = transform.position.x > direccionEnemigo.x ? 1f : -1f;
                Vector2 fuerzaDeRebote = new Vector2(direccionX, 1.5f).normalized;
                rb.AddForce(fuerzaDeRebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivaDanio());
            }
        }
    }

    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(duracionDanio);
        recibiendoDanio = false;
    }

    public void EliminarCuerpo()
    {
        Destroy(gameObject);
    }
}