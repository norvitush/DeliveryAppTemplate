using System.Collections.Generic;
using System;

public class BaseStateMachine : IGameStateMachine, IDisposable
{
    public bool IsDebugOn { get; set; }

    private State _state;
    private readonly Dictionary<Type, State> _allStates = new Dictionary<Type, State>();

    public Type CurrentStateType => _state?.GetType();

    public void AddState(State state)
    {
        _allStates.Add(state.GetType(), state);
    }
    public void RemoveState<T>() where T : State
    {
        if (_allStates.ContainsKey(typeof(T)))
            _allStates.Remove(typeof(T));
    }

    public void Enter<T>() where T : State
    {
        if (_allStates.TryGetValue(typeof(T), out var state))
        {
            if(IsDebugOn)
            {
                Helpers.ColorDebugLog($"{(CurrentStateType == null ? "INIT" : CurrentStateType.ToString())} -> {typeof(T)}", UnityEngine.Color.green);
            }

            _state = state;
            _state.Enter();
        }
    }

    public void LeaveCurrentState()
    {
        _state.Leave();
        _state = null;
    }

    public void UpdateStateLogic()
    {
        if (_state == null) return;

        _state.Update();
    }
    public void UpdateStatePhisics()
    {
        if (_state == null) return;

        _state.FixedUpdate();
    }

    public virtual void Dispose()
    {
        _state = null;
        foreach (IDisposable state in _allStates.Values)
        {
            state.Dispose();
        }
    }
}


