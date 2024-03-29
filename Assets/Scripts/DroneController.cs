﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float maxSpeed = 1;
    [SerializeField] private float maxDeflection = 10;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float maxPitch = 2f;

    private Rigidbody rb;
    private Vector3 input;
    private bool captureInput = true;

    private bool isAcceleratingX;
    private bool isAcceleratingZ;
    private float currentBankingVelocityX = 0;
    private float currentBankingVelocityZ = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue movementValue)
    {
        if (!captureInput)
        {
            input = Vector3.zero;
            return;
        }

        Vector2 movementVector = movementValue.Get<Vector2>();
        movementVector.Normalize();
        input = new Vector3(movementVector.x, 0, movementVector.y);

        //Debug.LogFormat("OnMove: {0}", movementVector);
    }

    void FixedUpdate()
    {
        Vector3 force = Vector3.zero;
        Vector3 normalizedVelocity = rb.velocity.normalized;
        float speed = rb.velocity.magnitude;

        isAcceleratingX = Mathf.Abs(input.x) > 0;
        isAcceleratingZ = Mathf.Abs(input.z) > 0;

        if (!isAcceleratingX && !isAcceleratingZ && rb.velocity.magnitude < 0.05f)
            rb.velocity = Vector3.zero;
        else
        {
            if (isAcceleratingX)
                force.x = input.x * acceleration;
            else if (speed > 0)
                force.x = -normalizedVelocity.x * acceleration;

            if (isAcceleratingZ)
                force.z = input.z * acceleration;
            else if (speed > 0)
                force.z = -normalizedVelocity.z * acceleration;

            rb.AddForce(force);
            LimitMaxSpeed();
        }

        speed = rb.velocity.magnitude;
        AdjustAudio(speed, maxSpeed);
        AdjustPitchAndBank(input, rb.velocity);

        //Debug.LogFormat("Fixed Update: {0}", rb.IsSleeping());
    }

    public void AutoCutOff()
    {
        StartCoroutine(AutoCutOffCoroutine());
    }

    private void LimitMaxSpeed()
    {
        float currentSpeed = rb.velocity.magnitude;
        float coeff = maxSpeed / currentSpeed;
        if (coeff < 1)
            rb.velocity = rb.velocity * coeff;
    }

    private void AdjustAudio(float currentSpeed, float maxSpeed)
    {
        float pitch = Mathf.Lerp(1, maxPitch, currentSpeed / maxSpeed);
        audioSource.pitch = pitch;
    }

    private void AdjustPitchAndBank(Vector3 input, Vector3 currentVelocity)
    {
        Vector3 rotation = transform.eulerAngles;
        if (isAcceleratingX)
            rotation.x = Mathf.SmoothDampAngle(rotation.x, input.x > 0 ? maxDeflection : -maxDeflection, ref currentBankingVelocityX, 0.2f);
        else if (Mathf.Abs(currentVelocity.x) > 0.1f)
            rotation.x = Mathf.SmoothDampAngle(rotation.x, currentVelocity.x > 0 ? -maxDeflection : maxDeflection, ref currentBankingVelocityX, 0.1f);
        else
            rotation.x = Mathf.SmoothDampAngle(rotation.x, 0, ref currentBankingVelocityX, 0.1f);

        if (isAcceleratingZ)
            rotation.z = Mathf.SmoothDampAngle(rotation.z, input.z > 0 ? maxDeflection : -maxDeflection, ref currentBankingVelocityZ, 0.2f);
        else if (Mathf.Abs(currentVelocity.z) > 0.1f)
            rotation.z = Mathf.SmoothDampAngle(rotation.z, currentVelocity.z > 0 ? -maxDeflection : maxDeflection, ref currentBankingVelocityZ, 0.1f);
        else
            rotation.z = Mathf.SmoothDampAngle(rotation.z, 0, ref currentBankingVelocityZ, 0.1f);

        transform.eulerAngles = rotation;
    }

    private IEnumerator AutoCutOffCoroutine()
    {
        Vector3 point1 = new Vector3(-1.7f, transform.position.y, 0);
        Vector3 point2 = new Vector3(1.25f, transform.position.y, 0);
        float distance = Vector3.Distance(transform.position, point1);

        while (distance > 0.2f)
        {
            input = point1 - transform.position;
            input.Normalize();
            yield return null;
            distance = Vector3.Distance(transform.position, point1);
        }

        input = Vector3.zero;
        yield return new WaitForSeconds(1);
        distance = Vector3.Distance(transform.position, point2);

        while (distance > 0.2f)
        {
            input = point2 - transform.position;
            input.Normalize();
            yield return null;
            distance = Vector3.Distance(transform.position, point2);
        }
        input = Vector3.zero;
    }

    public void BlockInput(bool block)
    {
        captureInput = !block;
    }
}
