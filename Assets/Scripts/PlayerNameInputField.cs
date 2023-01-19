using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerNameInputField : MonoBehaviour
{

    const string playerNamePrefKey="PlayerName";
    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        InputField inputField = gameObject.GetComponent<InputField>();
        if(inputField!=null)
        {
            if(PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                inputField.text = defaultName;
            }
        }
        PhotonNetwork.NickName = defaultName;
    }
    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty!");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
