using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For switching scenes
using Photon.Pun; // For multiplayer

public class LoginManager : MonoBehaviour
{
    // UI Elements
    public Button guestLoginButton;
    public Button facebookLoginButton;
    public InputField playerNameInputField; // Input for player to type their name
    public Text playerInfoText; // Displays Player Info (name only)
    public Button startGameButton;
    public Button logoutButton;
    public GameObject nameInputPanel; // Pop-up for name input
    public Text nameInputPanelText; // Instructions in the name input pop-up
    public Text errorMessageText; // Displays the result of the name availability check

    private string playerName;
    private string playerID;

    void Start()
    {
        // Initially hide player info and buttons
        playerInfoText.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);

        // Check if the player is already logged in (Guest login will be saved locally)
        if (PlayerPrefs.HasKey("playerID"))
        {
            playerID = PlayerPrefs.GetString("playerID");
            playerName = PlayerPrefs.GetString("playerName");
            DisplayPlayerInfo();
        }
        else
        {
            // Show login buttons if no saved data
            guestLoginButton.gameObject.SetActive(true);
            facebookLoginButton.gameObject.SetActive(true);
        }

        // Add listeners to buttons
        guestLoginButton.onClick.AddListener(GuestLogin);
        facebookLoginButton.onClick.AddListener(FacebookLogin);
        startGameButton.onClick.AddListener(StartGame);
        logoutButton.onClick.AddListener(Logout);
    }

    void GuestLogin()
    {
        // Generate a random 10-digit ID for guest login
        playerID = GenerateRandomID();
        PlayerPrefs.SetString("playerID", playerID); // Save the ID locally
        // Show the name input panel to get the player's name
        nameInputPanel.SetActive(true);
        nameInputPanelText.text = "Enter your name (max 15 characters)";
        errorMessageText.text = ""; // Clear any previous error messages
    }

    void FacebookLogin()
    {
        // Placeholder for Facebook login integration
        // You would use Facebook SDK here to get the player's Facebook data.
        playerID = "FB_" + Random.Range(100000, 999999).ToString();  // Replace with Facebook user ID

        // Show the name input panel for Facebook login to allow player to enter their name
        nameInputPanel.SetActive(true);
        nameInputPanelText.text = "Enter your name (max 15 characters)";
        errorMessageText.text = ""; // Clear any previous error messages
    }

    void OnNameEntered()
    {
        // Get the player's typed name
        string enteredName = playerNameInputField.text;

        // Check if the name is valid (max 15 characters)
        if (enteredName.Length > 15)
        {
            errorMessageText.text = "Name cannot exceed 15 characters";
            return;
        }

        // Check if the name is available (simulate a check, you can replace this with a real check)
        if (IsNameTaken(enteredName))
        {
            errorMessageText.text = "Player Name is not available";
            return;
        }
        else
        {
            errorMessageText.text = "Player Name is Available"; // Show that the name is available
            playerName = enteredName;
            PlayerPrefs.SetString("playerName", playerName);
            nameInputPanel.SetActive(false); // Hide the name input panel
            DisplayPlayerInfo();
        }
    }

    bool IsNameTaken(string name)
    {
        // Simulate checking if the name already exists (this can be replaced with a real server-side check)
        // For now, we return true if the name is "TestName", for example.
        return name == "TestName"; // Example condition for name availability check
    }

    void DisplayPlayerInfo()
    {
        // Show player info and buttons if logged in
        playerInfoText.gameObject.SetActive(true);
        playerInfoText.text = "Player Name: " + playerName; // Display name only
        startGameButton.gameObject.SetActive(true);
        logoutButton.gameObject.SetActive(true);

        guestLoginButton.gameObject.SetActive(false);
        facebookLoginButton.gameObject.SetActive(false);
    }

    void StartGame()
    {
        // Load multiplayer game scene (using Photon)
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon network
        SceneManager.LoadScene("GameScene"); // Replace with your game scene name
    }

    void Logout()
    {
        // Clear saved data and reset UI
        PlayerPrefs.DeleteKey("playerID");
        PlayerPrefs.DeleteKey("playerName");

        playerName = "";
        playerID = "";

        // Hide player info and buttons
        playerInfoText.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);

        guestLoginButton.gameObject.SetActive(true);
        facebookLoginButton.gameObject.SetActive(true);
    }

    string GenerateRandomID()
    {
        // Generate a 10-digit random ID
        return Random.Range(1000000000, 9999999999).ToString();
    }
}
