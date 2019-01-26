public class TargetDestroyedMessage
{
    public Target Target;

    public TargetDestroyedMessage(Target target)
    {
        Target = target;
    }
}


public class RepairEventMessage
{
    public bool IsRepairing;
    public RepairEventMessage(bool isRepairing)
    {
        IsRepairing = isRepairing;
    }
}

public class EnemyDeadMessage
{
    public EnemyController EnemyController;

    public EnemyDeadMessage(EnemyController controller)
    {
        EnemyController = controller;
    }
}