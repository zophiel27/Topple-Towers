using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform tower; 
    [SerializeField] private float offsetY;    
    [SerializeField] private float smoothSpeed = 2f;

    void LateUpdate()
    {
        float highestY = 0f;

        foreach (Transform child in tower)
        {
            if (child != null && child.position.y > highestY)
            {
                highestY = child.position.y;
            }
        }

        Vector3 targetPosition = new Vector3(transform.position.x, highestY + offsetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
