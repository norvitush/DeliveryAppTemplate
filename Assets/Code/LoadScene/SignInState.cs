using System;

public class SignInState : State
{
    public SignInState(IReadonlyServiceProvider allServices, IGameStateMachine stateMachine) : base(allServices, stateMachine)
    {
    }

    public override void Enter()
    {
        throw new NotImplementedException();
    }
   

    public override void Leave()
    {
        throw new NotImplementedException();
    }
}
