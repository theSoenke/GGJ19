public class TargetDestroyed
{
    public Target Target;

    public TargetDestroyed(Target target)
    {
        Target = target;
    }
}


public class RepairEvent
{
    public bool IsRepairing;
    public RepairEvent(bool isRepairing)
    {
        IsRepairing = isRepairing;
    }
}