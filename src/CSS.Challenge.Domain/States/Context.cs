using CSS.Challenge.Domain.Models;

namespace CSS.Challenge.Domain.States;

/// <summary>
/// This Context defines the interface to clients.
///
/// It maintains a ref to an instance of a State subclass, which
/// represents the current state of the Context.
/// 
/// The base State class declares methods that all derived State should
/// implement.
///
/// It also provides a backreference to the Context object,
/// associated with the State. 
/// 
/// </summary>
public class Context
{
    // A reference to the current state of the Context.
    public State LastState { get; private set; }
    private State _state = null;

    public Context(State state)
    {
        TransitionTo(state);
    }

    /// <summary>
    /// Transition to a target state.
    /// </summary>
    /// <param name="state">Target state.</param>
    public void TransitionTo(State state)
    {
        // this if statement acts as a blocker to prevent from going to unwanted states from state X.
        if (state.AllowedTransitionStates.Contains(state.GetType()) is false)
        {
            return;
        }

        LastState = state;
        _state = state;
        _state.SetContext(this);
            
    }

    public bool ExecuteState(OrderModel order)
    {
        this._state.TryHandleOrder(order);
        return true;
    }
}