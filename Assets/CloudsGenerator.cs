using NUnit.Framework.Internal;
using UnityEngine;

public class CloudsGenerator : MonoBehaviour
{
    [SerializeField] int initialCloudCount = 40;
    [SerializeField] Transform cloudSmallPrefab;
    [SerializeField] Transform cloudBigPrefab;
    [SerializeField] BoxCollider boxCollider;

    private Bounds bounds;
    private Transform cloudsParent;

    void Start()
    {
        bounds = boxCollider.bounds;

        // Create a parent for the clouds
        cloudsParent = new GameObject("CloudsParent").transform;

        // Spawn the initial clouds
        for (int i = 0; i < initialCloudCount; i++)
        {
            SpawnCloud();
        }
    }

    void Update()
    {
        MoveClouds();
    }

    void MoveClouds()
    {
        // Create a temp list to avoid modifying collection during iteration
        var toDestroy = new System.Collections.Generic.List<Transform>();

        foreach (Transform cloud in cloudsParent)
        {
            float moveSpeed = Random.Range(0.4f, 1f);
            cloud.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (cloud.position.x < bounds.min.x)
            {
                toDestroy.Add(cloud);
            }
        }

        foreach (var cloud in toDestroy)
        {
            Destroy(cloud.gameObject);
            SpawnCloud();
        }
    }

    void SpawnCloud()
    {
        float x = Random.Range(bounds.max.x - 20, bounds.max.x + 80);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 position = new Vector3(x, y, z);

        Transform cloud;
        int randomNumber = Random.Range(0, 2);

        switch (randomNumber)
        {
            case 0:
                cloud = Instantiate(cloudSmallPrefab, position, cloudSmallPrefab.rotation);
                cloud.SetParent(cloudsParent);
                break;
            case 1:
                cloud = Instantiate(cloudBigPrefab, position, cloudSmallPrefab.rotation);
                cloud.SetParent(cloudsParent);
                break;
        }
    }
}
