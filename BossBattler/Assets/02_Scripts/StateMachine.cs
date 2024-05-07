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
public class StateMachine
{
    private List<Transition> allTransitions = new List<Transition>();
    private List<Transition> activeTransitions = new List<Transition>();
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
        foreach (Transition transition in activeTransitions)
        {
            if (transition.Evaluate() && transition.fromState == currentState)
            {
                SwitchState(transition.toState);
                return;
            }
        }
        currentState?.OnUpdate();
    }
    public void SwitchState(System.Type newStateType)
    {
        if (stateCollection.ContainsKey(newStateType))
        {
            SwitchState(stateCollection[newStateType]);
        }
        else
        {
            Debug.LogError($"State {newStateType.ToString()} not found in stateCollection");
        }
    }
    public void SwitchState(IState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        activeTransitions = allTransitions.FindAll(x => x.fromState == currentState || x.fromState == null);
        currentState?.OnEnter();
    }
    public void AddState(IState state)
    {
        stateCollection.Add(state.GetType(), state);
    }
    public void AddTransition(Transition transition)
    {
        allTransitions.Add(transition);
    }
}
/// <summary>
/// The Transition class allows to inject Transitions into the Statemachine.
/// In this way the state themselves don't know about transitions which keeps states oblivious to eachother.
/// We can pass a condition along with the transition, either in lamba-format or just a function which returns a boolean.
/// </summary>
public class Transition
{
    public IState fromState;
    public IState toState;
    public Func<bool> condition;
    public Transition(IState fromState, IState toState, Func<bool> condition)
    {
        this.fromState = fromState;
        this.toState = toState;
        this.condition = condition;
    }
    public bool Evaluate()
    {
        return condition();
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
