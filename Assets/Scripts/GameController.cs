using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WaveController))]
public class GameController : MonoBehaviour
{
    public Text believerStatus;
    public Text nextRoundTimer;

    private float believer = 0;
    private float roundCountdown = 10;
    private List<EnemyController> enemies = new List<EnemyController>();
    private WaveController waveController;

    public void AddEnenemy(EnemyController enemy)
    {
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
    }

    void Update()
    {
        UpdateBelieverStatus();
        UpdateRoundTimer();
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
