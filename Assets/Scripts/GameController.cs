using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public string[] messages;
    public TextMeshProUGUI roundInitMessage;

    [HideInInspector]
    public float believer = 0f;
    [SerializeField]
    private float roundCountdown = 2;
    public float messageDisplayTime = 5f;
    public List<EnemyController> Enemies = new List<EnemyController>();   

    [HideInInspector]
    public WaveController waveController;

    private bool _isWaveActive;
    private int _enemyKilled;
    private float _countdown;
    private float _timeTilParty = 30f;
    private bool _isParty;
    private float _messageCountdown;


    public void AddEnenemy(EnemyController enemy)
    {
        enemy.gameController = this;
        Enemies.Add(enemy);

        if(_isParty)
        {
            enemy.OnParty(new PartyMessage(true));
        }
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        Enemies.Remove(enemy);
        if(waveController.AllEnemiesSpawned && Enemies.Count == 0)
        {
            EndWave();
        }

        //_enemyKilled++;
        //Debug.Log("Enemy killed. Dead: " + _enemyKilled + " / " + waveController.CurrentEnemyCount);

        //if (_enemyKilled >= waveController.CurrentEnemyCount)
        //{
        //    EndWave();
        //}
    }

    public void StartWave()
    {
        waveController.StartWave();
        _enemyKilled = 0;
        _isWaveActive = true;
        _timeTilParty = timeBetweenParties;
    }

    public void EndWave()
    {
        _timeTilParty = roundCountdown + _timeTilParty;
        waveController.EndWave();
        _isWaveActive = false;
        _countdown = roundCountdown;
        _messageCountdown = messageDisplayTime;
        nextRoundMessage.gameObject.SetActive(true);
    }

    public Target GetTarget(Vector3 position, bool shouldBePrimary, int minLevel, Func<Target, bool> isReachable)
    {
        var time = DateTime.Now;

        Target result = null;        
        var list = _targets.Where(t => t.Target.IsAvailable && t.Target.Level >= minLevel);

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
        
        MessageBus.Subscribe<EnemyDeadMessage>(this, OnEnemyDead);

        _countdown = roundCountdown;
        _messageCountdown = messageDisplayTime;

        MessageBus.Push(new GameStateMessage(GameStateMessage.GameState.GameStart));
    }

    void Update()
    {
        UpdateBelieverStatus();
        UpdateRoundTimer();

        _timeTilParty -= Time.deltaTime;

        if(_isParty && _timeTilParty <= timeBetweenParties)
        {
            _isParty = false;
            MessageBus.Push(new PartyMessage(false));
        }

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
        if (_isParty) return;

        partyMessage.enabled = false;
        var partyTableGo = Instantiate(partyTable);
        float partyDuration = 10f;
        _timeTilParty = timeBetweenParties + partyDuration;
        Destroy(partyTableGo, partyDuration);
        MessageBus.Push(new PartyMessage(true));
        _isParty = true;
    }

    private void DisplayMessage()
    {
        if (roundInitMessage.text == "")
        {
            var message = messages[UnityEngine.Random.Range(0, messages.Length)];
            roundInitMessage.gameObject.SetActive(true);
            message = "DING DONG! \n\n" + message;
            roundInitMessage.text = message;
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
   
    private void UpdateBelieverStatus()
    {
        believerBible.fillAmount = believer;
        if(believer >= 1)
        {
            Gameover();
        }
    }

    private void Gameover()
    {
        MessageBus.Push(new GameStateMessage(GameStateMessage.GameState.GameOver));
        SceneManager.LoadScene("Gameover");
    }

    private void UpdateRoundTimer()
    {
        if (!_isWaveActive)
        {
            if(_messageCountdown > 0)
            {
                _messageCountdown -= Time.deltaTime;
                DisplayMessage();
            }
            else if (_countdown > 0)
            {
                roundInitMessage.gameObject.SetActive(false);
                nextRoundMessage.gameObject.SetActive(true);
                nextRoundMessage.text = "Incoming in " + _countdown.ToString("0");
                _countdown -= Time.deltaTime;
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
