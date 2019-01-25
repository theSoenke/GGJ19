using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text believerStatus;

    private float believer = 0;
    private List<EnemyController> enemies = new List<EnemyController>();

    public void AddEnenemy(EnemyController enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        enemies.Remove(enemy);
    }

    void Update()
    {
        UpdateBelieverStatus();
    }

    private void UpdateBelieverStatus()
    {
        believerStatus.text = believer.ToString();
    }
}
