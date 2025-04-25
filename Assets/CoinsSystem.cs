using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsSystem : MonoBehaviour
{
    private List<TextMeshProUGUI> coinsPlayerTextList;
    private TextMeshProUGUI costBasicTowerText;
    private TextMeshProUGUI costMediumTowerText;
    private TextMeshProUGUI costStrongTowerText;
    private TextMeshProUGUI costFortressTowerText;

    public int currentPlayerCoins { get; set; } = 800;

    private int basicTowerCost = 100;
    private int mediumTowerCost = 200;
    private int strongTowerCost = 300;
    private int fortressTowerCost = 400;


    private void Start()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("CoinsPlayer");

        costBasicTowerText = GameObject.Find("costBasicTower").GetComponent<TextMeshProUGUI>();
        costMediumTowerText = GameObject.Find("costMediumTower").GetComponent<TextMeshProUGUI>();
        costStrongTowerText = GameObject.Find("costStrongTower").GetComponent<TextMeshProUGUI>();
        costFortressTowerText = GameObject.Find("costFortressTower").GetComponent<TextMeshProUGUI>();

        costBasicTowerText.text = basicTowerCost.ToString();
        costMediumTowerText.text = mediumTowerCost.ToString();
        costStrongTowerText.text = strongTowerCost.ToString();
        costFortressTowerText.text = fortressTowerCost.ToString();

        coinsPlayerTextList = new List<TextMeshProUGUI>();
        foreach (GameObject coinsP in objectsWithTag) 
        {
            TextMeshProUGUI tmp = coinsP.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                coinsPlayerTextList.Add(tmp);
        }

        UpdateCoins();
    }

    private void Update()
    {
    }

    public void UpdateCoins() 
    {
        foreach (TextMeshProUGUI tmp in coinsPlayerTextList)
        {
            tmp.text = currentPlayerCoins.ToString();
        }
    }

    public void BuyTower(string towerType) 
    {
        if (currentPlayerCoins >= basicTowerCost && towerType == "basicTower")
            currentPlayerCoins -= basicTowerCost;
        else if (currentPlayerCoins >= mediumTowerCost && towerType == "mediumTower")
            currentPlayerCoins -= mediumTowerCost;
        else if (currentPlayerCoins >= strongTowerCost && towerType == "strongTower")
            currentPlayerCoins -= strongTowerCost;
        else if (currentPlayerCoins >= fortressTowerCost && towerType == "fortressTower")
            currentPlayerCoins -= fortressTowerCost;
        else
            Debug.Log("Not enought coins to buy " + towerType);

            UpdateCoins();
    }

    public int TowerCost(string towerType)
    {
        if (towerType == "basicTower")
            return basicTowerCost;
        else if (towerType == "mediumTower")
            return mediumTowerCost;
        else if (towerType == "strongTower")
            return strongTowerCost;
        else if (towerType == "fortressTower")
            return fortressTowerCost;

        return 0;
    }
}
