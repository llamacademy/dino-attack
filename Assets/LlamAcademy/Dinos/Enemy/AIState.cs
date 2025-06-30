using Unity.Behavior;

namespace LlamAcademy.Dinos.Enemy
{
    [BlackboardEnum]
    public enum AIState
    {
        Idle,
        Attacking,
        Patrol,
        CommandedMove
    }
}
