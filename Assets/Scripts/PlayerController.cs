using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 7f;
    bool isGrounded = false;
    Rigidbody2D rb;
    [SerializeField] LayerMask groundLayer;
    SpriteRenderer sprite;
    Animator anim;
    Vector2 savePoint;
    [SerializeField] float RespawnHeight;
    private bool recibiendoDanio;
    public float fuerzaRebote = 10f;
    private bool atacando;
    [Header("Tiempo de Daño")]
    [SerializeField] float duracionDanio = 0.4f;
    private float contadorDanio;
    [Header("Configuración de Ataque")]
    [SerializeField] float duracionAtaque = 0.3f; // Cuánto dura la animación de ataque activa
    [SerializeField] float cooldownAtaque = 0.5f; // Tiempo quebe esperar para volver a atacar
    private float contadorAtaque;
    private float tiempoSiguienteAtaque = 0f;
    [SerializeField] GameObject rangoAtaqueObject;
    public bool muerto;
    public int vida = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        savePoint = transform.position;
    }

    void Update()
    {

        if (!muerto)
        {
            // Control del temporizador de daño automático
            if (recibiendoDanio)
            {
                contadorDanio -= Time.deltaTime;
                if (contadorDanio <= 0f)
                {
                    DesactivaDanio();
                }
            }

            // Control del temporizador de la duración del ataque
            if (atacando)
            {
                contadorAtaque -= Time.deltaTime;
                if (contadorAtaque <= 0f)
                {
                    DesactivaAtaque();
                }
            }

            float horizontalInput = Input.GetAxis("Horizontal");

            // Si está recibiendo daño, bloqueamos temporalmente el movimiento normal
            if (!recibiendoDanio)
            {
                if (horizontalInput > 0)
                {
                    sprite.flipX = false;
                    
                    // Mueve el collider a la derecha (distancia positiva)
                    if (rangoAtaqueObject != null)
                    {
                        rangoAtaqueObject.transform.localPosition = new Vector3(0.2f, rangoAtaqueObject.transform.localPosition.y, rangoAtaqueObject.transform.localPosition.z);
                    }
                }
                else if (horizontalInput < 0)
                {
                    sprite.flipX = true;
                    
                    // Mueve el collider a la izquierda (distancia negativa)
                    if (rangoAtaqueObject != null)
                    {
                        rangoAtaqueObject.transform.localPosition = new Vector3(-0.2f, rangoAtaqueObject.transform.localPosition.y, rangoAtaqueObject.transform.localPosition.z);
                    }
                }

                if (horizontalInput != 0)
                {
                    anim.SetBool("Run", true);
                }
                else
                {
                    anim.SetBool("Run", false);
                }

                rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }

            anim.SetFloat("Y", rb.velocity.y);
            if (isGrounded)
            {
                anim.SetFloat("Y", 0);
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                anim.SetBool("Crouch", true);
            }
            else
            {
                anim.SetBool("Crouch", false);
            }

            if (transform.position.y < RespawnHeight)
            {
                transform.position = savePoint;
            }

            // Detectar entrada de ataque con Cooldown
            if (Input.GetKeyDown(KeyCode.Z) && !atacando && Time.time >= tiempoSiguienteAtaque)
            {
                Atacando();
                tiempoSiguienteAtaque = Time.time + cooldownAtaque;
            }

        }

            Collider2D col = GetComponent<Collider2D>();
            isGrounded = Physics2D.OverlapCircle(transform.position - transform.up * ((col.bounds.extents.y / transform.localScale.y - col.offset.y) * transform.localScale.y), 0.01f, groundLayer);
            
            anim.SetBool("recibeDanio", recibiendoDanio);
            anim.SetBool("Atacando", atacando);
            anim.SetBool("muerto", muerto);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "checkpoint")
        {
            collision.GetComponent<Animator>().SetBool("Active", true);
            savePoint = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("checkpoint"))
        {
            collision.GetComponent<Animator>().SetBool("Active", false);
            collision.enabled = false;
        }

        if (collision.GetComponent<PlatformEffector2D>())
        {
            collision.isTrigger = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.GetComponent<PlatformEffector2D>())
        {
            if (Input.GetKey(KeyCode.S))
            {
                collision.transform.GetComponent<Collider2D>().isTrigger = true;
            }
        }
    }

    public void RecibeDanio(Vector2 direccionEnemigo, int cantDamage)
    {
        if (!recibiendoDanio && !muerto)
        {
            recibiendoDanio = true;
            vida -= cantDamage;
            contadorDanio = duracionDanio; 

            // Comprobamos si la vida llegó a 0 para morir
            if (vida <= 0)
            {
                muerto = true;
                rb.velocity = Vector2.zero;
                // Aquí puedes añadir lógica extra de muerte si lo deseas (animación, desactivar sprite, etc.)
            }
            else
            {
                // Si aún tienes vida, aplicamos el rebote con normalidad
                rb.velocity = Vector2.zero; 
                float direccionX = transform.position.x > direccionEnemigo.x ? 1f : -1f;
                Vector2 fuerzaDeRebote = new Vector2(direccionX, 1.5f).normalized;
                rb.AddForce(fuerzaDeRebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
    }

    public void Atacando()
    {
        atacando = true;
        contadorAtaque = duracionAtaque; // Reinicia el tiempo que dura la animación activa
    }

    public void DesactivaAtaque()
    {
        atacando = false;
    }
}