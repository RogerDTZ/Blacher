using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankMovement : NetworkBehaviour
{

    public float m_Movespeed = 10f;

    public Rigidbody2D m_Rigidbody;

    private float m_Movement_h;
    private float m_Movement_v;

    // Use this for initialization
    void Start()
    {
    }

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            GetInputMovement();
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Move();
        }
    }

    private void GetInputMovement()
    {
        m_Movement_h = Input.GetAxis("Horizontal");
        m_Movement_v = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        Vector2 movement;
        movement = new Vector2(m_Movement_h, m_Movement_v);
        movement = movement.normalized * m_Movespeed;
        m_Rigidbody.AddForce(movement);
    }

}
