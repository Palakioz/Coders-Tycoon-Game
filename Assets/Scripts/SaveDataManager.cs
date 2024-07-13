using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField] private ScoreManager ScoreManager; // Referencia al ScoreManager para poder acceder a las variables dentro de �l
    [SerializeField] private UpgradeManager UpgradeManagerMouse;
    [SerializeField] private UpgradeManager UpgradeManagerCriptoGranja;
    [SerializeField] private UpgradeManager UpgradeManagerGPU;
    [SerializeField] private UpgradeManager UpgradeManagerGPT;
    [SerializeField] private StarUpgradeManager UpgradeManagerChair;
    [SerializeField] private StarUpgradeManager UpgradeManagerMonitor;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        SaveData();
    //    }
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        LoadData();
    //    }

    //}

    private void Start()
    {
        if (PlayerPrefs.HasKey("id_user"))
        {
            LoadData();
        }
    }

    public void SaveData()
    {
        GameProgressData gameProgressData = new GameProgressData
        {
            neoCoins = ScoreManager.neoCoins,
            autoClickValue = ScoreManager.autoClickValue,
            clickPowerValue = ScoreManager.clickValue,
            neoStars = ScoreManager.neoStars,
            realAutoClickValue = ScoreManager.realAutoClickValue,
            realClickPowerValue = ScoreManager.realClickValue,
            totalCoinsEarned = ScoreManager.totalCoinsEarned,
            totalStarsEaenred = ScoreManager.totalStarsEarned,

            upgradeCostMouse = UpgradeManagerMouse.upgradeCost,
            upgradeIncrementMouse = UpgradeManagerMouse.upgradeIncrement,
            upgradeLevelMouse = UpgradeManagerMouse.upgradeLevel,

            upgradeCostCriptoGranja = UpgradeManagerCriptoGranja.upgradeCost,
            upgradeIncrementCriptoGranja = UpgradeManagerCriptoGranja.upgradeIncrement,
            upgradeLevelCriptoGranja = UpgradeManagerCriptoGranja.upgradeLevel,

            upgradeCostGPU = UpgradeManagerGPU.upgradeCost,
            upgradeIncrementGPU = UpgradeManagerGPU.upgradeIncrement,
            upgradeLevelGPU = UpgradeManagerGPU.upgradeLevel,

            upgradeCostGPT = UpgradeManagerGPT.upgradeCost,
            upgradeIncrementGPT = UpgradeManagerGPT.upgradeIncrement,
            upgradeLevelGPT = UpgradeManagerGPT.upgradeLevel,

            upgradeLevelChair = UpgradeManagerChair.upgradeLevel,
            upgradeLevelMonitor = UpgradeManagerMonitor.upgradeLevel
        };

        dataPutRequest dataPutRequest = new dataPutRequest
        {
            jsonSaveData = JsonUtility.ToJson(gameProgressData),
            id_user = int.Parse(PlayerPrefs.GetString("id_user")),
            coins = (int)ScoreManager.totalCoinsEarned,
            stars = (int)ScoreManager.totalStarsEarned
        };

        string jsonData = JsonUtility.ToJson(dataPutRequest);

        StartCoroutine(PutSaveData(PlayerPrefs.GetString("back_end_url") + "/save-data", jsonData));

    }

    private IEnumerator PutSaveData(string uri, string jsonData)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Put(uri, jsonData);
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error guardando datos de guardado: {webRequest.error}");
        }
        else
        {
            Debug.Log("Data saved successfully");
        }
    }

    public void LoadData()
    {
        StartCoroutine(GetSaveData(PlayerPrefs.GetString("id_user")));
    }

    private IEnumerator GetSaveData(string id_user)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(PlayerPrefs.GetString("back_end_url") + "/save-data/" + id_user))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(String.Format("Error al obtener save data: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Data loaded successfully!");
                    string jsonSaveData = webRequest.downloadHandler.text;
                    jsonSaveData = jsonSaveData.Substring(1, jsonSaveData.Length - 2);

                    JObject jsonData = JObject.Parse(jsonSaveData);
                    string datos_guardado = jsonData["datos_guardado"].ToString();

                    GameProgressData saveData = JsonConvert.DeserializeObject<GameProgressData>(datos_guardado);

                    ScoreManager.neoCoins = saveData.neoCoins;
                    ScoreManager.autoClickValue = saveData.realAutoClickValue;
                    ScoreManager.clickValue = saveData.realClickPowerValue;
                    ScoreManager.realAutoClickValue = saveData.realAutoClickValue;
                    ScoreManager.realClickValue = saveData.realClickPowerValue;
                    ScoreManager.totalCoinsEarned = saveData.totalCoinsEarned;
                    ScoreManager.totalStarsEarned = saveData.totalStarsEaenred;

                    UpgradeManagerMouse.upgradeCost = saveData.upgradeCostMouse;
                    UpgradeManagerMouse.upgradeIncrement = saveData.upgradeIncrementMouse;
                    UpgradeManagerMouse.upgradeLevel = saveData.upgradeLevelMouse;

                    UpgradeManagerCriptoGranja.upgradeCost = saveData.upgradeCostCriptoGranja;
                    UpgradeManagerCriptoGranja.upgradeIncrement = saveData.upgradeIncrementCriptoGranja;
                    UpgradeManagerCriptoGranja.upgradeLevel = saveData.upgradeLevelCriptoGranja;

                    UpgradeManagerGPU.upgradeCost = saveData.upgradeCostGPU;
                    UpgradeManagerGPU.upgradeIncrement = saveData.upgradeIncrementGPU;
                    UpgradeManagerGPU.upgradeLevel = saveData.upgradeLevelGPU;

                    UpgradeManagerGPT.upgradeCost = saveData.upgradeCostGPT;
                    UpgradeManagerGPT.upgradeIncrement = saveData.upgradeIncrementGPT;
                    UpgradeManagerGPT.upgradeLevel = saveData.upgradeLevelGPT;

                    ScoreManager.neoStars = (float)(+ saveData.upgradeLevelChair * 2);
                    ScoreManager.neoStars = (float)(+ saveData.upgradeLevelMonitor * 2);

                    for (int i = 0; i < saveData.upgradeLevelChair; i++)
                    {
                        UpgradeManagerChair.BuyUpgrade();
                    }

                    for (int i = 0; i < saveData.upgradeLevelMonitor; i++)
                    {
                        UpgradeManagerMonitor.BuyUpgrade();
                    }

                    ScoreManager.neoStars = saveData.neoStars;

                    break;


            }
        }
    }

    private class GameProgressData
    {
        // Valores del score
        public float neoCoins;
        public float autoClickValue;
        public float clickPowerValue;
        public float neoStars;
        public float realAutoClickValue;
        public float realClickPowerValue;
        public float totalCoinsEarned;
        public float totalStarsEaenred;

        // Valores de la tienda de mejoras
        public int upgradeCostMouse;
        public float upgradeIncrementMouse;
        public int upgradeLevelMouse;

        public int upgradeCostCriptoGranja;
        public float upgradeIncrementCriptoGranja;
        public int upgradeLevelCriptoGranja;

        public int upgradeCostGPU;
        public float upgradeIncrementGPU;
        public int upgradeLevelGPU;

        public int upgradeCostGPT;
        public float upgradeIncrementGPT;
        public int upgradeLevelGPT;

        public int upgradeLevelChair;
        public int upgradeLevelMonitor;
    }

    private class dataPutRequest
    {
        public string jsonSaveData;
        public int id_user;
        public int coins;
        public int stars;
    }

}