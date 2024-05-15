using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopCamera : MonoBehaviour
{
    private List<CharacterStatus> characters = new List<CharacterStatus>();
    public Camera cam;
    private void Start()
    {
        characters = PlayerConnector.instance.players;
        PlayerConnector.instance.im.onPlayerJoined += OnPlayerJoin;
    }

    Vector3 BasePos = Vector3.zero;
    Vector3 RelativePos = Vector3.zero; //Use this for camera wiggle/natural movement
    Vector3 ShakePos = Vector3.zero; //Use this for calculated screen shakes
    private void Update()
    {
        if(characters.Count < 1 || CinematicOverride) { return; }

        //Calculate NewPos
        Vector2 newPos = new();
        foreach (CharacterStatus character in characters)
        {
            newPos += (Vector2)character.transform.position;
        }
        newPos /= characters.Count;

        setCamPos(newPos);
        RandomMovement();

        //Calculate RelativePos
    }


    float WaveMoveTimer;
    Vector3 WaveWantPos;
    private void RandomMovement()
    {
        float deviation = 0.1f;
        WaveMoveTimer -= Time.deltaTime;
        if (WaveMoveTimer < 0f)
        {
            WaveMoveTimer = Random.Range(0.2f, 0.6f);
            WaveWantPos = new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation)) - RelativePos;
        }
        float Speed = 0.2f;
        float ang1 = Vector3.SignedAngle(getLookVector(), WaveWantPos, Vector3.forward);
        if (ang1 > 0)
        {
            Rotator.transform.Rotate(new Vector3(0, 0, 1), 270f * Time.deltaTime);
        }
        else
        {
            Rotator.transform.Rotate(new Vector3(0, 0, 1), -270f * Time.deltaTime);
        }
        RelativePos += getLookVector() * Speed * Time.deltaTime;
        setCamPos();
    }
    protected Vector3 getLookVector()
    {
        Quaternion rotref = Rotator.rotation;
        float rot = Mathf.Deg2Rad * rotref.eulerAngles.z;
        float dxf = Mathf.Cos(rot);
        float dyf = Mathf.Sin(rot);
        return new Vector3(dxf, dyf, 0);
    }

    private bool isShaking;
    private float ShakingDuration;
    private float ShakePower;
    public bool CinematicOverride;
    public void ShakeCamera(float Pow, float Dur)
    {
        //Use this to shake a camera! Power level decreases gradually. Duration is a buffer in which Power level does not decrease.
        ShakePower = Mathf.Max(ShakePower, Pow);
        ShakingDuration = Mathf.Max(ShakingDuration, Dur);
        if (!isShaking) StartCoroutine(CameraShake());
        
    }

    public Transform Rotator;

    private int ShakeDir = 1;
    IEnumerator CameraShake()
    {
        isShaking = true;
        while (ShakePower > 0)
        {
            if (ShakingDuration > 0) ShakingDuration -= Time.deltaTime;
            else ShakePower -= Time.deltaTime * 0.5f;
            ShakeInstance();
            yield return null;
        }
        isShaking = false;
        yield break;
    }

    private void ShakeInstance()
    {
        ShakeDir *= -1;
        ShakePos = new Vector3(Random.Range(-ShakePower, ShakePower), ShakeDir * ShakePower);
        setCamPos();
    }
    public void setCamPos()
    {
        transform.position = BasePos + RelativePos + ShakePos;
    }
    public void setCamPos(Vector3 newPos)
    {
        newPos = new Vector3(newPos.x, newPos.y, -10);
        BasePos = newPos;
        transform.position = newPos + RelativePos + ShakePos;
    }
    public void addCamPos(Vector3 pos)
    {
        BasePos += pos;
        setCamPos(BasePos);
    }

    private void OnPlayerJoin(PlayerInput input)
    {
        characters.Add(input.GetComponent<CharacterStatus>());
    }
}
