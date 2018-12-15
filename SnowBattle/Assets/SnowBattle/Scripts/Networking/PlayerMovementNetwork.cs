using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementNetwork : NetworkBehaviour
{ 
    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Animator anim;                      // Reference to the animator component.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    public float speed = 6f;            // The speed that the player will move at.
    public Joystick joystickleft;       // Reference the joytickleft
    public Joystick joystickright;      // Reference joystickright


    void Awake()
    {
        // Set up references.
        anim = GetComponent<Animator>();
        //set up rigidbody
        playerRigidbody = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        float h = joystickleft.Horizontal;

        float v = joystickleft.Vertical;

        float jh = joystickright.Horizontal;

        float jv = joystickright.Vertical;

        if (GetComponent<PlayerSetupNetwork>().myTeam.name == "Down")
        {
            // Move the player around the scene.
            Move(h, v);
            // Turn the player to face the mouse cursor.
            Turning(jh,jv);
        }
        else if (GetComponent<PlayerSetupNetwork>().myTeam.name == "Up")
        {
            // Move the player around the scene.
            Move(-h, -v);
            // Turn the player to face the mouse cursor.
            Turning(-jh,-jv);
        }

        // Animate the player.
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning(float jh,float jv)
    {
        
        Vector3 moveVector = (Vector3.right * jh + Vector3.forward * jv);
        if (moveVector != Vector3.zero)
        {
            playerRigidbody.MoveRotation(Quaternion.LookRotation(moveVector));
        }
    }

    void Animating(float h, float v)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        bool walking = h != 0f || v != 0f;

        // Tell the animator whether or not the player is walking.
        if(anim!=null)
        anim.SetBool("Run", walking);
    }
}
