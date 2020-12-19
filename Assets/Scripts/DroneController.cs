using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 input;
    public float acceleration = 1;
    public float maxSpeed = 1;

    private bool isAcceleratingX;
    private bool isAcceleratingZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.sleepThreshold = 1.5f;
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        input = new Vector3(movementVector.x, 0, movementVector.y);

        isAcceleratingX = Mathf.Abs(movementVector.x) > 0;
        isAcceleratingZ = Mathf.Abs(movementVector.y) > 0;
        //Debug.LogFormat("OnMove: {0}", movementVector);
    }

    void FixedUpdate()
    {
        Vector3 force = Vector3.zero;
        Vector3 normalizedVelocity = rb.velocity.normalized;
        float speed = rb.velocity.magnitude;
        

        if (!isAcceleratingX && !isAcceleratingZ && rb.velocity.magnitude < 0.05f)
            rb.velocity = Vector3.zero;
        else
        {
            if (isAcceleratingX)
                force.x = input.x * acceleration;
            else if (speed > 0)
                force.x = -normalizedVelocity.x * acceleration * 2;

            if (isAcceleratingZ)
                force.z = input.z * acceleration;
            else if (speed > 0)
                force.z = -normalizedVelocity.z * acceleration * 2;

            rb.AddForce(force);
            LimitMaxSpeed();
        }


        //if (isAcceleratingX)
        //{
        //    rb.AddForce(input * acceleration);
        //    LimitMaxSpeed();
        //}
        //else
        //{
        //    rb.AddForce(acceleration * 2 * -rb.velocity.normalized);
        //}

        //Debug.LogFormat("Fixed Update: {0}", rb.IsSleeping());
    }

    private void LimitMaxSpeed()
    {
        float currentSpeed = rb.velocity.magnitude;
        float coeff = maxSpeed / currentSpeed;
        if (coeff < 1)
            rb.velocity = rb.velocity * coeff;
    }
}
