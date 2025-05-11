using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class TowersScript : MonoBehaviour
{
    private List<Transform> towers;
    private Transform towersTransform;
    [SerializeField] private GameObject characters;
    [SerializeField] private ProjectileSpawner arrow;
    CharacterScript characterScript;
    private List<float> attackTimers;
    List<Transform> towerList;

    public GameObject basicTowerObject;
    public GameObject mediumTowerObject;
    public GameObject strongTowerObject;
    public GameObject fortressTowerObject;

    public GameUI gameUI;
    [SerializeField] private CoinsSystem coinsSystem;
    public GridSystem gridSystem;
    private new Camera camera;

    public Dictionary<string, TowerStats> towerTemplates;
    public enum TowerAttributes
    {
        Health,
        Damage,
        Range
    };


    void Awake()
    {
        // Initialize tower templates
        towerTemplates = new Dictionary<string, TowerStats>
        {
            ["BasicTower"] = new TowerStats { health = 10f, damage = 1.6f, range = 5f, towerType = TowerType.Basic },
            ["MediumTower"] = new TowerStats { health = 12f, damage = 1.8f, range = 7f, towerType = TowerType.Medium },
            ["StrongTower"] = new TowerStats { health = 15f, damage = 2.2f, range = 8f, towerType = TowerType.Strong },
            ["FortressTower"] = new TowerStats { health = 18f, damage = 2.8f, range = 9f, towerType = TowerType.Fortress },
            ["Castle"] = new TowerStats { health = 20f, damage = 3.5f, range = 10f, towerType = TowerType.Castle }
        };

        towers = new List<Transform>();
        attackTimers = new List<float>();
        towersTransform = GetComponent<Transform>();

        characterScript = characters.GetComponent<CharacterScript>();
        camera = Camera.main;

        foreach (Transform t in transform)
        {
            towers.Add(t);
            attackTimers.Add(0f);

            TowerAttributesScript attributes = t.GetComponent<TowerAttributesScript>();
            if (attributes == null)
            {
                attributes = t.gameObject.AddComponent<TowerAttributesScript>();
            }

            string tag = t.tag;

            if (towerTemplates.TryGetValue(tag, out TowerStats stats))
            {
                attributes.health = stats.health;
                attributes.damage = stats.damage;
                attributes.range = stats.range;
                attributes.towerType = stats.towerType;
            }
            else
            {
                Debug.LogWarning($"Tower with tag {tag} does not have a defined template.");
            }
        }
    }


    void Update()
    {
        for (int i = 0; i < towers.Count; i++)
        {
            HandleTowerBehavior(i);
        }
    }

    public void HandleTowerBehavior(int index)
    {
        Transform t = towers[index];
        if (t == null) return;

        TowerAttributesScript attributes = t.GetComponent<TowerAttributesScript>();
        if (attributes.health <= 0)
        {
            Destroy(t.gameObject);
            gridSystem.RemoveOccupiedPosition(t.position);
            if (t.CompareTag("Castle"))
            {
                Debug.Log("Warrions WIN");
            }
        }
        else
        {
            Transform closestCharacter = GetClosestCharacter(t, attributes.range);
            if (closestCharacter == null) return;

            Vector3 targetPosition = new Vector3(closestCharacter.position.x, t.position.y, closestCharacter.position.z);
            float attackDistance = closestCharacter.localScale.x + attributes.range;
            float currentDistance = Vector3.Distance(t.position, targetPosition);

            attackTimers[index] -= Time.deltaTime;
            if (currentDistance < attackDistance && attackTimers[index] <= 0f && !characterScript.isDead(closestCharacter))
            {
                attackTimers[index] = 1.5f;

                characterScript.CharacterHealth(closestCharacter, attributes.damage);
                Debug.Log(t.gameObject.name + " dealt " + attributes.damage + " damage to " + closestCharacter);

                ProjectileSpawner thisArrow = t.GetComponentInChildren<ProjectileSpawner>();
                if (thisArrow != null)
                {
                    thisArrow.ShootArrow(closestCharacter);
                }
                else
                {
                    Debug.LogWarning("ProjectileSpawner not found for tower: " + t.name);
                }
            }
        }
    }


    public float TowerHealth(Transform targetTower)
    {
        if (targetTower != null && targetTower.CompareTag("BasicTower") || targetTower.CompareTag("MediumTower") || targetTower.CompareTag("StrongTower") || targetTower.CompareTag("FortressTower"))
        {
            TowerAttributesScript attributes = targetTower.GetComponent<TowerAttributesScript>();
            attributes.health -= characterScript.CharacterAttack();
            Debug.Log($"{targetTower.name} has {attributes.health} HP");
            return attributes.health;
        }

        if (targetTower != null && targetTower.CompareTag("Castle"))
        {
            TowerAttributesScript attributes = targetTower.GetComponent<TowerAttributesScript>();
            attributes.health -= characterScript.CharacterAttack();
            Debug.Log($"{targetTower.name} has {attributes.health} HP");
            return attributes.health;
        }

        return 0;
    }


    private Transform GetClosestCharacter(Transform tower, float range)
    {
        Transform closest = null;
        float minDistance = range;

        foreach (Transform character in characters.transform)
        {
            float distance = Vector3.Distance(tower.position, character.position);
            if (distance <= range && distance < minDistance)
            {
                minDistance = distance;
                closest = character;
            }
        }

        return closest;
    }

    public GameObject PlaceTower(bool waitingToPlaceTower, GameObject newTower)
    {
        Vector3 placementPos = gridSystem.PlacementPosition();
        Quaternion rotationPos = gridSystem.RotationPosition();
        string towerSelected = gameUI.towerSelected;

        if (waitingToPlaceTower && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!gridSystem.OccupiedPositionSet.Contains(placementPos) && gameUI.waitingToPlaceTower && coinsSystem.currentPlayerCoins >= coinsSystem.TowerCost(towerSelected))
            {
                if (newTower == basicTowerObject)
                    newTower = Instantiate(basicTowerObject, placementPos, rotationPos, towersTransform);
                if (newTower == mediumTowerObject)
                    newTower = Instantiate(mediumTowerObject, placementPos, rotationPos, towersTransform);
                if (newTower == strongTowerObject)
                    newTower = Instantiate(strongTowerObject, placementPos, rotationPos, towersTransform);
                if (newTower == fortressTowerObject)
                    newTower = Instantiate(fortressTowerObject, placementPos, rotationPos, towersTransform);

                towers.Add(newTower.transform);
                attackTimers.Add(0f); // Initialize the timer for the new tower

                gridSystem.AddOccupiedPosition(placementPos); // Add it AFTER placement

                TowerAttributesScript attributes = newTower.GetComponent<TowerAttributesScript>();
                if (attributes == null)
                    attributes = newTower.AddComponent<TowerAttributesScript>();

                if (newTower.CompareTag("BasicTower"))
                {
                    attributes.health = 10f;
                    attributes.damage = 1.6f;
                    attributes.range = 7f;
                    attributes.towerType = TowerType.Basic;
                }

                if (newTower.CompareTag("MediumTower"))
                {
                    attributes.health = 10f;
                    attributes.damage = 1.6f;
                    attributes.range = 7f;
                    attributes.towerType = TowerType.Medium;
                }
                if (newTower.CompareTag("StrongTower"))
                {
                    attributes.health = 10f;
                    attributes.damage = 1.6f;
                    attributes.range = 7f;
                    attributes.towerType = TowerType.Strong;
                }
                if (newTower.CompareTag("FortressTower"))
                {
                    attributes.health = 10f;
                    attributes.damage = 1.6f;
                    attributes.range = 7f;
                    attributes.towerType = TowerType.Fortress;
                }
            }
        }

        return newTower;
    }
}