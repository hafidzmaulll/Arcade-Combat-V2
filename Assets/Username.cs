using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using Photon.Pun;

public class Username : MonoBehaviour
{
    public TMP_InputField inputField; // Updated to TMP_InputField
    public GameObject UsernamePage;
    public TextMeshProUGUI MyUsername; // Updated to TextMeshProUGUI

    // Start is called before the first frame update
    void Start()
    {
        string savedUsername = PlayerPrefs.GetString("Username");

        if (string.IsNullOrEmpty(savedUsername))
        {
            UsernamePage.SetActive(true);
        }
        else
        {
            PhotonNetwork.NickName = savedUsername;
            MyUsername.text = savedUsername;
            inputField.text = savedUsername; // Populate the input field with the saved username
            UsernamePage.SetActive(false);
        }
    }

    public void SaveUsername()
    {
        string newUsername = inputField.text;

        if (!string.IsNullOrEmpty(newUsername))
        {
            PhotonNetwork.NickName = newUsername;
            PlayerPrefs.SetString("Username", newUsername);
            MyUsername.text = newUsername;
            UsernamePage.SetActive(false); // Hide the username page after saving
        }
    }

    // Optional: Method to open the username page to allow changes
    public void OpenUsernamePage()
    {
        UsernamePage.SetActive(true);
    }
}
