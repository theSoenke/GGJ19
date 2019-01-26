using UnityEngine;


public class Fence : Target, ITakeDamage
{
    public FenceState State;
    public float Health;
    public float RepairTime = 1.0f;

    public Icon3d RepairIcon;
    public GameObject FenceOK;
    public GameObject FenceBroken;

    private float _repairStartedTime;

    public void Start()
    {
        RepairIcon.OnIconClicked += Repair;
    }

    public bool TakeDamage(float value)
    {
        Health -= value;
        if (Health <= 0 && State != FenceState.Broken)
        {
            State = FenceState.Broken;
            MessageBus.Push(new TargetDestroyed(this));
            return true;
        }
        return false;
    }

    public DamageType GetDamageType()
    {
        return DamageType.Object;
    }

    public void Update()
    {
        UpdateRepair();
        UpdateGraphics();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (State == FenceState.Broken)
        {
            RepairIcon.gameObject.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        RepairIcon.gameObject.SetActive(false);

        if (State == FenceState.Repairing)
        {
            State = FenceState.Broken;
        }
    }


    public void Repair()
    {
        _repairStartedTime = Time.time;
        State = FenceState.Repairing;
    }


    private void UpdateRepair()
    {
        if (State == FenceState.Repairing && Time.time > _repairStartedTime + RepairTime)
        {
            State = FenceState.Ok;
        }
    }

    private void UpdateGraphics()
    {
        if (State == FenceState.Ok)
        {
            FenceOK.SetActive(true);
            FenceBroken.SetActive(false);
        }
        else
        {
            FenceOK.SetActive(false);
            FenceBroken.SetActive(true);
        }
    }


    public enum FenceState
    {
        Ok,
        Broken,
        Repairing
    }
}