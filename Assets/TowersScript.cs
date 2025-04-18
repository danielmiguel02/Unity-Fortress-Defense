using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class TowersScript : MonoBehaviour
{
    private List<Transform> towers;
    private Transform towersTransform;
    [SerializeField] private GameObject characters;
    CharacterScript characterScript;
    private List<float> attackTimers;
    List<Transform> towerList;

    public GameObject basicTowerObject;
    public GameUI gameUI;
    public GridSystem gridSystem;
    private new Camera camera;

    public enum TowerAttributes {
        Health,
        Damage,
        Range
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

            if (t.CompareTag("BasicTower"))
            {
                attributes.health = 10f;
                attributes.damage = 1.6f;
                attributes.range = 7f;
            }

            if (t.CompareTag("Castle"))
            {
                attributes.health = 20f;
                attributes.damage = 0f;
                attributes.range = 10f;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < towers.Count; i++)
        {
            Transform t = towers[i];

            // Safeguard: skip if already destroyed
            if (t == null) continue;

            TowerAttributesScript attributes = t.GetComponent<TowerAttributesScript>();

            if (attributes.health <= 0)
            {
                Destroy(t.gameObject);
                if (t.gameObject.CompareTag("Castle")) 
                {
                    Debug.Log("Warrions WIN");
                }
            } else
            {
                Transform closestCharacter = GetClosestCharacter(t);
                if (closestCharacter == null)
                    continue;

                Vector3 targetPosition = new Vector3(closestCharacter.position.x, t.position.y, closestCharacter.position.z);
                float attackDistance = closestCharacter.localScale.x + attributes.range;
                float currentDistance = Vector3.Distance(t.position, targetPosition);


                attackTimers[i] -= Time.deltaTime;
                if (currentDistance < attackDistance && attackTimers[i] <= 0f) {
                    characterScript.CharacterHealth(closestCharacter, attributes.damage);
                    attackTimers[i] = 1.0f;
                }
            }
        }
    }

    public float TowerHealth(Transform targetTower)
    {
        if (targetTower != null && targetTower.CompareTag("BasicTower"))
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

    public float TowerAttack()
    {
        foreach (Transform t in towers)
        {
            if (t == null) continue;

            TowerAttributesScript attributes = t.GetComponent<TowerAttributesScript>();
            if (t.CompareTag("BasicTower"))
                return attributes.damage;

            attributes = t.GetComponent<TowerAttributesScript>();
            if (t.CompareTag("Castle"))
                return attributes.damage;

        }
        return 0;
    }

    private Transform GetClosestCharacter(Transform tower)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform character in characters.transform)
        {
            float distance = Vector3.Distance(tower.position, character.position);
            if (distance < minDistance)
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

        if (waitingToPlaceTower && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Check if the position is already occupied BEFORE adding
            if (!gridSystem.OccupiedPositionSet.Contains(placementPos) && !gameUI.PlacedTower())
            {
                newTower = Instantiate(basicTowerObject, placementPos, Quaternion.identity, towersTransform);
                towers.Add(newTower.transform);
                attackTimers.Add(0f); // Initialize the timer for the new tower

                gridSystem.AddOccupiedPosition(placementPos); // Add it AFTER placement
                gameUI.WaitingToPlaceTower(false);

                TowerAttributesScript attributes = newTower.GetComponent<TowerAttributesScript>();
                if (attributes == null)
                    attributes = newTower.AddComponent<TowerAttributesScript>();

                if (newTower.CompareTag("BasicTower"))
                {
                    attributes.health = 10f;
                    attributes.damage = 1.6f;
                    attributes.range = 7f;
                }
            }
        }

        return newTower;
    }

}