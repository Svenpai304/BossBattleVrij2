using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElectroGolem : MonoBehaviour
{
    public Rigidbody2D rb;
    [HideInInspector] public CharacterStatus currentTarget;

    public float ChargeYThreshold;

    [HideInInspector] public StateMachine stateMachine = new();

    private void Start()
    {
        SetupStateMachine();
    }

    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate();
    }

    private void SetupStateMachine()
    {
        stateMachine.AddState(new EGStates.Idle(this));
        stateMachine.AddState(new EGStates.StartCharge(this));
        stateMachine.AddState(new EGStates.Charge(this));

        stateMachine.SwitchState(new EGStates.Idle(this));
    }

    public bool CheckChargeAllowed()
    {
        foreach (CharacterStatus player in PlayerConnector.instance.players)
        {
            if (Mathf.Abs(player.transform.position.y - transform.position.y) < ChargeYThreshold)
            {
                currentTarget = player;
                Debug.Log("Charge target found");
                return true;
            }
        }
        Debug.Log("No charge target found");
        return false;
    }


}

namespace EGStates
{
    public class Idle : State<ElectroGolem>
    {
        public Idle(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {

        }

        public override void OnUpdate()
        {
            if(Owner.CheckChargeAllowed())
            {
                Owner.stateMachine.SwitchState(typeof(EGStates.StartCharge));
            }
        }

        public override void OnExit()
        {

        }
    }

    public class StartCharge : State<ElectroGolem>
    {
        // Step back a little, then charge in direction very quickly until hitting something or stopping

        private int Xdirection;
        private float speed = 2f;
        private float maxTime = 0.5f;
        private float time = 0f;
        public StartCharge(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            time = 0;
            Xdirection = -(int)Mathf.Sign((Owner.transform.position - Owner.currentTarget.transform.position).x);
            Owner.rb.velocity = Vector2.zero;
            Owner.rb.AddForce(new Vector2(Xdirection * 100, 0));
        }

        public override void OnUpdate()
        {
            Debug.Log(time);
            time += Time.fixedDeltaTime;
            if (time > maxTime)
            {
                Owner.stateMachine.SwitchState(typeof(EGStates.Charge));
            }
        }

        public override void OnExit()
        {
            Owner.rb.velocity = new Vector2(0, Owner.rb.velocity.y);
        }
    }

    public class Charge : State<ElectroGolem>
    {
        // Charge in direction very quickly until hitting something or reaching a max distance

        private float startX;
        private int Xdirection;
        private float acceleration = 30000f;
        private float maxSpeed = 40;
        private float maxDistance = 20;
        private float maxTime = 3;
        private float time;
        public Charge(ElectroGolem owner) : base(owner) { }

        public override void OnEnter()
        {
            time = 0;
            Xdirection = -(int)Mathf.Sign((Owner.transform.position - Owner.currentTarget.transform.position).x);
            startX = Owner.transform.position.x;
        }

        public override void OnUpdate()
        {
            time += Time.fixedDeltaTime;
            if (Owner.rb.velocity.x < maxSpeed)
            {
                Debug.Log(new Vector2(acceleration * Xdirection, 0));
                Owner.rb.AddForce(new Vector2(acceleration * Xdirection * Time.fixedDeltaTime, 0));
            }
            if (time >= maxTime || Mathf.Abs(Owner.transform.position.x - startX) >= maxDistance)
            {
                Owner.stateMachine.SwitchState(typeof(EGStates.Idle));
            }
        }

        public override void OnExit()
        {
            Owner.rb.velocity = Vector2.zero;
        }
    }
}
