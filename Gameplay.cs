using System.Collections;
using System.Collections.Generic;
using System.IO; 
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Gameplay : MonoBehaviour

{

    public RawImage pokeRawImage; //creating UI Elements
    public TextMeshProUGUI playerScoreText, PotentialRoundScoreText, pokemonNameText;
    public TextMeshProUGUI hint1Button, hint2Button, revealPokemonButton;
    public InputField playerGuessTextBox;
    public TextMeshProUGUI pokemonAbilityText, pokemonTypeText;
    bool pokemonRevealed = false;
    bool hint1Revealed = false;
    bool hint2Revealed = false;
    bool correctGuess = false;
    string pokemonType = "", pokemonAbility = "", g_pokemonName = "";
    string playerGuess = "";
    int playerscoreNum = 0;
    int potentialroundscoreNum = 1000;
    int roundsRemaining = 0;
    int highScore = 0;
    
        



    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    // Start is called before the first frame update

    void Start()

    {
        PlayerPrefLoad();
        pokeRawImage.texture = Texture2D.blackTexture;  // set initial Pokemon Sprite Spot as blank canvas spot
        playerScoreText.text = "" + playerscoreNum;   // setting all interface displays equal to initial values (most of them blank until revealed)
        PotentialRoundScoreText.text = "" + potentialroundscoreNum;
        pokemonNameText.text = "";
        pokemonAbilityText.text = "";
        pokemonTypeText.text = "";


        int randomPokeIndex = Random.Range(1, 808); // Pick Random Pokemon id. Min: inclusive, Max Exclusive

        StartCoroutine(GetPokemonAtIndex(randomPokeIndex));

        // Get Pokemon Info from Pokemon API

        IEnumerator GetPokemonAtIndex(int pokemonIndex)

        {
            string PokemonURL = basePokeURL + "pokemon/" + pokemonIndex.ToString();
            UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(PokemonURL);
            yield return pokeInfoRequest.SendWebRequest();

            if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
            {
                Debug.LogError(pokeInfoRequest.error);
                yield break;
            }

            JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);
            string pokemonName = pokeInfo["name"];
            string pokemonSpriteURL = pokeInfo["sprites"]["front_default"];

            JSONNode pokemonTypes = pokeInfo["types"];
            string[] pokemonTypeNames = new string[pokemonTypes.Count];

            for (int i = 0, j = pokemonTypes.Count - 1; i < pokemonTypes.Count; i++, j--)
            {
                pokemonTypeNames[j] = pokemonTypes[i]["type"]["name"];
            }

            JSONNode pokemonAbilities = pokeInfo["abilities"];
            string[] pokemonAbilityNames = new string[pokemonAbilities.Count];

            for (int i = 0, j = pokemonAbilities.Count - 1; i < pokemonAbilities.Count; i++, j--)
            {
                pokemonAbilityNames[j] = pokemonAbilities[i]["ability"]["name"];
            }

            // Get Pokemon Sprite
            UnityWebRequest pokemonSpriteRequest = UnityWebRequestTexture.GetTexture(pokemonSpriteURL);
            yield return pokemonSpriteRequest.SendWebRequest();

            if (pokemonSpriteRequest.isNetworkError || pokemonSpriteRequest.isHttpError)
            {
                Debug.LogError(pokemonSpriteRequest.error);
                yield break;
            }

            Texture2D pokeOriginalImage = DownloadHandlerTexture.GetContent(pokemonSpriteRequest);
            byte[] pokeOriginalBytes = pokeOriginalImage.EncodeToPNG();
            // comment out to write to file.  File.WriteAllBytes(@"C:\Users\gohan\WhosThatPokemon\Assets\Images\Pokemon Sprites\" + randomPokeIndex + ".png", pokeOriginalBytes);

            pokeRawImage.texture = pokeOriginalImage;
            pokeRawImage.texture.filterMode = FilterMode.Point;
            pokeRawImage.material.SetColor("_Color", Color.black); // set silhouette for Pokemon Guess

            string CapitalizeFirstLetter(string str)  //method for Capitalizing First letter in string name
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            g_pokemonName = CapitalizeFirstLetter(pokemonName);
            pokemonAbility = "Ability: " + CapitalizeFirstLetter(pokemonAbilityNames[0]);
            pokemonType = "Type: " + CapitalizeFirstLetter(pokemonTypeNames[0]);
        }

        

    }

    public void OnEnterTextBox()
    {
        playerGuess = playerGuessTextBox.text;

        if (playerGuess == g_pokemonName && correctGuess == false)
        {
            playerscoreNum += potentialroundscoreNum;
            playerScoreText.text = "" + playerscoreNum;
            RevealName();
            correctGuess = true;

        }

        

    }

    public void OnButtonNextPokemon()
    {
        RevealName();
        PlayerPrefs.SetInt("Score", playerscoreNum);
        SetHighScore();
        roundsRemaining = (roundsRemaining - 1);
        PlayerPrefs.SetInt("Rounds Remaining", roundsRemaining);
        if (roundsRemaining == 0)
        {
            LoadMenu();
        }
        else
        {
            LoadNextPokemon();
        }
    }

    private void RevealName()
    {
        pokemonNameText.text = g_pokemonName;
        
    }

    public void OnButtonHint1()
    {
       if (hint1Revealed == false)
        {
            potentialroundscoreNum = potentialroundscoreNum - 200;
            PotentialRoundScoreText.text = "" + potentialroundscoreNum;
            RevealHint1();
            hint1Revealed = true;
        }
        
    }

    private void RevealHint1()
    {
        pokemonTypeText.text = pokemonType;

    }

    public void OnButtonHint2()
    {
        if (hint2Revealed == false)
        {
            potentialroundscoreNum = potentialroundscoreNum - 200;
            PotentialRoundScoreText.text = "" + potentialroundscoreNum;
            RevealHint2();
            hint2Revealed = true;
        }
    }

    private void RevealHint2()
    {
        pokemonAbilityText.text = pokemonAbility;
    }


    public void OnButtonRevealPokemon()
    {
        if (pokemonRevealed == false)
        {
            potentialroundscoreNum = potentialroundscoreNum - 500;
            PotentialRoundScoreText.text = "" + potentialroundscoreNum;
            RevealPokemon();
            pokemonRevealed = true;
        }
        
    }

    private void RevealPokemon()
    {
        pokemonRevealed = true;
        pokeRawImage.material.SetColor("_Color", Color.white);
    }

    public void PlayerPrefLoad()
    {
        playerscoreNum = PlayerPrefs.GetInt("Score");
        roundsRemaining = PlayerPrefs.GetInt("Rounds Remaining");
        
    }

    public void LoadNextPokemon()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void SetHighScore()
    {
        if (PlayerPrefs.HasKey("High Score"))
        {
            highScore = PlayerPrefs.GetInt("High Score");
        }
        if (playerscoreNum > highScore)
        {
            PlayerPrefs.SetInt("High Score", playerscoreNum);
        }

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

}