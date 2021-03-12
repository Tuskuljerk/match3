using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameOver = false;

     private void Awake()
     {
         if (instance == null)
         {
             DontDestroyOnLoad(gameObject);
             instance = GetComponent<GameManager>();
         }
         else
         {
             Destroy(gameObject);
         }

       //soundToggle = FindObjectOfType<Toggle>();
     }

    
}
