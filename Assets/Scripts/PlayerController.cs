using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;

    private AudioClip clipHitWall;
    private AudioClip clipHitPickup;
    private AudioSource rollingSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);

        clipHitWall = Resources.Load<AudioClip>("Sounds/wall");
        clipHitPickup = Resources.Load<AudioClip>("Sounds/pickup");

        rollingSound = gameObject.GetComponents<AudioSource>()[1];
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = String.Format("Count: {0}", count.ToString());
        if (count >= 9)
        {
            winTextObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);



        rollingSound.volume = 0;
        if (IsGrounded())
        {
            // Map Range
            var volume = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 12, rb.velocity.magnitude));
            var pitch = Mathf.Lerp(0.3f, 1, Mathf.InverseLerp(0, 12, rb.velocity.magnitude));
            rollingSound.volume = volume;
            rollingSound.pitch = pitch;
        }

    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.51f);
    }
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(new Vector3(0.0f, 25.0f, 0.0f), ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();

            gameObject.GetComponent<AudioSource>().PlayOneShot(clipHitPickup);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(clipHitWall);
        }

        if (collision.collider.CompareTag("plane"))
        {
            Debug.Log("Game Over");
        }
    }

}