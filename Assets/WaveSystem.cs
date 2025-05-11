using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private Transform charactersObject;
    private int characters;

    private GameObject spawnIndicatorImage1;
    private GameObject spawnIndicatorImage2;
    private GameObject spawnIndicatorImage3;
    private GameObject spawnIndicatorImage4;
    private TextMeshPro spawnIndicatorText1;
    private TextMeshPro spawnIndicatorText2;
    private TextMeshPro spawnIndicatorText3;
    private TextMeshPro spawnIndicatorText4;

    private CharacterScript characterScript;

    int spawner1Count = 0;
    int spawner2Count = 0;
    int spawner3Count = 0;
    int spawner4Count = 0;

    public int wave { get; private set; } = 0;
    public int waveMax { get; private set; } = 6;
    private bool waveSpawned = false;

    [SerializeField] private GameUI gameUI;

    private void Start()
    {
        characterScript = charactersObject.GetComponent<CharacterScript>();

        spawnIndicatorImage1 = GameObject.Find("spawnIndicatorImage1");
        spawnIndicatorImage2 = GameObject.Find("spawnIndicatorImage2");
        spawnIndicatorImage3 = GameObject.Find("spawnIndicatorImage3");
        spawnIndicatorImage4 = GameObject.Find("spawnIndicatorImage4");
        spawnIndicatorText1 = GameObject.Find("spawnIndicatorText1").GetComponent<TextMeshPro>();
        spawnIndicatorText2 = GameObject.Find("spawnIndicatorText2").GetComponent<TextMeshPro>();
        spawnIndicatorText3 = GameObject.Find("spawnIndicatorText3").GetComponent<TextMeshPro>();
        spawnIndicatorText4 = GameObject.Find("spawnIndicatorText4").GetComponent<TextMeshPro>();

        WaveManager();
    }


    private void Update()
    {
        if (wave < waveMax && characters == 0 && waveSpawned)
        {
            waveSpawned = false;
            Debug.Log("Characters " + characters);
            Debug.Log("New wave");
            WaveManager();
            gameUI.DisableShop();
        }

        if (characters == 0 && !waveSpawned)
        {
            gameUI.PlayButtonDisplay();
            gameUI.StoreButtonDisplay();

            int total = GetKnightCountForWave(wave);

            spawnIndicatorText1.text = ((total + 1) / 2).ToString();
            spawnIndicatorText2.text = (total / 2).ToString();

            total = GetMageCountForWave(wave);

            if (total == 0)
            {
                spawnIndicatorImage3.SetActive(false);
                spawnIndicatorText3.enabled = false;

                spawnIndicatorImage4.SetActive(false);
                spawnIndicatorText4.enabled = false;
            }
            else 
            {
                spawnIndicatorImage3.SetActive(true);
                spawnIndicatorText3.enabled = true;

                spawnIndicatorImage4.SetActive(true);
                spawnIndicatorText4.enabled = true;

                spawnIndicatorText3.text = ((total + 1) / 2).ToString();
                spawnIndicatorText4.text = (total / 2).ToString();
            }

            spawnIndicatorImage1.gameObject.SetActive(true);
            spawnIndicatorImage2.gameObject.SetActive(true);
            spawnIndicatorText1.enabled = true;
            spawnIndicatorText2.enabled = true;
        }
        else 
        {
            spawnIndicatorImage1.gameObject.SetActive(false);
            spawnIndicatorImage2.gameObject.SetActive(false);
            spawnIndicatorImage3.gameObject.SetActive(false);
            spawnIndicatorImage4.gameObject.SetActive(false);
            spawnIndicatorText1.enabled = false;
            spawnIndicatorText2.enabled = false;
            spawnIndicatorText3.enabled = false;
            spawnIndicatorText4.enabled = false;
        }
    }

    private void SpawnWave(GameObject character, int amount)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("WaveSpawner");
        Dictionary<string, Transform> spawnerDict = new Dictionary<string, Transform>();

        foreach (var obj in objectsWithTag)
        {
            spawnerDict[obj.name] = obj.transform;
        }

        Transform spawner1 = spawnerDict["WaveSpawner1"];
        Transform spawner2 = spawnerDict["WaveSpawner2"];
        Transform spawner3 = spawnerDict["WaveSpawner1"];
        Transform spawner4 = spawnerDict["WaveSpawner2"];

        spawner1Count = 0;
        spawner2Count = 0;
        spawner3Count = 0;
        spawner4Count = 0;

        Transform[] objectsTransform = new Transform[] { spawner1, spawner2 };

        for (int i = 0; i < amount; i++)
        {
            Transform spawnPoint = objectsTransform[i % objectsTransform.Length];

            Vector3 offset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            Vector3 spawnPosition = spawnPoint.position + offset;

            GameObject knight = Instantiate(character, spawnPosition, Quaternion.identity, charactersObject);
            characterScript.RegisterCharacter(knight.transform);
            characters++;
            
            NavMeshAgent agent = knight.GetComponent<NavMeshAgent>();

            if (spawnPoint == spawner1)
            {
                spawner1Count++;
            }
            else if (spawnPoint == spawner2)
            {
                spawner2Count++;
            }
            else if (spawnPoint == spawner3) 
            {
                spawner3Count++;
            }
            else if (spawnPoint == spawner4)
            {
                spawner4Count++;
            }
        }
    }

    private void WaveManager() 
    {
        if (wave <= waveMax)
        {
            if (!waveSpawned)
            {
                switch (wave)
                {
                    case 0:
                        Debug.Log("Game Started");
                        break;
                    case 1:
                        SpawnWave(knightPrefab, GetKnightCountForWave(wave - 1));
                        Debug.Log("Wave 1 spawned");
                        break;
                    case 2:
                        SpawnWave(knightPrefab, GetKnightCountForWave(wave - 1));
                        Debug.Log("Wave 2 spawned");
                        break;
                    case 3:
                        SpawnWave(knightPrefab, GetKnightCountForWave(wave - 1));
                        SpawnWave(magePrefab, GetMageCountForWave(wave - 1));
                        Debug.Log("Wave 3 spawned");
                        break;
                    case 4:
                        SpawnWave(knightPrefab, GetKnightCountForWave(wave - 1));
                        SpawnWave(magePrefab, GetMageCountForWave(wave - 1));
                        Debug.Log("Wave 4 spawned");
                        break;
                    case 5:
                        SpawnWave(knightPrefab, GetKnightCountForWave(wave - 1));
                        SpawnWave(magePrefab, GetMageCountForWave(wave - 1));
                        Debug.Log("Wave 5 spawned");
                        break;
                    case 6:
                        SpawnWave(knightPrefab, GetKnightCountForWave(wave - 1));
                        SpawnWave(magePrefab, GetMageCountForWave(wave + 1));
                        Debug.Log("Wave 6 spawned");
                        break;
                    default:
                        Debug.Log("No waves to spawn");
                        break;
                }
            }
        }
    }

    private int GetKnightCountForWave(int waveNumber)
    {
        switch (waveNumber)
        {
            case 0: return 1;
            case 1: return 2;
            case 2: return 4;
            case 3: return 7;
            case 4: return 8;
            case 5: return 10;
            default: return 0;
        }
    }
    private int GetMageCountForWave(int waveNumber)
    {
        switch (waveNumber)
        {
            case 2: return 2;
            case 3: return 2;
            case 4: return 3;
            case 5: return 5;
            default: return 0;
        }
    }

    public void RemoveDeadCharacter()
    {
        characters--;
        Debug.Log("Character died, total: " + characters);
    }

    public int PlayNextRound()
    {
        waveSpawned = true;
        return wave++;
    }
}