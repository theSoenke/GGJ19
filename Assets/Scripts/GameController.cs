using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WaveController))]
public class GameController : MonoBehaviour
{
    [SerializeField]
    private TargetConfig[] _targets;
    private int _targetValues;
    private const int maxTargetFindingTries = 100;

    public GameObject partyTable;
    public TextMeshProUGUI partyMessage;
    public Target[] Walls;
    public Target[] Player;
    public float timeBetweenParties = 10f;

    public Image believerBible;
    public TextMeshProUGUI nextRoundMessage;

    [Range(0f,1f)]
    public float believer = 0f;
    [SerializeField]
    private float roundCountdown = 2;
    public List<EnemyController> Enemies = new List<EnemyController>();   
    private WaveController waveController;

    private bool _isWaveActive;
    private float _timeTilParty = 10f;

    public void AddEnenemy(EnemyController enemy)
    {
        enemy.gameController = this;
        Enemies.Add(enemy);
        MessageBus.Subscribe<PartyMessage>(enemy, OnParty);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        Enemies.Remove(enemy);
    }

    public void StartWave()
    {
        waveController.StartWave();
        _isWaveActive = true;
    }

    public Target GetTarget(Vector3 position, bool shouldBePrimary, Func<Target, bool> isReachable)
    {
        var time = DateTime.Now;

        Target result = null;        
        var list = _targets.Where(t => t.Target.IsAvailable);

        if (shouldBePrimary)
        {            
            list = list.Where(t => t.Target.IsPrimaryTarget);           
        }

        list = list.ToList().Select(t => new TargetConfig(t)).OrderBy(t => t.Propability).ToList();

        var p = 0;
        var minDistance = list.First().GetDistance(position);
        var maxDistance = minDistance;
        foreach (var t in list)
        {
            var oldValue = p;
            p += t.Propability;
            t.Propability += oldValue;

            var distance = t.GetDistance(position);
            if (distance < minDistance) minDistance = distance;
            if (distance > maxDistance) maxDistance = distance;

            t.Distance = distance;
        }

        var distanceDiff = maxDistance - minDistance;
        foreach (var t in list)
        {
            t.Distance = (t.Distance - minDistance) / distanceDiff;
            if(!t.Target.IsPrimaryTarget)
                t.Propability -= (int)(t.Propability * (t.Distance * 0.5f));
        }

        var tries = 0;
        while (result == null && tries < maxTargetFindingTries)
        {
            var randomValue = UnityEngine.Random.Range(0, p + 1);
            
            var target = list.FirstOrDefault(t => randomValue <= t.Propability);

            if(target != null)
            {
                if (target.Target.CheckReachable)
                {
                    if (isReachable(target.Target)) result = target.Target;
                }
                else
                {
                    result = target.Target;
                }
            }

            tries++;
        }

        var wasted = DateTime.Now - time;
        //Debug.Log("Time wasted fiding Target : " + wasted.TotalMilliseconds+" ms");

        return result;
    }

    void Start()
    {
        _targets = FindObjectsOfType<Target>().Select(t => new TargetConfig(t)).OrderBy(t => t.Propability).ToArray();
       
        waveController = GetComponent<WaveController>();
        Walls = GameObject.FindGameObjectsWithTag("Target").Select(t => t.GetComponent<Target>()).ToArray();
        Player = GameObject.FindGameObjectsWithTag("Player").Select(t => t.GetComponent<Target>()).ToArray();

        MessageBus.Subscribe<TargetDestroyedMessage>(this, OnTargetDestroyed);
        MessageBus.Subscribe<EnemyDeadMessage>(this, OnEnemyDead);
    }

    void Update()
    {
        UpdateBelieverStatus();
        UpdateRoundTimer();

        _timeTilParty -= Time.deltaTime;

        if(_timeTilParty <= 0)
        {
            partyMessage.enabled = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Party();
            }
        }
    }

    private void Party()
    {
        partyMessage.enabled = false;
        var partyTableGo = Instantiate(partyTable);
        float partyDuration = 10f;
        _timeTilParty = timeBetweenParties + partyDuration;
        Destroy(partyTableGo, partyDuration);


        foreach(var enemy in Enemies)
        {
            MessageBus.Push(new PartyMessage(enemy));
        }
    }

    private void OnTargetDestroyed(TargetDestroyedMessage msg)
    {
        var walls = Walls.ToList();
        walls.Remove(msg.Target);
        Walls = walls.ToArray();
    }

    private void OnEnemyDead(EnemyDeadMessage msg)
    {
        if (Enemies.Contains(msg.enemyController))
        {
            RemoveEnemy(msg.enemyController);
        }
    }

    private void OnParty(PartyMessage msg)
    {
        if (Enemies.Contains(msg.enemyController))
        {
            var target = waveController.spawns[UnityEngine.Random.Range(0, waveController.spawns.Length)];
            msg.enemyController.SetDestionation(target.position, 10f);
        }
    }

    private void UpdateBelieverStatus()
    {
        if(believerBible != null)
        {
            believerBible.fillAmount = believer;
        }
    }

    private void UpdateRoundTimer()
    {
        if (!_isWaveActive)
        {
            if (roundCountdown > 0)
            {
                nextRoundMessage.text = "Incoming in " + roundCountdown.ToString("0");
                roundCountdown -= Time.deltaTime;
            }
            else
            {
                nextRoundMessage.gameObject.SetActive(false);
                StartWave();
            }
        }       
    }
}


public class TargetConfig
{
    public Target Target;
    public int Propability;
    public float Distance;

    public TargetConfig(Target target)
    {
        Target = target;
        Propability = target.Priority;
    }

    public TargetConfig(TargetConfig config)
    {
        Target = config.Target;
        Propability = Target.Priority;
    }

    public float GetDistance(Vector3 point)
    {
        return (point - Target.TargetPosition.position).magnitude;
    }
}
