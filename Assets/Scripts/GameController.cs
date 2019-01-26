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
    private const int MaxTargetFindingTries = 100;

    public Target[] Walls;
    public Target[] Player;

    public Image believerBible;
    public TextMeshProUGUI nextRoundMessage;

    [Range(0f,1f)]
    public float believer = 0f;
    [SerializeField]
    private float roundCountdown = 2;
    private List<EnemyController> enemies = new List<EnemyController>();   
    private WaveController waveController;

    private bool _isWaveActive;

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
        while (result == null && tries < MaxTargetFindingTries)
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
        _targets = GameObject.FindObjectsOfType<Target>().Select(t => new TargetConfig(t)).OrderBy(t => t.Propability).ToArray();
        //_targetValues = 0;
        //foreach (var t in _targets)
        //{
        //    var oldValue = _targetValues;
        //    _targetValues += t.Propability;
        //    t.Propability += _targetValues;
        //}
       

        waveController = GetComponent<WaveController>();
        Walls = GameObject.FindGameObjectsWithTag("Target").Select(t => t.GetComponent<Target>()).ToArray();
        Player = GameObject.FindGameObjectsWithTag("Player").Select(t => t.GetComponent<Target>()).ToArray();

        MessageBus.Subscribe<TargetDestroyed>(this, OnTargetDestroyed);
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
