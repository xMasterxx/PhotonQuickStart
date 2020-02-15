using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

/// <summary>
/// Player name input field. Let the user input his name, will appear above the player in the game.
/// </summary>
[RequireComponent(typeof(InputField))]
public class PlayerNameInput : MonoBehaviour
{
    // Store the PlayerPref Key to avoid typos
    private const string PLAYER_NAME = "PlayerName";

    private void Start()
    {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(PLAYER_NAME))
            {
                defaultName = PlayerPrefs.GetString(PLAYER_NAME);
                _inputField.text = defaultName;
            }
        }
        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;


        PlayerPrefs.SetString(PLAYER_NAME, value);
    }

}


