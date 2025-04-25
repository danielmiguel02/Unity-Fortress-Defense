using UnityEngine;
using UnityEngine.UI; // Use this instead of UIElements
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class GameUI : MonoBehaviour
{
    private Button buildButton;
    private Button basicTowerButton;
    private Button mediumTowerButton;
    private Button strongTowerButton;
    private Button fortressTowerButton;
    private Image towerKeys;
    private Canvas buildMenu;

    [SerializeField] private GameObject basicTowerObject;
    [SerializeField] private GameObject mediumTowerObject;
    [SerializeField] private GameObject strongTowerObject;
    [SerializeField] private GameObject fortressTowerObject;
    [SerializeField] private TowersScript towersScript;
    [SerializeField] private GridSystem gridSystem;

    private CoinsSystem coinsSystem;

    public bool waitingToPlaceTower { get; set; } = false;
    public string towerSelected {  get; set; }

    private void Awake()
    {
        // Optional setup if needed
    }

    private void OnEnable()
    {
        // Find buttons by name in the scene or better yet assign via inspector
        buildButton = GameObject.Find("Build-Button").GetComponent<Button>();
        buildMenu = GameObject.Find("Towers-Store").GetComponent<Canvas>();
        basicTowerButton = GameObject.Find("BasicTowerButton").GetComponent<Button>();
        mediumTowerButton = GameObject.Find("MediumTowerButton").GetComponent<Button>();
        strongTowerButton = GameObject.Find("StrongTowerButton").GetComponent<Button>();
        fortressTowerButton = GameObject.Find("FortressTowerButton").GetComponent<Button>();
        towerKeys = GameObject.Find("Tower-Keys").GetComponent<Image>();
        coinsSystem = GetComponent<CoinsSystem>();

        buildButton.onClick.AddListener(OnBuildButtonClicked);
        basicTowerButton.onClick.AddListener(OnBasicTowerButtonClicked);
        mediumTowerButton.onClick.AddListener(OnMediumTowerButtonClicked);
        strongTowerButton.onClick.AddListener(OnStrongTowerButtonClicked);
        fortressTowerButton.onClick.AddListener(OnFortressTowerButtonClicked);

        buildMenu.enabled = false;
        towerKeys.enabled = false;
    }

    void Update()
    {
        EventSystem.current.SetSelectedGameObject(null);

        switch (towerSelected) 
        {
            case "basicTower":
                towersScript.PlaceTower(waitingToPlaceTower, basicTowerObject);
                break;
            case "mediumTower":
                towersScript.PlaceTower(waitingToPlaceTower, mediumTowerObject);
                break;
            case "strongTower":
                towersScript.PlaceTower(waitingToPlaceTower, strongTowerObject);
                break;
            case "fortressTower":
                towersScript.PlaceTower(waitingToPlaceTower, fortressTowerObject);
                break;
        }

        if (waitingToPlaceTower) 
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                coinsSystem.BuyTower(towerSelected);
            }

            if (Input.GetMouseButtonDown(1)) 
            {
                waitingToPlaceTower = false;
                towerKeys.enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                gridSystem.ghostObject.transform.Rotate(0f, 60f, 0f);
            }
        }
    }

    private void OnBuildButtonClicked()
    {
        Debug.Log("Build Button");

        if (!buildMenu.enabled)
            buildMenu.enabled = true;
        else buildMenu.enabled = false;
    }

    private void OnBasicTowerButtonClicked()
    {
        towerSelected = "basicTower";
        gridSystem.ObjectToPlace = "BasicTower";

        Debug.Log("BasicTowerButton Clicked");
        buildMenu.enabled = false;
        towerKeys.enabled = true;

        waitingToPlaceTower = true;
        gridSystem.StartPlacing();
    }
    private void OnMediumTowerButtonClicked()
    {
        towerSelected = "mediumTower";
        gridSystem.ObjectToPlace = "MediumTower";

        Debug.Log("MediumTowerButton Clicked");
        buildMenu.enabled = false;
        towerKeys.enabled = true;

        waitingToPlaceTower = true;
        gridSystem.StartPlacing();
    }
    private void OnStrongTowerButtonClicked()
    {
        towerSelected = "strongTower";
        gridSystem.ObjectToPlace = "StrongTower";

        Debug.Log("StrongTowerButton Clicked");
        buildMenu.enabled = false;
        towerKeys.enabled = true;

        waitingToPlaceTower = true;
        gridSystem.StartPlacing();
    }
    private void OnFortressTowerButtonClicked()
    {
        towerSelected = "fortressTower";
        gridSystem.ObjectToPlace = "FortressTower";

        Debug.Log("FortressTowerButton Clicked");
        buildMenu.enabled = false;
        towerKeys.enabled = true;

        waitingToPlaceTower = true;
        gridSystem.StartPlacing();
    }

    public bool PlacedTower()
    {
        return !waitingToPlaceTower;
    }
}
