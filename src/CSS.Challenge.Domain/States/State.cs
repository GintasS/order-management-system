using CSS.Challenge.Domain.Models;

namespace CSS.Challenge.Domain.States;

public abstract class State
{
    protected Context Context;
    public List<Type> AllowedTransitionStates = new();

    protected State(List<Type> allowedTransitionStates)
    {
        AllowedTransitionStates = allowedTransitionStates;
    }

    public void SetContext(Context context)
    {
        Context = context;
    }

    public abstract bool TryHandleOrder(OrderModel order);
}