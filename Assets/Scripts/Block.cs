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
        // adding random sway, like simulating wind, that increases with time
        float sway = swayForce * Time.deltaTime;
        Vector3 randomSway = new Vector3(Random.Range(-sway, sway), 0, Random.Range(-sway, sway));
        rb.AddForce(randomSway, ForceMode.Force);
    }
}
