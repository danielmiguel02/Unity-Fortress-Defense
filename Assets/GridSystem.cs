using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private GameObject objectToPlace;
    public float gridSize = 1f;
    private GameObject ghostObject;

    private HashSet<Vector3> occupiedPosition = new HashSet<Vector3>();
    public HashSet<Vector3> OccupiedPositionSet => occupiedPosition;

    public GameUI gameUI;
    public TowersScript towersScript;

    private void Start()
    {
        CreateGhostObject();
    }

    private void Update()
    {
        if (!gameUI.PlacedTower()) // Only show ghost while building
            UpdateGhostPosition();
        else
            ghostObject.SetActive(false); // Hide it once tower is placed
    }

    public void CreateGhostObject()
    {
        ghostObject = Instantiate(objectToPlace);

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_ScrBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        ghostObject.SetActive(false); // Keep it hidden until placement starts
    }


    void UpdateGhostPosition() 
    { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;

            Vector3 snappedPosition = new Vector3(Mathf.Round(point.x / gridSize) * gridSize, (float)((Mathf.Round(point.y / gridSize) * gridSize) + 0.5), Mathf.Round(point.z / gridSize) * gridSize);

            ghostObject.transform.position = snappedPosition;

            if (occupiedPosition.Contains(snappedPosition) && !gameUI.PlacedTower())
                SetGhostColor(Color.red);
            else if (!occupiedPosition.Contains(snappedPosition))
                SetGhostColor(Color.green);
            else
                SetGhostColor(Color.clear); //Clear the color so it doesn't show green/red (not working)
        }
    }

    void SetGhostColor(Color color) 
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers) 
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    public Vector3 PlacementPosition() 
    {
        Vector3 placementPosition = ghostObject.transform.position;

        return placementPosition;
    }

    public HashSet<Vector3> OccupiedPosition(Vector3 occupiedPos) 
    {
        occupiedPosition.Add(occupiedPos);
        return occupiedPosition;
    }

    public void StartPlacing()
    {
        if (ghostObject == null)
            CreateGhostObject();

        if (gameUI.PlacedTower() == false)
            ghostObject.SetActive(true);
    }

    public void AddOccupiedPosition(Vector3 pos)
    {
        occupiedPosition.Add(pos);
    }
}
