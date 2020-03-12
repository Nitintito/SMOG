using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*Movement*/
    private float moveSpeed = 7.0f;                // Ground move speed
    private float runAcceleration = 14.0f;         // Ground accel
    private float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    private float airAcceleration = 2.0f;          // Air accel
    private float airDecceleration = 2.0f;         // Deacceleration experienced when ooposite strafing
    private float airControl = 0.3f;               // How precise air control is
    private float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
    private float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    private float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    private bool holdJumpToBhop = false;           // When enabled allows player to just hold jump button to keep on bhopping perfectly. Beware: smells like casual.

    /*Camera*/
    public Transform playerView;     // Camera
    public float playerViewYOffset = 0.6f; // The height at which the camera is bound to
    public float xMouseSensitivity = 30.0f;
    public float yMouseSensitivity = 30.0f;
    private float rotX = 0.0f; // Camera rotations
    private float rotY = 0.0f; // Camera rotations

    private float gravity = -9.81f;
    private CharacterController _controller;

    private Vector3 movedirectionNorm = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    /*Jumping*/
    bool wishJump = false;

    struct Cmd
    {
        public float moveForward;
        public float moveSideways;
    }

    private Cmd _cmd;

    void Start()
    {
        SetMoveInputs();

        _controller = GetComponent<CharacterController>();

    }

    void Update()
    {
        RotateCamera();
        //sist i update
        _controller.Move(playerVelocity * Time.deltaTime);
    }

    void SetMoveInputs()
    {
        _cmd.moveForward = Input.GetAxisRaw("Vertical");
        _cmd.moveSideways = Input.GetAxisRaw("Horizontal");
    }

    void QueueJump()
    {
        if (Input.GetButtonDown("Jump") && !wishJump)
        {
            wishJump = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            wishJump = false;
        }
    }

    void Friction()
    {

    }
    /**
     * Called every frame when the engine detects that the player is on the ground
     */
    void GroundMove()
    {
        Vector3 wishDir;

        SetMoveInputs();

        wishDir = new Vector3(_cmd.moveForward, 0, _cmd.moveSideways);
        wishDir = transform.TransformDirection(wishDir);
        wishDir.Normalize();
        movedirectionNorm = wishDir;

        var wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        Accelerate(wishDir, wishSpeed, runAcceleration);

        playerVelocity.y -= gravity * Time.deltaTime;
    }

    void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
    {
        float addSpeed;
        float accelSpeed;
        float currentSpeed;

        currentSpeed = Vector3.Dot(playerVelocity, wishDir);
        addSpeed = wishSpeed - currentSpeed;
        if (addSpeed <= 0)
        {
            return;
        }
        accelSpeed = accel * Time.deltaTime * wishSpeed;
        if (accelSpeed > addSpeed)
        {
            accelSpeed = addSpeed;
        }

        playerVelocity.x += accelSpeed * wishDir.x;
        playerVelocity.y += accelSpeed * wishDir.z;
    }

    void RotateCamera()
    {
        /* Camera rotation stuff, mouse controls this shit */
        rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        if (rotX < -90)
        {
            rotX = -90;
        }
        else if (rotX > 90)
        {
            rotX = 90;
        }

        this.transform.rotation = Quaternion.Euler(0, rotY, 0); // rotates collider
        playerView.rotation = Quaternion.Euler(rotX, rotY, 0); // rotates camera

        //Need to move the camera after the player has been moved because otherwise the camera will clip the player if going fast enough and will always be 1 frame behind.
        // Set the camera's position to the transform
        playerView.position = new Vector3(
           transform.position.x,
           transform.position.y + playerViewYOffset,
           transform.position.z);
    }
    /**
     * Called every frame when the engine detects that the player is in the air
     */
    void AirMovement()
    {
        Vector3 wishDir;
        float wishVel = airAcceleration;
        float accel;

        SetMoveInputs();
        //om kefft byta plats på cmds
        wishDir = new Vector3(_cmd.moveSideways, 0, _cmd.moveForward);
        wishDir = transform.TransformDirection(wishDir);

        float wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        wishDir.Normalize();
        movedirectionNorm = wishDir;

        float wishSpeed2 = wishSpeed;
        if (Vector3.Dot(playerVelocity, wishDir) < 0)
        {
            accel = airDecceleration;
        }
        else
        {
            accel = airAcceleration;
        }

        if (_cmd.moveForward == 0 && _cmd.moveSideways != 0)
        {
            if (wishSpeed > sideStrafeSpeed)
            {
                wishSpeed = sideStrafeSpeed;
            }
            accel = sideStrafeSpeed;
        }

        Accelerate(wishDir, wishSpeed, accel);

        if (airControl > 0)
        {
            AirControl(wishDir, wishSpeed2);
        }

        playerVelocity.y -= gravity * Time.deltaTime;
    }

    /**
     * Air control occurs when the player is in the air, it allows
     * players to move side to side much faster rather than being
     * 'sluggish' when it comes to cornering.
     */
    void AirControl(Vector3 wishDir, float wishSpeed)
    {
        float zSpeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if (Mathf.Abs(_cmd.moveForward) < 0.001 || Mathf.Abs(wishSpeed) < 0.001)
        {
            return;
        }

        zSpeed = playerVelocity.y;
        playerVelocity.y = 0;
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishDir);
        k = 32f;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishDir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishDir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishDir.z * k;

            playerVelocity.Normalize();
            movedirectionNorm = playerVelocity;
        }
        playerVelocity.x *= speed;
        playerVelocity.y = zSpeed;
        playerVelocity.z *= speed;
    }
}
