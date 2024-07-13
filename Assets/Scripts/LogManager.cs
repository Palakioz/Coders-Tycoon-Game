using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LogManager : MonoBehaviour
{
    [SerializeField] private SaveDataManager saveDataManager;

    public void setUser(string id_user)
    {
        PlayerPrefs.SetString("id_user", id_user);
    }

    public void setBackEndUrl(string url)
    {
        PlayerPrefs.SetString("back_end_url", url);
    }

    [Obsolete]
    public void logConexion()
    {
        StartCoroutine(postLog(PlayerPrefs.GetString("id_user"), "conexion"));
    }

    [Obsolete]
    public void logDesconexion()
    {
        StartCoroutine(postLog(PlayerPrefs.GetString("id_user"), "desconexion"));
    }

    [Obsolete]
    public void logIn(string userDataJson)
    {
        UserData userData = JsonUtility.FromJson<UserData>(userDataJson);
        setUser(userData.usuario_id);
        setBackEndUrl(userData.backend_url);
        logConexion();
        SceneManager.LoadScene("MainScene");
    }

    [Obsolete]
    public void logOut()
    {
        logDesconexion();
        saveDataManager.SaveData();
        SceneManager.LoadScene("WelcomeScene");
    }

    //[Obsolete]
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        logIn("2");
    //    }
    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        logOut();
    //    }
    //}

    [Obsolete]
    private IEnumerator postLog(string id_user, string connection_type)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(PlayerPrefs.GetString("back_end_url") + "/log-conexion/" + id_user + "/" + connection_type, " "))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(String.Format("Error al hacer log: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Log made successfully!");
                    break;
            }
        }
    }

    [System.Serializable]
    public class UserData
    {
        public string usuario_id;
        public string backend_url;
    }
}
