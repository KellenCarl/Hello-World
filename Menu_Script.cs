using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu_Script : MonoBehaviour
{

    //creating UI Elements
    public TextMeshProUGUI highScoreNumber, yourScoreNumber;
    public InputField playerRoundsInput;
    int highScore = 0;
    int yourScore = 0;
    int numberofRounds;
    string userRounds;

    



    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefLoad();
        highScoreNumber.text = "" + highScore;
        yourScoreNumber.text = "" + yourScore;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            PlayerPrefs.SetInt("Score", 0);
            Application.Quit();
        }


    }

    public void PlayerPrefLoad()
    {
        if (PlayerPrefs.HasKey("High Score"))
            { 
            highScore = PlayerPrefs.GetInt("High Score");
        }
        if (PlayerPrefs.HasKey("Score"))
        {
            yourScore = PlayerPrefs.GetInt("Score");
        }
        
    }

    public void LoadNextPokemon()
    {
        SceneManager.LoadScene(1);
    }

    public void OnEnterTextBox()
    {

        userRounds = playerRoundsInput.text;

        if (int.TryParse(userRounds, out numberofRounds))
        {

            if (0 < numberofRounds && numberofRounds < 11)
            {
                PlayerPrefs.SetInt("Rounds Remaining", numberofRounds);
                LoadNextPokemon();

            }


        }
        else
        {
            Debug.Log("Not a valid int");
        }
        

        


    }

}
