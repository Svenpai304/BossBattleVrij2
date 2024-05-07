using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroGolem : MonoBehaviour
{
    private StateMachine stateMachine = new();

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
        stateMachine.AddTransition(new Transition(new EGStates.Idle(this), new EGStates.Charge(this), CheckChargeAllowed));
    }

    private bool CheckChargeAllowed()
    {
        return true;
    }
}

namespace EGStates
{
    public class Idle : State<ElectroGolem>
    {
        public Idle(ElectroGolem owner) : base(owner)
        {

        }

        public override void OnEnter()
        {

        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {

        }
    }

    public class Charge : State<ElectroGolem>
    {
        public Charge(ElectroGolem owner) : base(owner)
        {

        }

        public override void OnEnter()
        {

        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {

        }
    }
}
