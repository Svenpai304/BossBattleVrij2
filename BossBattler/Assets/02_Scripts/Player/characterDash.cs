using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterDash : MonoBehaviour
{
    characterJump jump;
    characterStatus status;
    Rigidbody2D rb;

    public float dashForce;
    public float dashStartForce;
    public float gravMultiplier;

    public bool dashing;


    private void Start()
    {
        jump = GetComponent<characterJump>();
        status = GetComponent<characterStatus>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (dashing)
        {
            rb.AddForce(status.LookDirection * dashForce);
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
        if(status.DashTime == status.MaxDashTime)
        {
            rb.AddForce(status.LookDirection * dashStartForce);
        }
        Physics2D.gravity = new Vector2(0, 9.81f * gravMultiplier);

    }

    private void EndDash()
    {
        dashing = false;
        jump.enabled = true;
        Physics2D.gravity = new Vector2(0, 9.81f);

    }
}
