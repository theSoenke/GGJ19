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
    private bool _playerInRange;
    private bool _repairLocked;

    private float _currentHealth;
    private GameStateMessage.GameState _gameState;

    public void Start()
    {
        RepairIcon.OnIconClicked += Repair;
        MessageBus.Subscribe<RepairEventMessage>(this, OnRepairEvent);
        MessageBus.Subscribe<GameStateMessage>(this, OnGameStateEvent);
        _currentHealth = Health;
    }

    public bool TakeDamage(float value)
    {
        _currentHealth -= value;
        if (_currentHealth <= 0 && State != FenceState.Broken)
        {
            State = FenceState.Broken;
            IsAvailable = false;
            MessageBus.Push(new TargetDestroyedMessage(this));
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


    private void OnGameStateEvent(GameStateMessage msg)
    {
        _gameState = msg.State;
        if(msg.State == GameStateMessage.GameState.WaveOver) {
            _currentHealth = Health;
        }
    }

    private void OnRepairEvent(RepairEventMessage ev)
    {
        _repairLocked = ev.IsRepairing;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        _playerInRange = true;
        if (State == FenceState.Broken)
        {
            RepairIcon.gameObject.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        _playerInRange = false;


        if (State == FenceState.Repairing)
        {
            State = FenceState.Broken;
            MessageBus.Push(new RepairEventMessage(false));
        }
    }


    public void Repair()
    {
        _repairStartedTime = Time.time;
        State = FenceState.Repairing;
        MessageBus.Push(new RepairEventMessage(true));
    }


    private void UpdateRepair()
    {
        if (State == FenceState.Repairing && Time.time > _repairStartedTime + RepairTime)
        {
            State = FenceState.Ok;
            IsAvailable = true;
            MessageBus.Push(new RepairEventMessage(false));
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

        RepairIcon.gameObject.SetActive(CanRepair());
    }


    private bool CanRepair()
    {
        return _playerInRange && !_repairLocked && State == FenceState.Broken && _gameState == GameStateMessage.GameState.WaveOver;
    }


    public enum FenceState
    {
        Ok,
        Broken,
        Repairing
    }
}