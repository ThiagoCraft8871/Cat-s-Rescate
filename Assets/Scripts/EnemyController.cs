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
            movement = Vector2.zero;
            enMovimiento = false;
            animator.SetBool("enMovimiento", false);
            return; // Sale del Update para que no siga persiguiendo
        }

        // Si sigue vivo, continúa con su lógica normal
        if (playerVivo && !muerto)
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
        
        if(!recibiendoDanio)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // CORREGIDO: Se llama una sola vez pasando la posición del enemigo correctamente
                playerController.RecibeDanio(transform.position, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ataque"))
        {
            // Aquí sí existe 'collision' y podemos calcular la dirección del ataque del jugador
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
            if (vida <= 0)
            {
                muerto = true;
                enMovimiento = false;
            }
            else
            {
                contadorDanio = duracionDanio; 
                rb.velocity = Vector2.zero; 
                float direccionX = transform.position.x > direccionEnemigo.x ? 1f : -1f;
                Vector2 fuerzaDeRebote = new Vector2(direccionX, 1.5f).normalized;
                rb.AddForce(fuerzaDeRebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivaDanio());
                if(!playerVivo)
                {
                    enMovimiento = false;
                }
            }
        }
    }

    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
    }
    public void EliminarCuerpo()
    {
        Destroy(gameObject);
    }
}