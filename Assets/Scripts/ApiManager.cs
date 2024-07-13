using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System;

public class ApiManager : MonoBehaviour
{
    [SerializeField] private ScoreManager ScoreManager; // Referencia al ScoreManager para poder acceder a las neo stars en caso de ser agregadas
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);

    public class Usuario
    {
        public string nombre_usuario { get; set; }
        public int neo_coins { get; set; }
        public int neo_stars { get; set; }
        public int puntuacion { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Curso
    {
        public string nombre_curso { get; set; }
        public int estrellas_recompensa { get; set; }
        public int porcentaje_progreso { get; set; }
        public bool estrellas_reclamadas { get; set; }
    }


    public GameObject fila_usuario_leaderboard;
    public GameObject fila_curso;
    public GameObject leaderboard;
    public GameObject courses;

    public void GetLeaderboard()
    {        
        StartCoroutine(GetLeaderboard(PlayerPrefs.GetString("back_end_url") + "/leaderboard"));
    }
    
    public void GetCourses()
    {
        StartCoroutine(GetCourses(PlayerPrefs.GetString("back_end_url") + "/cursos/" + PlayerPrefs.GetString("id_user")));
    }

    public void DestroyChildren(string tag)
    {
        foreach (Transform child in GameObject.FindGameObjectWithTag(tag).transform)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator GetLeaderboard(String uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(String.Format("Error al obtener leadeboard: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    try
                    {
                        DestroyChildren("Filas Leaderboard");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                    Debug.Log(webRequest.downloadHandler.text);
                    List<Usuario> usuarios = JsonConvert.DeserializeObject<List<Usuario>>(webRequest.downloadHandler.text);
                    int rank = 1;
                    foreach (Usuario usuario in usuarios)
                    {
                        GameObject nueva_fila = Instantiate(fila_usuario_leaderboard, transform);
                        nueva_fila.transform.SetParent(GameObject.FindGameObjectWithTag("Filas Leaderboard").transform, false);

                        TMP_Text[] textos = nueva_fila.GetComponentsInChildren<TMP_Text>();

                        textos[0].text = rank + ". " + usuario.nombre_usuario;
                        textos[1].text = usuario.neo_coins.ToString();
                        textos[2].text = usuario.neo_stars.ToString();
                        textos[3].text = usuario.puntuacion.ToString();
                        rank++;
                    }
                    break;
            }
        }
    }

    IEnumerator GetCourses(String uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(String.Format("Error al obtener cursos del usuario: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    try
                    {
                        DestroyChildren("Filas Cursos");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                    Debug.Log(webRequest.downloadHandler.text);
                    List<Curso> cursos = JsonConvert.DeserializeObject<List<Curso>>(webRequest.downloadHandler.text);

                    foreach (Curso curso in cursos)
                    {
                        GameObject nueva_fila = Instantiate(fila_curso, transform);
                        nueva_fila.transform.SetParent(GameObject.FindGameObjectWithTag("Filas Cursos").transform, false);

                        TMP_Text[] textos = nueva_fila.GetComponentsInChildren<TMP_Text>();

                        textos[0].text = curso.nombre_curso;
                        textos[1].text = curso.porcentaje_progreso.ToString();
                        textos[2].text = curso.estrellas_recompensa.ToString();

                        if (curso.porcentaje_progreso == 100 && !curso.estrellas_reclamadas)
                        {
                            textos[3].text = "<sprite name=checkmark>";
                            textos[3].color = Color.green;
                            ScoreManager.neoStars += curso.estrellas_recompensa;
                            ScoreManager.totalStarsEarned += curso.estrellas_recompensa;

                            dataPutRequest dataPutRequest = new dataPutRequest
                            {
                                id_user = int.Parse(PlayerPrefs.GetString("id_user")),
                                course_name = curso.nombre_curso
                            };
                            // codigo para acutalizar las estrellsa como reclamadas dentro de la base de datos (PUT)
                            string jsonData = JsonUtility.ToJson(dataPutRequest);
                            StartCoroutine(putClaimedStars(jsonData));
                        }
                        else if (curso.porcentaje_progreso < 100)
                        {
                            textos[3].text = "X";
                            textos[3].color = Color.red;
                        }
                        else
                        {
                            textos[3].text = "<sprite name=checkmark>";
                            textos[3].color = Color.green;
                        }

                    }
                    break;
                     
            }
        }
    }

    IEnumerator putClaimedStars(string request_body)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Put(PlayerPrefs.GetString("back_end_url") + "/actualizar-estrellas-reclamadas", request_body);
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error guardando datos de guardado: {webRequest.error}");
        }
        else
        {
            Debug.Log("Claimed stars updated successfully");
        }
    }

    private class dataPutRequest
    {
        public int id_user;
        public string course_name;
    }

}
