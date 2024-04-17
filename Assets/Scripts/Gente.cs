using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gente: MonoBehaviour
{
    public float speedPublic =2f;
    public Vector2 direction;
    public Vector2 movement;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
   void Update()
    {
        GetMoveInput(); // Llamamos al método GetMoveInput para actualizar la dirección
        movement = direction.normalized * speedPublic * Time.deltaTime;
        transform.Translate(movement);
    }

    void GetMoveInput()
    {
        direction = Vector2.zero; // Reiniciamos la dirección en cada frame

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }

        // Normalizamos la dirección si el jugador está moviéndose en diagonal
        if (direction.magnitude > 1)
        {
            direction = direction.normalized;
        }
    }
}
