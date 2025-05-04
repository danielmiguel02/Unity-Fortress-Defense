using UnityEngine;
using UnityEngine.UI; // Use this instead of UIElements
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System.Runtime.InteropServices;

public class GameUI : MonoBehaviour
{
    private Button buildButton;
    private Button basicTowerButton;
    private Button mediumTowerButton;
    private Button strongTowerButton;
    private Button fortressTowerButton;
    private Button playButton;
    private Image waveIndicatorImage;
    private TextMeshProUGUI waveIndicatorText;
    private Image towerKeys;
    private Canvas buildMenu;

    private TextMeshProUGUI healthBasicTower;
    private TextMeshProUGUI damageBasicTower;
    private TextMeshProUGUI rangeBasicTower;

    private TextMeshProUGUI healthMediumTower;
    private TextMeshProUGUI damageMediumTower;
    private TextMeshProUGUI rangeMediumTower;

    private TextMeshProUGUI healthStrongTower;
    private TextMeshProUGUI damageStrongTower;
    private TextMeshProUGUI rangeStrongTower;

    private TextMeshProUGUI healthFortressTower;
    private TextMeshProUGUI damageFortressTower;
    private TextMeshProUGUI rangeFortressTower;

    [SerializeField] private GameObject basicTowerObject;
    [SerializeField] private GameObject mediumTowerObject;
    [SerializeField] private GameObject strongTowerObject;
    [SerializeField] private GameObject fortressTowerObject;
    [SerializeField] private TowersScript towersScript;
    [SerializeField] private GridSystem gridSystem;

    private CoinsSystem coinsSystem;
    [SerializeField] private WaveSystem waveSystem;

    public Dictionary<string, TowerStats> towerTemplates;

    public bool waitingToPlaceTower { get; set; } = false;
    public string towerSelected {  get; set; }

    float timer = 0f;

    private void OnEnable()
    {
        // Find buttons by name in the scene or better yet assign via inspector
        buildButton = GameObject.Find("Build-Button").GetComponent<Button>();
        buildMenu = GameObject.Find("Towers-Store").GetComponent<Canvas>();
        basicTowerButton = GameObject.Find("BasicTowerButton").GetComponent<Button>();
        mediumTowerButton = GameObject.Find("MediumTowerButton").GetComponent<Button>();
        strongTowerButton = GameObject.Find("StrongTowerButton").GetComponent<Button>();
        fortressTowerButton = GameObject.Find("FortressTowerButton").GetComponent<Button>();
        playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        waveIndicatorImage = GameObject.Find("WaveIndicator").GetComponent<Image>();
        waveIndicatorText = GameObject.Find("WaveIndicatorText").GetComponent<TextMeshProUGUI>();
        towerKeys = GameObject.Find("Tower-Keys").GetComponent<Image>();

        healthBasicTower = GameObject.Find("healthBasicTower").GetComponent<TextMeshProUGUI>();
        damageBasicTower = GameObject.Find("damageBasicTower").GetComponent<TextMeshProUGUI>();
        rangeBasicTower = GameObject.Find("rangeBasicTower").GetComponent<TextMeshProUGUI>();

        healthMediumTower = GameObject.Find("healthMediumTower").GetComponent<TextMeshProUGUI>();
        damageMediumTower = GameObject.Find("damageMediumTower").GetComponent<TextMeshProUGUI>();
        rangeMediumTower = GameObject.Find("rangeMediumTower").GetComponent<TextMeshProUGUI>();

        healthStrongTower = GameObject.Find("healthStrongTower").GetComponent<TextMeshProUGUI>();
        damageStrongTower = GameObject.Find("damageStrongTower").GetComponent<TextMeshProUGUI>();
        rangeStrongTower = GameObject.Find("rangeStrongTower").GetComponent<TextMeshProUGUI>();

        healthFortressTower = GameObject.Find("healthFortressTower").GetComponent<TextMeshProUGUI>();
        damageFortressTower = GameObject.Find("damageFortressTower").GetComponent<TextMeshProUGUI>();
        rangeFortressTower = GameObject.Find("rangeFortressTower").GetComponent<TextMeshProUGUI>();

        coinsSystem = GetComponent<CoinsSystem>();

        buildButton.onClick.AddListener(OnBuildButtonClicked);
        basicTowerButton.onClick.AddListener(OnBasicTowerButtonClicked);
        mediumTowerButton.onClick.AddListener(OnMediumTowerButtonClicked);
        strongTowerButton.onClick.AddListener(OnStrongTowerButtonClicked);
        fortressTowerButton.onClick.AddListener(OnFortressTowerButtonClicked);
        playButton.onClick.AddListener(PlayButtonInteraction);

        buildButton.gameObject.SetActive(false);
        buildMenu.enabled = false;
        towerKeys.enabled = false;
        playButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (towersScript.towerTemplates.TryGetValue("BasicTower", out TowerStats stats))
        {
            healthBasicTower.text = $"{stats.health:F1}";
            damageBasicTower.text = $"{stats.damage:F1}";
            rangeBasicTower.text = $"{stats.range:F1}";
        }
        if (towersScript.towerTemplates.TryGetValue("MediumTower", out stats))
        {
            healthMediumTower.text = $"{stats.health:F1}";
            damageMediumTower.text = $"{stats.damage:F1}";
            rangeMediumTower.text = $"{stats.range:F1}";
        }
        if (towersScript.towerTemplates.TryGetValue("StrongTower", out stats))
        {
            healthStrongTower.text = $"{stats.health:F1}";
            damageStrongTower.text = $"{stats.damage:F1}";
            rangeStrongTower.text = $"{stats.range:F1}";
        }
        if (towersScript.towerTemplates.TryGetValue("FortressTower", out stats))
        {
            healthFortressTower.text = $"{stats.health:F1}";
            damageFortressTower.text = $"{stats.damage:F1}";
            rangeFortressTower.text = $"{stats.range:F1}";
        }
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

        
        if (timer < 6f) 
        {
            timer += Time.deltaTime % 6f;
        }

        if (timer <= 6f)
        {
            waveIndicatorImage.enabled = true;
            waveIndicatorText.enabled = true;
        }
        else
        {
            waveIndicatorImage.enabled = false;
            waveIndicatorText.enabled = false;
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
        if (coinsSystem.currentPlayerCoins >= coinsSystem.basicTowerCost)
        {
            towerSelected = "basicTower";
            gridSystem.ObjectToPlace = "BasicTower";

            Debug.Log("BasicTowerButton Clicked");
            buildMenu.enabled = false;
            towerKeys.enabled = true;

            waitingToPlaceTower = true;
            gridSystem.StartPlacing();
        }
        else 
        {
            Debug.Log("Not enough coins");
        }
    }
    private void OnMediumTowerButtonClicked()
    {
        if (coinsSystem.currentPlayerCoins >= coinsSystem.mediumTowerCost)
        {
            towerSelected = "mediumTower";
            gridSystem.ObjectToPlace = "MediumTower";

            Debug.Log("MediumTowerButton Clicked");
            buildMenu.enabled = false;
            towerKeys.enabled = true;

            waitingToPlaceTower = true;
            gridSystem.StartPlacing();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }
    private void OnStrongTowerButtonClicked()
    {
        if (coinsSystem.currentPlayerCoins >= coinsSystem.strongTowerCost)
        {
            towerSelected = "strongTower";
            gridSystem.ObjectToPlace = "StrongTower";

            Debug.Log("StrongTowerButton Clicked");
            buildMenu.enabled = false;
            towerKeys.enabled = true;

            waitingToPlaceTower = true;
            gridSystem.StartPlacing();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }
    private void OnFortressTowerButtonClicked()
    {
        if (coinsSystem.currentPlayerCoins >= coinsSystem.fortressTowerCost)
        {
            towerSelected = "fortressTower";
            gridSystem.ObjectToPlace = "FortressTower";

            Debug.Log("FortressTowerButton Clicked");
            buildMenu.enabled = false;
            towerKeys.enabled = true;

            waitingToPlaceTower = true;
            gridSystem.StartPlacing();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

    public bool PlacedTower()
    {
        return !waitingToPlaceTower;
    }

    // When wave is going desable everything from shop
    public void DisableShop() 
    { 
        buildMenu.enabled = false;
        buildButton.gameObject.SetActive(false);
        gridSystem.DisableGhostObject();
        waitingToPlaceTower = false;
        towerKeys.enabled = false;
    }

    public void PlayButtonInteraction() 
    {
        waveSystem.PlayNextRound();
        playButton.gameObject.SetActive(false);
        waveIndicatorText.text = waveSystem.wave + "/" + waveSystem.waveMax;

        timer = 0;
    }

    public void PlayButtonDisplay() 
    {
        playButton.gameObject.SetActive(true);
    }

    public void StoreButtonDisplay() 
    {
        buildButton.gameObject.SetActive(true);
    }
}
