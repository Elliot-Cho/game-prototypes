using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;

    private Animator anim;  // Player animator
    private bool moving;    // To check if player is currently moving
    private Vector2 prevMove;   // Keep track of previous movement
    private Rigidbody2D body;  // Player's rigidbody

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

        // Set axis input to variables
        int xMove = (int)Input.GetAxisRaw("Horizontal");
        int yMove = (int)Input.GetAxisRaw("Vertical");

        // Player initially not moving
        moving = false;

        // Move on axis on input
        if (Mathf.Abs(xMove) > 0f)
        {
            //transform.Translate(new Vector3(xMove * moveSpeed * Time.deltaTime, 0f, 0f));
            body.velocity = new Vector2(xMove * moveSpeed, body.velocity.y);
            moving = true;
            prevMove = new Vector2(xMove, 0f);
        }
        if (Mathf.Abs(yMove) > 0f)
        {
            //transform.Translate(new Vector3(0f, yMove * moveSpeed * Time.deltaTime, 0f));
            body.velocity = new Vector2(body.velocity.x, yMove * moveSpeed);
            moving = true;
            prevMove = new Vector2(0f, yMove);
        }

        // Stop movement
        if (xMove == 0f)
        {
            body.velocity = new Vector2(0f, body.velocity.y);
        }
        if (yMove == 0f)
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
        }

        // Update animator parameters
        anim.SetFloat("MoveX", xMove);
        anim.SetFloat("MoveY", yMove);
        anim.SetBool("Moving", moving);
        anim.SetFloat("PrevMoveX", prevMove.x);
        anim.SetFloat("PrevMoveY", prevMove.y);
    }
}

