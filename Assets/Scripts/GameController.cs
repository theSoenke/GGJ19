using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WaveController))]
public class GameController : MonoBehaviour
{    
    public Target[] Walls;
    public Target[] Player;

    public Text believerStatus;
    public Text nextRoundTimer;

    private float believer = 0;
    private float roundCountdown = 10;
    private List<EnemyController> enemies = new List<EnemyController>();   
    private WaveController waveController;

    public void AddEnenemy(EnemyController enemy)
    {
        enemy.GameController = this;
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        enemies.Remove(enemy);
    }

    public void StartWave()
    {
        waveController.StartWave();
    }

    void Start()
    {
        waveController = GetComponent<WaveController>();
        Walls = GameObject.FindGameObjectsWithTag("Target").Select(t => t.GetComponent<Target>()).ToArray();

        MessageBus.Subscribe<TargetDestroyed>(OnTargetDestroyed);
    }

    void Update()
    {
        UpdateBelieverStatus();
        UpdateRoundTimer();
    }

    private void OnTargetDestroyed(TargetDestroyed msg)
    {
        var walls = Walls.ToList();
        walls.Remove(msg.Target);
        Walls = walls.ToArray();
    }

    private void UpdateBelieverStatus()
    {
        believerStatus.text = believer.ToString();
    }

    private void UpdateRoundTimer()
    {
        if(roundCountdown > 0)
        {
            nextRoundTimer.text = "Incoming in " + roundCountdown.ToString("0");
            roundCountdown -= Time.deltaTime;
        } else
        {
            nextRoundTimer.enabled = false;
        }
    }
}
