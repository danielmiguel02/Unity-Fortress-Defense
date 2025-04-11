using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class TowersScript : MonoBehaviour
{
    private Transform[] towers;
    [SerializeField] private GameObject characters;
    CharacterScript characterScript;

    public enum TowerAttributes { 
        Health,
        Range
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Transform> towerList = new List<Transform>();

        characterScript = characters.GetComponent<CharacterScript>();

        foreach (Transform t in transform)
        {
            towerList.Add(t);

            TowerAttributesScript attributes = t.GetComponent<TowerAttributesScript>();
            if (attributes == null) { 
                attributes = t.gameObject.AddComponent<TowerAttributesScript>();
            }

            if (t.CompareTag("BasicTower")) {
                attributes.health = 10f;
            }
        }

        towers = towerList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < towers.Length; i++)
        {
            Transform t = towers[i];

            // Safeguard: skip if already destroyed
            if (t == null) continue;

            TowerAttributesScript attributes = t.GetComponent<TowerAttributesScript>();

            if (attributes.health <= 0)
            {
                Destroy(t.gameObject);
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

        return 0;
    }
}