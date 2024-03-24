using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateChecker : MonoBehaviour
{
    // Tiempo esperado entre pulsaciones de tecla
    public float expectedTime = 0.6316f;

    // Margen de error permitido
    public float errorMargin = 0.05f;

    // Tiempo en que se presionó la tecla por última vez
    private float lastKeyPressTime = 0f;

    // Tiempo en que se mostró el último tick
    private float lastTickTime = 0f;

   
    // Referencia al componente SpriteRenderer del GameObject para la palabra "Good"
    public SpriteRenderer goodSpriteRenderer;

    // Referencia al componente SpriteRenderer del GameObject para la palabra "Bad"
    public SpriteRenderer badSpriteRenderer;
   
    

    void Start()
    {
        
        // Inicia la invocación repetida de la función Tick
        InvokeRepeating("Tick", 0f, expectedTime);

        goodSpriteRenderer.enabled = false;
        badSpriteRenderer.enabled = false;
    }

    void Update()
    {
        // Detecta si se presiona la tecla de flecha hacia arriba
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Calcula el tiempo actual
            float currentTime = Time.time;

            // Verifica si el tiempo transcurrido desde la última pulsación está dentro del margen de error
            if (Mathf.Abs(currentTime - lastKeyPressTime - expectedTime) <= errorMargin)
            {
                Debug.Log("¡Bien!");

                StartCoroutine(ShowSprite(goodSpriteRenderer));

            }
            else
            {
                Debug.Log("¡Mal!");

                StartCoroutine(ShowSprite(badSpriteRenderer));
            }

            // Actualiza el tiempo de la última pulsación
            lastKeyPressTime = currentTime;
        }
    }

    // Función que se ejecutará en cada tick
    void Tick()
    {
        Debug.Log("Tick");        
    }

    IEnumerator ShowSprite(SpriteRenderer spriteRenderer)
    {
        // Muestra el sprite
        spriteRenderer.enabled = true;

        // Espera 0.2 segundos
        yield return new WaitForSeconds(0.2f);

        // Oculta el sprite
        spriteRenderer.enabled = false;
    }

    // Corutina para volver al color original después de un tiempo
    
}
