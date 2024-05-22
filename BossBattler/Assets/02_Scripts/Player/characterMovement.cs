using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

//This script handles moving the character on the X axis, both on the ground and in the air.

public class CharacterMovement : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] MovementLimiter moveLimit;
    [SerializeField] private ParticleSystem moveParticles;
    public CharacterStatus status;
    private Rigidbody2D body;
    private BoxCollider2D bc;
    private Animator animator;
    CharacterGround ground;

    [Header("Movement Stats")]
    [SerializeField, Range(0f, 20f)][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed")] public float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop after letting go")] public float maxDecceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed when in mid-air")] public float maxAirAcceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop in mid-air when no direction is used")] public float maxAirDeceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction when in mid-air")] public float maxAirTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction;
    [SerializeField, Range(0f, 1f)][Tooltip("Threshold for dropping through platforms")] private float holdingDownThreshold = 0.7f;
    [SerializeField] private LayerMask defaultExclude;
    [SerializeField] private LayerMask descendingExclude;

    [Header("Options")]
    [Tooltip("When false, the charcter will skip acceleration and deceleration and instantly move and stop")] public bool useAcceleration;
    public bool itsTheIntro = true;

    [Header("Calculations")]
    public float directionX;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;
    private float acceleration;
    private float deceleration;
    private float turnSpeed;

    [Header("Current State")]
    public bool onGround;
    public bool pressingKey;

    private void Awake()
    {
        //Find the character's Rigidbody and ground detection script
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ground = GetComponent<CharacterGround>();
        bc = GetComponent<BoxCollider2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
        //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.

        if (moveLimit.characterCanMove)
        {
            directionX = context.ReadValue<Vector2>().x;

            //Allows players to pass through platforms 
            if (context.ReadValue<Vector2>().y < -holdingDownThreshold) 
            { bc.excludeLayers = descendingExclude; }
            else { bc.excludeLayers = defaultExclude; }
        }
    }

    private void Update()
    {
        //Used to stop movement when the character is playing her death animation
        if (!moveLimit.characterCanMove && !itsTheIntro)
        {
            directionX = 0;
        }

        //Used to flip the character's sprite when she changes direction
        //Also tells us that we are currently pressing a direction button
        if (directionX != 0)
        {
            //transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);
            animator.SetBool("Running", true);
            pressingKey = true;
        }
        else
        {
            animator.SetBool("Running", false);
            pressingKey = false;
        }

        //Calculate's the character's desired velocity - which is the direction you are facing, multiplied by the character's maximum speed
        //Friction is not used in this game
        desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(maxSpeed - friction, 0f);

    }

    private void FixedUpdate()
    {
        //Fixed update runs in sync with Unity's physics engine

        //Get Kit's current ground status from her ground script
        onGround = ground.GetOnGround();

        //Get the Rigidbody's current velocity
        velocity = body.velocity;

        //Calculate movement, depending on whether "Instant Movement" has been checked
        if (useAcceleration)
        {
            runWithAcceleration();
        }
        else
        {
            if (onGround)
            {
                runWithoutAcceleration();
            }
            else
            {
                runWithAcceleration();
            }
        }

        //Handle move particle emission
        if(onGround && Mathf.Abs(velocity.x) >= maxSpeed - 1)
        {
            if (!moveParticles.isPlaying)
            {
                moveParticles.Play();
            }
        }
        else
        {
            moveParticles.Stop();
        }
    }

    private void runWithAcceleration()
    {
        //Set our acceleration, deceleration, and turn speed stats, based on whether we're on the ground on in the air

        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        deceleration = onGround ? maxDecceleration : maxAirDeceleration;
        turnSpeed = onGround ? maxTurnSpeed : maxAirTurnSpeed;

        if (pressingKey)
        {
            //If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around and so should use the turn speed stat.
            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                //If they match, it means we're simply running along and so should use the acceleration stat
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            //And if we're not pressing a direction at all, use the deceleration stat
            maxSpeedChange = deceleration * Time.deltaTime;
        }

        //Move our velocity towards the desired velocity, at the rate of the number calculated above
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange) * getSpeedMod();

        //Update the Rigidbody with this new velocity
        body.velocity = velocity;

    }

    public float getSpeedMod()
    {
        return status.GroundSpeedMult;
    }
    private void runWithoutAcceleration()
    {
        //If we're not using acceleration and deceleration, just send our desired velocity (direction * max speed) to the Rigidbody
        velocity.x = desiredVelocity.x * getSpeedMod();

        body.velocity = velocity;
    }
}