using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private CharacterScript characterScript;
    [SerializeField] private GameObject objectToThrow;

    private float damage;

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void ShootArrow(Transform target)
    {
        if (target != null)
        {
            GameObject projectile = Instantiate(objectToThrow, transform.position, transform.rotation);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            Vector3 startPos = transform.position;
            Vector3 targetPos = target.position;

            // Calculate the direction and distance to the target
            Vector3 toTarget = targetPos - startPos;
            Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);

            float y = toTarget.y;
            float xz = toTargetXZ.magnitude;

            float gravity = Physics.gravity.y;

            // Choose an angle (e.g. 45 degrees for high arc)
            float angleInDegrees = 35f;
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // Calculate the velocity magnitude
            float v2 = (gravity * xz * xz) / (2 * (y - Mathf.Tan(angleInRadians) * xz) * Mathf.Pow(Mathf.Cos(angleInRadians), 2));

            // If v2 is negative, that means no valid solution
            if (v2 <= 0)
            {
                Debug.LogWarning("Target too close or too far; can't compute a valid trajectory.");
                return;
            }

            float velocity = Mathf.Sqrt(v2);

            // Decompose velocity into XZ and Y components
            Vector3 directionXZ = toTargetXZ.normalized;
            Vector3 velocityVector = directionXZ * velocity * Mathf.Cos(angleInRadians);
            velocityVector.y = velocity * Mathf.Sin(angleInRadians);

            velocityVector *= 0.8f;

            projectileRb.linearVelocity = velocityVector;

            Destroy(projectile, 10f);
        }
    }
}
