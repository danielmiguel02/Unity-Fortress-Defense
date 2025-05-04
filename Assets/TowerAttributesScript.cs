using UnityEngine;

public enum TowerType
{
    Basic,
    Medium,
    Strong,
    Fortress,
    Castle
}

public class TowerAttributesScript : MonoBehaviour
{
    public float health;
    public float damage;
    public float range;

    public TowerType towerType;
}