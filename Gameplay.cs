using System.Collections;
using System.Collections.Generic;
using System.IO; 
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;


public class Gameplay : MonoBehaviour

{

    public RawImage pokeRawImage; //creating UI Elements
    public TextMeshProUGUI playerScoreText, PotentialRoundScoreText, pokemonNameText;
    public TextMeshProUGUI hint1Button, hint2Button, revealPokemonButton, playerGuessTextBox;
    public TextMeshProUGUI pokemonAbilityText, pokemonTypeText;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    // Start is called before the first frame update

    void Start()

    {

        pokeRawImage.texture = Texture2D.blackTexture;
        playerScoreText.text = "0";
        PotentialRoundScoreText.text = "1000";
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
            File.WriteAllBytes(@"C:\Users\gohan\New Unity Project\Assets\Images\Pokemon Sprites\" + randomPokeIndex + ".png", pokeOriginalBytes);

            pokeRawImage.texture = pokeOriginalImage;
            pokeRawImage.texture.filterMode = FilterMode.Point;
                        
            pokeRawImage.color = Color.black;
                    
            pokemonNameText.text = CapitalizeFirstLetter(pokemonName);
            pokemonAbilityText.text = "Ability: " + CapitalizeFirstLetter(pokemonAbilityNames[0]);
            pokemonTypeText.text = "Type: " + CapitalizeFirstLetter(pokemonTypeNames[0]);
        }

        string CapitalizeFirstLetter(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}