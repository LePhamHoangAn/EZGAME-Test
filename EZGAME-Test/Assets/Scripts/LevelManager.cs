using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Settings")]
    public int totalLevels = 10;
    public int baseEnemyCount = 1;
    public float enemyCountGrowth = 0.5f;
    public float healthGrowth = 0.3f;
    private bool _allyMode;

    [Header("EnemiesShouldBePlaceByInceasingDifficulty")]
    public GameObject[] enemyPrefabs;
    public Vector2 spawnAreaMin = new Vector2(-10, -10);
    public Vector2 spawnAreaMax = new Vector2(10, 10);
    public Transform playerTransform;

    [Header("Allies")]
    public GameObject Ally;
    public float alliesAmount;

    [Header("UI")]
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject MenuScreen;
    private int currentLevel = 0;
    [SerializeField] private PlayerHealth _playerHealth;

    private void Start()
    {
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        MenuScreen.SetActive(true);
    }
    public void StartLevel(int levelIndex)
    {
        if(_allyMode)
        {
            SpawnAllies();
        }
        

        int enemyCount = Mathf.RoundToInt(baseEnemyCount + levelIndex * enemyCountGrowth);
        float enemyHealthMultiplier = 1 + levelIndex * healthGrowth;
        GameObject _prefab;
        if(levelIndex==0)
        {
            Vector3 spawnPos = new Vector3(Random.Range(spawnAreaMin.x, spawnAreaMax.x), playerTransform.position.y, Random.Range(spawnAreaMin.y, spawnAreaMax.y));

            _prefab = enemyPrefabs[0];
            GameObject enemy = Instantiate(_prefab, spawnPos, Quaternion.identity);

        }
        else if(levelIndex==1)
        {
            Vector3 spawnPos = new Vector3(Random.Range(spawnAreaMin.x, spawnAreaMax.x), playerTransform.position.y, Random.Range(spawnAreaMin.y, spawnAreaMax.y));

            _prefab = enemyPrefabs[1];
            GameObject enemy = Instantiate(_prefab, spawnPos, Quaternion.identity);

        }
        else
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(spawnAreaMin.x, spawnAreaMax.x), playerTransform.position.y, Random.Range(spawnAreaMin.y, spawnAreaMax.y));

                GameObject prefab = enemyPrefabs[Random.Range(0, Mathf.Min(enemyPrefabs.Length, 1 + levelIndex / 3))];
                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);


                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                    health.enemyHealth *= enemyHealthMultiplier;
            }

        }



        currentLevel = levelIndex;
    }

    private void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            WinScreen.SetActive(true);
        }

        if (_playerHealth != null &&_playerHealth.GetPlayerHealth()<=0)
        {
            LoseScreen.SetActive(true);

        }
    }

    private void SpawnAllies()
    {
        for (int i = 0; i < alliesAmount; i++)
        {
            // Just a tiny random offset near the player, simple scatter
            Vector3 offset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));

            Vector3 allyspawnPos = playerTransform.position + offset;

            Instantiate(Ally, allyspawnPos, Quaternion.identity);
        }
    }

    public void StartGame()
    {
        _allyMode = false;
        MenuScreen.SetActive(false);
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        StartLevel(0);

    }

    public void StartGameWithAllies()
    {
        _allyMode = true;
        MenuScreen.SetActive(false);
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        StartLevel(0);

    }

    public void RestartLevel()
    {
        //Clear enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allies)
        {
            Destroy(ally);
        }
        
        //Reset char's health
        _playerHealth.SetPlayerHealth(_playerHealth.playerHealth);
        LoseScreen.SetActive(false);

        StartLevel(currentLevel);
    }
    public void NextLevel()
    {
        if (currentLevel + 1 < totalLevels)
        {
            //Reset char's health
            _playerHealth.SetPlayerHealth(_playerHealth.playerHealth);

            StartLevel(currentLevel + 1);
            WinScreen.SetActive(false);

            GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
            foreach (GameObject ally in allies)
            {
                Destroy(ally);
            }
            if (_allyMode)
            {
                SpawnAllies();
            }
        }
        else
        {
            print("All levels finished");
        }
    }
}