using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pismeno : MonoBehaviour
{
    public event Action<Pismeno> OnPismenoDropped;

    private readonly string droneTag = "Player";
    private Rigidbody rb;
    private bool isDropped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HitSomething(collision.collider);
        //Debug.LogFormat("Collision enter");
    }

    private void OnTriggerEnter(Collider other)
    {
        HitSomething(other);
        //Debug.LogFormat("Trigger enter");
    }

    private void HitSomething(Collider other)
    {
        if (!isDropped && other.CompareTag(droneTag))
        {
            Drop();
        }
    }

    public void Drop()
    {
        rb.useGravity = true;
        isDropped = true;
        OnPismenoDropped?.Invoke(this);
    }
}
