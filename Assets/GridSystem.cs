using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public string ObjectToPlace { private get; set; }

    [SerializeField] private GameObject BasicTowerObject;
    [SerializeField] private GameObject MediumTowerObject;
    [SerializeField] private GameObject StrongTowerObject;
    [SerializeField] private GameObject FortressTowerObject;

    private GameObject TowerToPlace;
    public GameObject ghostObject { get; set; }

    private HashSet<Vector3> occupiedPosition = new HashSet<Vector3>();
    public HashSet<Vector3> OccupiedPositionSet => occupiedPosition;


    public LayerMask groundLayerMask;
    public GameUI gameUI;
    public TowersScript towersScript;

    private Transform currentTileHit;

    private void Start()
    {
        TowerToPlace = BasicTowerObject;
        CreateGhostObject();
    }

    private void Update()
    {
        if (!gameUI.PlacedTower())
            UpdateGhostPosition();
        else
            ghostObject.SetActive(false);
    }

    public void CreateGhostObject()
    {
        switch (ObjectToPlace)
        {
            case "BasicTower":
                TowerToPlace = BasicTowerObject;
                break;
            case "MediumTower":
                TowerToPlace = MediumTowerObject;
                break;
            case "StrongTower":
                TowerToPlace = StrongTowerObject;
                break;
            case "FortressTower":
                TowerToPlace = FortressTowerObject;
                break;
        }

        ghostObject = Instantiate(TowerToPlace);

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        ghostObject.SetActive(false);
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayerMask))
        {
            currentTileHit = hit.collider.transform;
            Vector3 tilePosition = currentTileHit.position;

            ghostObject.transform.position = tilePosition;
            ghostObject.SetActive(true);

            if (occupiedPosition.Contains(tilePosition))
                SetGhostColor(Color.red);
            else
                SetGhostColor(Color.green);
        }
        else
        {
            ghostObject.SetActive(false);
        }
    }

    void SetGhostColor(Color color)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.sharedMaterial;
            mat.color = color;
        }
    }

    public void DisableGhostObject() 
    {
        ghostObject.SetActive(false);
        UpdateGhostPosition();
    }

    public Vector3 PlacementPosition()
    {
        return ghostObject.transform.position;
    }

    public Quaternion RotationPosition()
    {
        return ghostObject.transform.rotation;
    }

    public void AddOccupiedPosition(Vector3 pos)
    {
        occupiedPosition.Add(pos);
    }

    public void RemoveOccupiedPosition(Vector3 pos)
    {
        occupiedPosition.Remove(pos);
    }

    public void StartPlacing()
    {
        if (ghostObject != null)
            Destroy(ghostObject);

        CreateGhostObject();

        if (!gameUI.PlacedTower())
            ghostObject.SetActive(true);
    }

    public bool TryPlaceTower()
    {
        if (currentTileHit != null)
        {
            Vector3 pos = currentTileHit.position;

            if (!occupiedPosition.Contains(pos))
            {
                Instantiate(TowerToPlace, pos, Quaternion.identity);
                AddOccupiedPosition(pos);

                return true;
            }
        }
        return false;
    }
}

