using System;

public abstract class State : IDisposable
{
    protected readonly IReadonlyServiceProvider _allServices;
    protected readonly IGameStateMachine _stateMachine;

    public State(IReadonlyServiceProvider allServices, IGameStateMachine stateMachine)
    {
        _allServices = allServices;
        _stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Leave();
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    public virtual void Dispose() { }
}