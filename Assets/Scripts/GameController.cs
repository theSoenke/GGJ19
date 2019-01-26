using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WaveController))]
public class GameController : MonoBehaviour
{    
    public Target[] Walls;
    public Target[] Player;

    public Image believerBible;
    public TextMeshProUGUI nextRoundMessage;

    [Range(0f,1f)]
    public float believer = 0f;
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
        believerBible.fillAmount = believer;
    }

    private void UpdateRoundTimer()
    {
        if(roundCountdown > 0)
        {
            nextRoundMessage.text = "Incoming in " + roundCountdown.ToString("0");
            roundCountdown -= Time.deltaTime;
        } else
        {
            nextRoundMessage.gameObject.SetActive(false);
        }
    }
}
