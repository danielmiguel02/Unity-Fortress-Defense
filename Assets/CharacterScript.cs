using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    [SerializeField] private GameObject towers;
    private Transform[] characters;
    private Animator[] animators; // Hold Animator references
    private float[] attackTimers;
    public float speed = 1.5f;

    TowersScript towersScript;
    CharacterAttributesScript attributes;

    public enum CharacterAttributes { 
        Health,
        Damage,
        Range
    };

    void Start()
    {
        List<Transform> charList = new List<Transform>();
        List<Animator> animList = new List<Animator>();

        towersScript = towers.GetComponent<TowersScript>();

        foreach (Transform child in transform)
        {
            charList.Add(child);
            animList.Add(child.GetComponent<Animator>()); // Get Animator for each character

            CharacterAttributesScript attributes = child.GetComponent<CharacterAttributesScript>();
            if (attributes == null)
            {
                attributes = child.gameObject.AddComponent<CharacterAttributesScript>();
            }

            if (child.CompareTag("Knight"))
            {
                attributes.health = 10f;
                attributes.damage = 1f;
                attributes.range = 0f;
            }
            if (child.CompareTag("Mage"))
            {
                attributes.health = 7f;
                attributes.damage = 1.3f;
                attributes.range = 3.2f;
            }
        }

        characters = charList.ToArray();
        animators = animList.ToArray();

        attackTimers = new float[characters.Length];
        for (int i = 0; i < attackTimers.Length; i++)
        {
            attackTimers[i] = 0f; // Start at 0 so they can attack immediately
        }
    }

    void Update()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            Transform chars = characters[i];
            Animator anim = animators[i];

            Transform closestTower = GetClosestTower(chars.transform);


            if (closestTower == null)
            {
                anim.Play("Idle");
            }
            else if (closestTower != null)
            {
                Vector3 targetPosition = new Vector3(closestTower.position.x, chars.transform.position.y, closestTower.position.z);
                Vector3 direction = targetPosition - chars.transform.position;
                float distance = direction.magnitude;

                attributes = chars.GetComponent<CharacterAttributesScript>();
                if (distance > closestTower.localScale.x + attributes.range)
                {
                    chars.transform.position = Vector3.MoveTowards(chars.transform.position, targetPosition, speed * Time.deltaTime);
                    chars.transform.forward = Vector3.Normalize(direction);

                    anim.Play("Running_A");
                }
                else
                {
                    attackTimers[i] -= Time.deltaTime;

                    if (attackTimers[i] <= 0f)
                    {
                        anim.Play("attack_animation");
                        towersScript.TowerHealth(closestTower);
                        attackTimers[i] = 1.0f; // Cooldown between attacks (1 second)
                    }
                }
            }
        }
    }

    private Transform GetClosestTower(Transform character)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < towers.transform.childCount; i++)
        {
            Transform tower = towers.transform.GetChild(i);
            float distance = Vector3.Distance(character.position, tower.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = tower;
            }
        }

        return closest;
    }

    public float CharacterHealth(float damage)
    {
        foreach (Transform c in characters)
        {
            if (c.CompareTag("Knight") && c != null)
            {
                CharacterAttributesScript attributes = c.GetComponent<CharacterAttributesScript>();
                Debug.Log($"{c.name} has {attributes.health} HP");
            }
        }
        return 0;
    }

    public float CharacterAttack() 
    {
        foreach (Transform c in characters)
        { 
            if (c.CompareTag("Knight") && c != null)
            {
                CharacterAttributesScript attributes = c.GetComponent<CharacterAttributesScript>();

                return attributes.damage;
            }
        }
        return 0;
    }
}
