using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> CurrentState;
    protected bool IsTransitioningState = false;
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
        CurrentState.EnterState();
    }
    protected virtual void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();
        if (nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();

        }
        else
        {
            TransitionToState(nextStateKey);
        }
    }
    protected virtual void FixedUpdate(){}
    protected virtual void LateUpdate(){}

    public void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }

}