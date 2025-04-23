using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    [SerializeField] private float swayForce;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float sway = swayForce * Time.deltaTime;
        Vector3 randomSway = new Vector3(Random.Range(-sway, sway), 0, Random.Range(-sway, sway));
        rb.AddForce(randomSway, ForceMode.Force);
    }
}
