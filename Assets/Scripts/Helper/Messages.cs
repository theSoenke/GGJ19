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
    public EnemyController enemyController;

    public EnemyDeadMessage(EnemyController controller)
    {
        enemyController = controller;
    }
}

public class PartyMessage
{
    public EnemyController enemyController;

    public PartyMessage(EnemyController controller)
    {
        enemyController = controller;
    }
}