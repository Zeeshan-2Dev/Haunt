using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;  // Make sure you've imported the Facebook SDK
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public GameObject loginPanel; // Panel for login UI
    public GameObject playerInfoText; // Player info UI text
    public Button startButton; // Start game button
    public Button logoutButton; // Logout button
    public GameObject guestLoginPanel; // Panel for guest login
    public InputField playerNameInput; // Input field for player name
    public Button guestLoginButton; // Button to login as guest
    public Button confirmButton; // Button to confirm name input
    public Button facebookLoginButton; // Facebook login button
    public Text feedbackText; // Text to show feedback on name availability

    private string playerName; // Player's name
    private string playerID; // Random generated player ID
    private bool isLoggedIn = false; // Login status
    private bool isFacebookLogin = false; // Facebook login status

    void Start()
    {
        InitializeUI();

        // Add listeners to buttons
        guestLoginButton.onClick.AddListener(OnGuestLogin);
        confirmButton.onClick.AddListener(OnConfirmPlayerName);
        startButton.onClick.AddListener(OnStartGame);
        logoutButton.onClick.AddListener(OnLogout);
        facebookLoginButton.onClick.AddListener(OnFacebookLogin);

        // Check if the player has previously logged in
        CheckLoginStatus();
    }

    // Initialize UI elements (hide/show based on login status)
    void InitializeUI()
    {
        playerInfoText.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        startButton.interactable = false; // Start button is disabled initially
        guestLoginPanel.SetActive(false); // Hide the guest login panel initially
    }

    // Check if the player was previously logged in (either guest or Facebook)
    void CheckLoginStatus()
    {
        if (PlayerPrefs.HasKey("PlayerName") && PlayerPrefs.HasKey("PlayerID"))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            playerID = PlayerPrefs.GetString("PlayerID");
            isLoggedIn = true;
            DisplayPlayerInfo();
        }
        else if (FB.IsInitialized && FB.IsLoggedIn)
        {
            // If Facebook is already logged in, handle Facebook login
            isFacebookLogin = true;
            HandleFacebookLogin();
        }
        else
        {
            // If no login data exists, show the guest login panel
            guestLoginPanel.SetActive(true);
        }
    }

    // Handle Guest Login Button Click
    void OnGuestLogin()
    {
        guestLoginPanel.SetActive(true); // Show the name input panel
    }

    // Handle Confirm Button (for input name validation and saving data)
    void OnConfirmPlayerName()
    {
        playerName = playerNameInput.text;

        if (IsValidName(playerName))
        {
            // Simulate checking if the name is available in the database
            if (CheckNameAvailability(playerName))
            {
                // Generate a random player ID
                playerID = GenerateRandomPlayerID();

                // Save player name and ID locally using PlayerPrefs
                SavePlayerData(playerName, playerID);

                // Display player info
                DisplayPlayerInfo();
            }
            else
            {
                feedbackText.text = "Player name is not available!";
            }
        }
        else
        {
            feedbackText.text = "Name must be between 5 and 12 characters!";
        }
    }

    // Check if the player name is valid
    bool IsValidName(string name)
    {
        return name.Length >= 5 && name.Length <= 12 && !string.IsNullOrEmpty(name);
    }

    // Simulate checking if the name is available (usually check against a database or server)
    bool CheckNameAvailability(string name)
    {
        // For the sake of this example, we will assume the name is always available
        return true;
    }

    // Generate a random 10-digit player ID
    string GenerateRandomPlayerID()
    {
        return Random.Range(1000000000, 9999999999).ToString();
    }

    // Save the player data (name and ID) locally using PlayerPrefs
    void SavePlayerData(string name, string id)
    {
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.SetString("PlayerID", id);
        PlayerPrefs.Save();
    }

    // Display player info (name and player ID)
    void DisplayPlayerInfo()
    {
        playerInfoText.SetActive(true);
        playerInfoText.GetComponent<Text>().text = "Player: " + playerName + " (ID: " + playerID + ")";
        startButton.interactable = true;
        logoutButton.gameObject.SetActive(true);
        guestLoginPanel.SetActive(false); // Hide the guest login panel
    }

    // Handle Start Game button click (connect to Photon)
    void OnStartGame()
    {
        if (isLoggedIn)
        {
            // Here you would connect to Photon to start the game
            // PhotonNetwork.ConnectUsingSettings(); // This is for Photon integration
            Debug.Log("Starting game with player: " + playerName);
        }
    }

    // Handle Facebook Login Button Click
    void OnFacebookLogin()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }
        else
        {
            FB.LogInWithReadPermissions(new string[] { "public_profile", "email" }, HandleFacebookLogin);
        }
    }

    // Facebook Initialization Complete Callback
    private void OnInitComplete()
    {
        if (FB.IsLoggedIn)
        {
            HandleFacebookLogin(FB.AccessToken);
        }
    }

    // Facebook Hide Unity Callback (to manage Facebook's native UI)
    private void OnHideUnity(bool isGameShown)
    {
        // Handle the state of the game when Unity is hidden (if needed)
    }

    // Handle the Facebook Login Response
    private void HandleFacebookLogin(ILoginResult result = null)
    {
        if (result != null && result.Error == null)
        {
            // Successful login, now check if the player is old or new
            string facebookID = FB.UserId;

            // Simulate checking if the player exists in the database via Facebook ID
            bool isOldPlayer = CheckIfFacebookPlayerExists(facebookID);

            if (isOldPlayer)
            {
                // Retrieve player data from the server or locally (simulate)
                playerName = GetPlayerNameFromServer(facebookID); // Example server retrieval
                playerID = GetPlayerIDFromServer(facebookID); // Example server retrieval
                SavePlayerData(playerName, playerID);
                isLoggedIn = true;
                DisplayPlayerInfo();
            }
            else
            {
                // New player, ask for name
                guestLoginPanel.SetActive(true);
                feedbackText.text = "Welcome new player! Please choose a name.";
            }
        }
        else
        {
            Debug.LogError("Facebook login failed: " + result.Error);
        }
    }

    // Simulate checking if the Facebook player exists (use server/database for real-world implementation)
    bool CheckIfFacebookPlayerExists(string facebookID)
    {
        // For the sake of example, assume no player exists with this ID
        return false; // If this is a new player, return false
    }

    // Simulate retrieving player name from a server/database
    string GetPlayerNameFromServer(string facebookID)
    {
        return "NewFacebookPlayer"; // Simulating a name retrieval from a server
    }

    // Simulate retrieving player ID from a server/database
    string GetPlayerIDFromServer(string facebookID)
    {
        return GenerateRandomPlayerID(); // Simulate retrieving a player ID
    }

    // Handle Logout button click
    void OnLogout()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("PlayerID");
        playerInfoText.SetActive(false);
        startButton.interactable = false;
        logoutButton.gameObject.SetActive(false);
        guestLoginPanel.SetActive(true); // Show the guest login panel again
    }
}
