using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public Button reiniciarButton;
    public Button menuButton;
    private bool gameOverActivo = false;
    public GameObject FinPanel; // Arrastra aquí tu "FinPanel" desde Unity
    private bool juegoTerminado = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (reiniciarButton != null)
            reiniciarButton.onClick.AddListener(ReiniciarEscena);
        if (menuButton != null)
            menuButton.onClick.AddListener(IrAlMenu);
    }

    void Update()
    {
        if (gameOverActivo)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReiniciarEscena();
            }
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
            {
                IrAlMenu();
            }
        }
    }

    public void ActivarGameOver() 
    {
        if (gameOverActivo) return;
        gameOverActivo = true;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // ¡Esto es lo que congela la pantalla y detiene todo!
        Time.timeScale = 0f; 
    }

    public void ReiniciarEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void ActivarVictoria()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (FinPanel != null)
        {
            FinPanel.SetActive(true);
        }

        // Congela la pantalla tal como en el Game Over, pero sin alterar la vida
        Time.timeScale = 0f; 
    }
}