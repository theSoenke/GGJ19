using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class WaveController : MonoBehaviour
{
    [HideInInspector]
    public Transform[] spawns;
    private Target[] _targets;
    private GameController _gameController;

    private bool _started = false;
    private int _currentWave = 0;
    private WaveSetting _currentWaveSetting;
    private float _timeToSpawn;
    private float? _timeToIncreaseSpawnCount;
    private int _spawnCount;
    private int _spawnEntityCount;
    private int _spawnedCount;

    public float enemyScale = 0.02f;
    public WaveSetting[] Waves;
    public int CurrentEnemyCount;
    public bool AllEnemiesSpawned;
    public string[] messages;

    void Start()
    {
        _gameController = GetComponent<GameController>();
        spawns = GameObject.FindGameObjectsWithTag("EnemySpawn").Select(g => g.transform).ToArray();
        _targets = GameObject.FindGameObjectsWithTag("Target").Select(t => t.GetComponent<Target>()).ToArray();
    }

    public void StartWave()
    {
        if (_started) return;

        if(_currentWave >= Waves.Length)
        {
            _gameController.Won();
            return;
        }

        _currentWaveSetting = Waves[_currentWave];
        _currentWaveSetting.Init();

        _timeToSpawn = 0;
        _spawnedCount = 0;
        _spawnCount = _currentWaveSetting.SpawnCount;
        _spawnEntityCount = 0;
        CurrentEnemyCount = _currentWaveSetting.MaxEnemies;
        AllEnemiesSpawned = false;
        _timeToIncreaseSpawnCount = _currentWaveSetting.SpawnCountIncreaseTimeInSeconds > 0 ? _currentWaveSetting.SpawnCountIncreaseTimeInSeconds : (float?)null;

        _started = true;     
        MessageBus.Push(new GameStateMessage(GameStateMessage.GameState.WaveStart));      
    }

    public void EndWave()
    {
        _currentWave++;
        _started = false;
        MessageBus.Push(new GameStateMessage(GameStateMessage.GameState.WaveOver));
    }

    void Update()
    {
        if (_started)
        {
            UpdateWave();

            if (_spawnedCount > _currentWaveSetting.MaxEnemies)
            {
                _started = false;
                AllEnemiesSpawned = true;
            }
        }
    }



    private void UpdateWave()
    {        
        _timeToSpawn -= Time.deltaTime;

        if(_timeToSpawn <= 0)
        {
            SpawnEnemies();

            _timeToSpawn = _currentWaveSetting.SpawnTimeInSeconds;
        }
    }

    private void SpawnEnemies()
    {
        var prefabs = _currentWaveSetting.GetEnemiesPrefabs(_spawnCount);
        foreach (var p in prefabs)
        {
            var spawnIndex = UnityEngine.Random.Range(0, spawns.Length);
            var spawn = spawns[spawnIndex];
            
            var go = Instantiate(p, spawn.position, spawn.rotation);
            if(Application.platform == RuntimePlatform.Android)
            {
                go.transform.localScale = new Vector3(enemyScale, enemyScale, enemyScale);
            }
            var ec = go.GetComponent<EnemyController>();

            //var targetIndex = UnityEngine.Random.Range(0, _targets.Length);
            //var target = _targets[targetIndex];

            //ec.SetTarget(target);

            _gameController.AddEnenemy(ec);

            _spawnedCount++;            
        }
    }
}

[Serializable]
public class WaveSetting
{
    public int MaxEnemies = 1;
    public float SpawnTimeInSeconds = 2f;
    public int SpawnCount = 1;
    public float SpawnCountIncreaseTimeInSeconds = 10f;
    public WaveEnemySetting[] Enemies;

    private int _propabilities = 0;

    public void Init()
    {
        _propabilities = 0;

        Enemies = Enemies.OrderBy(s => s.SpawnProbability).ToArray();

        foreach(var e in Enemies)
        {
            var oldValue = _propabilities;
            _propabilities += e.SpawnProbability;
            e.SpawnProbability += oldValue;
        }
    }

    public IEnumerable<GameObject> GetEnemiesPrefabs(int count)
    {
        var result = new List<GameObject>();
        for (var i = 0; i < count; i++)
        {
            var number = UnityEngine.Random.Range(0, _propabilities);
            var prefab = Enemies.FirstOrDefault(e => number <= e.SpawnProbability);
            if (prefab != null)
            {
                result.Add(prefab.Prefab);
            }
        }

        return result;
    }
}

[Serializable]
public class WaveEnemySetting
{
    public GameObject Prefab;
    public int SpawnProbability;
}
