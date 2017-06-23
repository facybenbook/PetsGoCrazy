﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! Manages geneneral player movement
public class PlayerMovement : MonoBehaviour {

    [Header("Player Movement Settings")]
    public string horizontalAxis = "Horizontal"; //!< Reference to player's horizontal axis movement
    [Range(1, 10)]
    public int movementSpeed;       //!< Player movement velocity
  //public string verticalAxis;     //!< Reference to player's vertical axis movement
    

    [Header ("Player Jump Settings")]
    public string jumpButton = "Jump";  //!< Reference to player's jump movement button 
    [Range(10, 100)]
    public int jumpForce = 20;      //!< How high should the player jump
    private Collider2D isGrounded;  //!< Whether or not the player is grounded
    public Transform groundCheck;    //!< A position marking where to check if the player is grounded
    public LayerMask groundLayer;    //!< Reference to the ground layer

    private float maximumX;         //!< Maximum X position player can go
    private float maximumY;         //!< Maximum Y position player can go

    private bool jump;              //!< Controls if the player is jumping or not
    [HideInInspector]
    public bool isFacingRight = true;     //!< Controls if the player is moving to the right

    public SpriteRenderer sprite;

    void Start()
    {
        Vector3 cameraSize = Camera.main.ScreenToWorldPoint(new Vector3 (Camera.main.transform.position.x, 
                                                                            Camera.main.transform.position.y,
                                                                            Camera.main.transform.position.z));

        maximumX = -cameraSize.x - (sprite.size.x / 2);
        maximumY = -cameraSize.y - (sprite.size.y / 2);
    }

    void Update()
    {
        // Checks if player is grounded by drawing a circle around it and checking overlapping objects on ground layer
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundLayer);
        if (Input.GetButtonDown(jumpButton) && isGrounded)
            jump = true;
    }

    void FixedUpdate()
    {

        float horizontal = Input.GetAxis(horizontalAxis);
    
        Vector2 movement = new Vector2(horizontal, 0);
        gameObject.GetComponent<Rigidbody2D>().velocity = movement * movementSpeed;

        if (horizontal > 0 && !isFacingRight)
            FlipPlayer();
        else if (horizontal < 0 && isFacingRight)
            FlipPlayer();

        gameObject.GetComponent<Rigidbody2D>().position = new Vector3(Mathf.Clamp(gameObject.GetComponent<Rigidbody2D>().position.x,
                                                                       -maximumX, maximumX),

                                                                      Mathf.Clamp(gameObject.GetComponent<Rigidbody2D>().position.y,
                                                                      -maximumY, maximumY), 0);
        if (jump)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>().playSound("Jump", .5f);
            jump = false;
        }
    }

    private void FlipPlayer()
    {
        isFacingRight = !isFacingRight;
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Die()
    {
        gameObject.SetActive(false);
        StartCoroutine("Respawn");
    }

    IEnumerator Respawn ()
    {
        yield return new WaitForSeconds(2);
        if (gameObject.name.Contains("Dog"))
        {
            transform.position = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().dogSpawnPoint.position;
            ScoreManager.catPoints += 100;
            gameObject.SetActive(true);
        }
        else
        {
            transform.position = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().catSpawnPoint.position;
            ScoreManager.dogPoints += 100;
            gameObject.SetActive(true);
        }
    }
}
