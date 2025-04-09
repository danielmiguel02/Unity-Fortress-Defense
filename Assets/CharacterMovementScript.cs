using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    [SerializeField] private GameObject towers;
    private Animator[] animators;
    public float speed = 1.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Animator anim in animators) {

            Transform closestTower = GetClosestTower(anim.transform);

            if (closestTower != null)
            {
                Vector3 targetPosition = new Vector3(closestTower.position.x, anim.transform.position.y, closestTower.position.z);
                Vector3 direction = targetPosition - anim.transform.position;
                float distance = direction.magnitude;

                if (distance > closestTower.localScale.x + 0.1f)
                {
                    anim.transform.position = Vector3.MoveTowards(anim.transform.position, targetPosition, speed * Time.deltaTime);
                    anim.transform.forward = Vector3.Normalize(direction);

                    anim.SetFloat("Blend", 0.25f);
                }
                else {
                    anim.SetFloat("Blend", 0f);
                }
            }
        }
    }

    private Transform GetClosestTower(Transform character) {

        Transform closest = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < towers.transform.childCount; i++) {
            Transform tower = towers.transform.GetChild(i);
            float distance = Vector3.Distance(character.position, tower.position);

            if (distance < minDistance) {
                minDistance = distance;
                closest = tower;
            }
        }

        return closest;
    }
}
