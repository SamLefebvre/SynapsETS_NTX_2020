using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorX : MonoBehaviour
{
    public float speed = 300;
    public float percentSpeed = 1.0f;
    // Start is called before the first frame update
    void Update()
    {
        transform.Rotate(new Vector3(0, speed * percentSpeed, 0) * Time.deltaTime);
    }
}
