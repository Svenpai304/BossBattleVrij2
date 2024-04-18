
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterStatus : MonoBehaviour
{
    public int MaxHealth;
    public float MaxDashTime;
    public float MouseSensitivity;
    public float StickLookDeadzone;

    [SerializeField] private GameObject keyboardLookReticle;
    [SerializeField] private GameObject gamepadLookArrow;
    public float gamepadArrowOffset;
    private GameObject lookObject;
    private Transform gamepadArrow;


    public float Health { get { return health; } set { health = value; } }
    public float Power { get { return power; } set { power = value; } }
    public float DashTime { get { return dashTime; } set { dashTime = value; } }
    public Vector2 LookDirection;

    [SerializeField, Unity.Collections.ReadOnly] private float health;
    [SerializeField, Unity.Collections.ReadOnly] private float power;
    [SerializeField, Unity.Collections.ReadOnly] private float dashTime;

    [SerializeField, Unity.Collections.ReadOnly] private bool isKeyboard = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && playerInput.devices.Contains(Keyboard.current))
        {
            isKeyboard = true;
            lookObject = Instantiate(keyboardLookReticle, Camera.main.transform);
            KeyboardLook(transform.position);
        }
        else
        {
            Debug.Log("Non-keyboard player added");
            lookObject = Instantiate(gamepadLookArrow, transform);
            gamepadArrow = lookObject.GetComponentsInChildren<Transform>()[1];
            GamepadLook(Vector2.right);
        }
    }

    private void Update()
    {
        if(isKeyboard)
        {
            LookDirection = (Vector2)(lookObject.transform.position - transform.position).normalized;
        }
        else
        {
            lookObject.transform.localPosition = (Vector3)(LookDirection * gamepadArrowOffset);
        }
    }

    public void OnLook(InputAction.CallbackContext c)
    {
        Debug.Log(c.ToString());
        Vector2 input = c.ReadValue<Vector2>();
        if (isKeyboard) { KeyboardLook(input); }
        else { GamepadLook(input); }
    }

    private void KeyboardLook(Vector2 pos)
    {
        lookObject.transform.localPosition += (Vector3)(pos * MouseSensitivity);
        if(Mathf.Abs(lookObject.transform.localPosition.x) > Camera.main.orthographicSize / Display.main.renderingHeight * Display.main.renderingWidth)
        {
            lookObject.transform.localPosition = new Vector3(
                Camera.main.orthographicSize / Display.main.renderingHeight * Display.main.renderingWidth * Mathf.Sign(lookObject.transform.localPosition.x),
                lookObject.transform.localPosition.y, 10);
        }
        if (Mathf.Abs(lookObject.transform.localPosition.y) > Camera.main.orthographicSize)
        {
            lookObject.transform.localPosition = new Vector3(lookObject.transform.localPosition.x, Camera.main.orthographicSize * Mathf.Sign(lookObject.transform.localPosition.y), 10);
        }
        LookDirection = (Vector2)(lookObject.transform.position - transform.position).normalized;
    }

    public void GamepadLook(Vector2 dir)
    {
        if (dir.sqrMagnitude < Mathf.Pow(StickLookDeadzone, 2)) { return; }

        dir = dir.normalized;
        lookObject.transform.localPosition = (Vector3)(dir * gamepadArrowOffset);
        Debug.Log(Vector2.SignedAngle(Vector2.zero, dir));
        gamepadArrow.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir));
        LookDirection = dir;
    }

}
