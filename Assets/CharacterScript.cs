using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterScript : MonoBehaviour
{
    [SerializeField] private GameObject towers;
    private Transform[] characters;
    private Animator[] animators;
    private NavMeshAgent[] agents;
    private float[] attackTimers;

    public float speed = 1.5f;

    TowersScript towersScript;

    void Start()
    {
        List<Transform> charList = new List<Transform>();
        List<Animator> animList = new List<Animator>();
        List<NavMeshAgent> agentList = new List<NavMeshAgent>();

        towersScript = towers.GetComponent<TowersScript>();

        foreach (Transform child in transform)
        {
            charList.Add(child);
            animList.Add(child.GetComponent<Animator>());

            // Ensure CharacterAttributesScript exists
            var attributes = child.GetComponent<CharacterAttributesScript>();
            if (attributes == null)
                attributes = child.gameObject.AddComponent<CharacterAttributesScript>();

            // Set attributes based on character type
            if (child.CompareTag("Knight"))
            {
                attributes.health = 10f;
                attributes.damage = 1f;
                attributes.range = 0f;
            }
            else if (child.CompareTag("Mage"))
            {
                attributes.health = 7f;
                attributes.damage = 1.3f;
                attributes.range = 2.2f;
            }

            // NavMeshAgent
            var agent = child.GetComponent<NavMeshAgent>();
            if (agent == null)
                agent = child.gameObject.AddComponent<NavMeshAgent>();

            agent.speed = speed;
            agent.angularSpeed = 0f;
            agent.updateRotation = false;
            agentList.Add(agent);
        }

        characters = charList.ToArray();
        animators = animList.ToArray();
        agents = agentList.ToArray();

        attackTimers = new float[characters.Length];
    }

    void Update()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            Transform character = characters[i];
            Animator anim = animators[i];
            NavMeshAgent agent = agents[i];

            // Safeguard: skip if already destroyed
            if (character == null) continue;


            var attributes = character.GetComponent<CharacterAttributesScript>();
            if (attributes.health <= 0f) {
                Destroy(character.gameObject);
            }

            Transform closestTower = GetClosestTower(character);
            if (closestTower == null)
            {
                anim.Play("Idle");
                agent.ResetPath();
                continue;
            }

            Vector3 targetPosition = new Vector3(closestTower.position.x, character.position.y, closestTower.position.z);
            float stoppingDistance = closestTower.localScale.x + attributes.range;
            float currentDistance = Vector3.Distance(character.position, targetPosition);

            agent.stoppingDistance = stoppingDistance;

            if (currentDistance > stoppingDistance)
            {
                if (agent.isOnNavMesh)
                    agent.SetDestination(targetPosition);

                Vector3 moveDir = agent.velocity.normalized;
                if (moveDir.magnitude > 0.1f)
                    character.forward = moveDir;

                anim.Play("Running_A");
            }
            else
            {
                if (agent.isOnNavMesh)
                    agent.ResetPath();

                attackTimers[i] -= Time.deltaTime;
                if (attackTimers[i] <= 0f)
                {
                    anim.Play("attack_animation");
                    towersScript.TowerHealth(closestTower);
                    attackTimers[i] = 1.0f;
                }
            }
        }
    }

    private Transform GetClosestTower(Transform character)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform tower in towers.transform)
        {
            float distance = Vector3.Distance(character.position, tower.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = tower;
            }
        }

        return closest;
    }

    public float CharacterHealth(Transform targetCharacter, float damage)
    {
        if (targetCharacter != null)
        {
            CharacterAttributesScript attributes = targetCharacter.GetComponent<CharacterAttributesScript>();
            attributes.health -= damage;
            Debug.Log($"{targetCharacter.name} took {damage} damage and has {attributes.health} HP");
            return attributes.health;
        }

        return 0;
    }

    public float CharacterAttack()
    {
        foreach (Transform c in characters)
        {
            if (c == null) continue;

            var attributes = c.GetComponent<CharacterAttributesScript>();
            if (c.CompareTag("Knight") || c.CompareTag("Mage"))
                return attributes.damage;
        }
        return 0;
    }
}
