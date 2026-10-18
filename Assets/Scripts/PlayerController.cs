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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        savePoint = transform.position;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput > 0)
        {
            sprite.flipX = false;
        }
        if (horizontalInput < 0)
        {
            sprite.flipX = true;
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

        // (Se eliminó la línea errónea con 'direccion' que estaba aquí)

        Collider2D col = GetComponent<Collider2D>();
        isGrounded = Physics2D.OverlapCircle(transform.position - transform.up * ((col.bounds.extents.y / transform.localScale.y - col.offset.y) * transform.localScale.y), 0.01f, groundLayer);
        anim.SetBool("recibeDanio", recibiendoDanio);
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
        // Lógica para el Checkpoint
        if (collision.CompareTag("checkpoint"))
        {
            collision.GetComponent<Animator>().SetBool("Active", false);
            collision.enabled = false;
        }

        // Lógica para el PlatformEffector2D
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

    public void RecibeDanio(Vector2 direccion, int cantDamage)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
            
            // CORREGIDO: AddForce en lugar de AddFroce
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
    }
}