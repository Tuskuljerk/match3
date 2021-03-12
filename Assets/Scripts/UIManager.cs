using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public Text yourScoreTxt;
    public Text scoreTxt;
    public Text moveCounterTxt;

    public Toggle soundToggle;



    private int _score, _moveCounter;

    void Awake()
    {
        instance = GetComponent<UIManager>();
        _moveCounter = 35;
        moveCounterTxt.text = _moveCounter.ToString();
    }

    // Show the game over panel
    public void GameOver()
    {
        GameManager.instance.gameOver = true;
        gameOverPanel.SetActive(true);

        yourScoreTxt.text = _score.ToString();
    }



    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            scoreTxt.text = _score.ToString();
        }
    }

    public int MoveCounter
    {
        get
        {
            return _moveCounter;
        }

        set
        {
            _moveCounter = value;
            if (_moveCounter <= 0)
            {
                _moveCounter = 0;
                StartCoroutine(WaitForShifting());
                
            }
            moveCounterTxt.text = _moveCounter.ToString();
        }
    }

    private IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
        yield return new WaitForSeconds(2f);
        GameOver();
    }

 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void Unpause()
    {
        pausePanel.SetActive(false);
    }
    public void Pause()
    {
        pausePanel.SetActive(true);
    }

    public void SoundButtonOnOFF()
    {
        soundToggle = FindObjectOfType<Toggle>();
        if (soundToggle.isOn == true)
        {
            AudioListener.volume = 1;
            PlayerPrefs.SetInt("Audio", 1);
        }
        if (soundToggle.isOn == false)
        {
            AudioListener.volume = 0;
            PlayerPrefs.SetInt("Audio", 0);
        }
    }
}
