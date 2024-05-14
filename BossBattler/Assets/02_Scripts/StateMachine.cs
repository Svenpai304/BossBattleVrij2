using UnityEngine;
using System;
using System.Collections.Generic;
using static UnityEditor.VersionControl.Asset;
using Unity.VisualScripting;
/*
* StateMachine by Valentijn Muijrers
*/
/// <summary>
/// This is the stateMachine class, it manages transitions and states.
/// Note that we do not necessarily need to store the states, but for this example I added the stateCollection.
/// We pass the states along with the constructor.
/// We inject the transitions into the statemachine after the constructor.
/// In the SwitchState Method we load activeTransitions from the allTransitions List
/// Each frame we first check for transitions, then we update the currentState
/// I created two SwitchState implementates, one by Type and one by interface, not necessary but now you can choose which one you like better
/// </summary>
/// 

// Edit by Sven Wels:
// Tore out transition system completely because it was annoying me
public class StateMachine
{
    private IState currentState;
    private Dictionary<Type, IState> stateCollection = new Dictionary<Type, IState>();
    public StateMachine(params IState[] states)
    {
        for (int i = 0; i < states.Length; i++)
        {
            stateCollection.Add(states[i].GetType(), states[i]);
        }
    }
    public void OnFixedUpdate()
    {
        currentState?.OnUpdate();
    }
    public void SwitchState(System.Type newStateType)
    {
        SwitchState(stateCollection[newStateType]);
    }
    public void SwitchState(IState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }
    public void AddState(IState state)
    {
        stateCollection.Add(state.GetType(), state);
    }
}
/// <summary>
/// The interface for using States, we can also just use concrete States,
/// but this is more abstract, because we can now easily turn everything into a state by implementing the interface.
/// </summary>
public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}
/// <summary>
/// This is an abstract State, it also is generic so we can pass along a specific Actor which can then easily be accessed by specific States.
/// We make sure that the generic type is always a MonoBehaviour using the 'where' keyword.
/// </summary>
public abstract class State<T> : IState where T : MonoBehaviour
{
    public T Owner { get; protected set; }
    public State(T owner)
    {
        Owner = owner;
    }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnUpdate() { }
}
