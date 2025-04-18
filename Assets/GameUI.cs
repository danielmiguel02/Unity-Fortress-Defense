using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{    
    private VisualElement ui;
    private Button buildButton;
    private Button basicTowerButton;
    private VisualElement buildMenu;

    [SerializeField] private GameObject basicTowerObject;
    public TowersScript towersScript;
    public GridSystem gridSystem;

    private bool waitingToPlaceTower = false;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        buildButton = ui.Q<Button>("Build-Button");
        buildButton.clicked += OnBuildButtonClicked;

        basicTowerButton = ui.Q<Button>("Basic-Tower-Button");
        basicTowerButton.clicked += OnBasicTowerButtonClicked;

        buildMenu = ui.Q<VisualElement>("Build-Menu");
        buildMenu.style.display = DisplayStyle.None;
    }

    void Update()
    {
        towersScript.PlaceTower(waitingToPlaceTower, basicTowerObject);
    }

    private void OnBuildButtonClicked()
    {
        Debug.Log("Build Button");

        if (buildMenu.style.display == DisplayStyle.None)
            buildMenu.style.display = DisplayStyle.Flex;
        else
            buildMenu.style.display = DisplayStyle.None;
    }

    private void OnBasicTowerButtonClicked()
    {
        Debug.Log("Basic-Tower-Button");
        buildMenu.style.display = DisplayStyle.None;

        waitingToPlaceTower = true;
        gridSystem.StartPlacing();
    }

    public void WaitingToPlaceTower(bool placedTower) 
    {
        if (placedTower)
            waitingToPlaceTower = true;

        waitingToPlaceTower = false;
    }

    public bool PlacedTower() 
    {
        if (waitingToPlaceTower)
            return false;

        return true;
    }
}
