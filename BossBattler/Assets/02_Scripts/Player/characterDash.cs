using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDash : MonoBehaviour
{
    CharacterJump jump;
    CharacterStatus status;
    CharacterLook look;
    Rigidbody2D rb;

    public float dashForce;
    public float dashStartForce;
    public float gravMultiplier;

    public bool dashing;


    private void Start()
    {
        jump = GetComponent<CharacterJump>();
        status = GetComponent<CharacterStatus>();
        look = GetComponent<CharacterLook>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (dashing)
        {
            rb.AddForce(look.LookDirection * dashForce);
            status.DashTime = Mathf.Clamp(status.DashTime - Time.fixedDeltaTime, 0, status.MaxDashTime);
            if(status.DashTime == 0)
            {
                EndDash();
            }
        }
        else if(jump.onGround)
        {
            status.DashTime = status.MaxDashTime;
        }
    }

    public void OnDashButton(InputAction.CallbackContext c)
    {
        if (c.started && status.DashTime > 0) { StartDash(); }
        if (c.canceled && dashing) { EndDash(); }
    }

    private void StartDash()
    {
        dashing = true;
        jump.enabled = false;
        if (status.DashTime == status.MaxDashTime)
        {
            rb.AddForce(look.LookDirection * dashStartForce * status.GroundSpeedMult);
        }
        rb.gravityScale *= gravMultiplier;

    }

    private void EndDash()
    {
        dashing = false;
        jump.enabled = true;
        rb.gravityScale /= gravMultiplier;

    }
}
