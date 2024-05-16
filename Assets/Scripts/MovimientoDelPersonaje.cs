using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoDelPersonaje : MonoBehaviour
{
    public float velocidadDeMovimiento = 5f;
    private float desplazamientoX, desplazamientoY;
    private Vector2 direccionDeMovimiento;
    private Rigidbody2D rbPersonaje;
    // Start is called before the first frame update
    void Start()
    {
        rbPersonaje = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CapturaDePulsaciones();
    }
    void FixedUpdate()
    {
        MovimientoDelPersonaje();
    }
    void CapturaDePulsaciones()
    {
        desplazamientoX = Input.GetAxixRaw("Horizontal");
        desplazamientoY = Input.GetAxixRaw("Vertical");
        direccionDeMovimiento = new Vector2(desplazamientoX, desplazamientoY)
    }
    void MovimientoDelPersonaje()
    {
        rbPersonaje.Velocity = new Vector2(direccionDeMovimiento.x*velocidadDeMovimiento, direccionDeMovimiento.y*velocidadDeMovimiento);
    }
}
