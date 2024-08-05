using System;

public interface IGameStateMachine
{
    Type CurrentStateType { get; }
    bool IsDebugOn { get; set; }

    void AddState(State state);
    void Enter<T>() where T : State;
    void LeaveCurrentState();
    void RemoveState<T>() where T : State;
    void UpdateStateLogic();
    void UpdateStatePhisics();
}
