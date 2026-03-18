using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json;

public class PlayerAPIManager : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:3000";
    private int loggedInPlayerId = -1;
    private string loggedInUsername = "";

    [Header("Panels")]
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject updateScorePanel;
    [SerializeField] private GameObject updatePasswordPanel;
    [SerializeField] private GameObject deleteAccountPanel;

    [Header("Register Panel")]
    [SerializeField] private TMP_InputField registerUsernameField;
    [SerializeField] private TMP_InputField registerPasswordField;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button goToLoginButton;

    [Header("Login Panel")]
    [SerializeField] private TMP_InputField loginUsernameField;
    [SerializeField] private TMP_InputField loginPasswordField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;

    [Header("Score Panel")]
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI deathsText;
    [SerializeField] private Button goToUpdateScoreButton;
    [SerializeField] private Button goToUpdatePasswordButton;
    [SerializeField] private Button goToDeleteAccountButton;
    [SerializeField] private Button logoutButton;

    [Header("Update Score Panel")]
    [SerializeField] private TMP_InputField killsInputField;
    [SerializeField] private TMP_InputField deathsInputField;
    [SerializeField] private Button updateScoreButton;
    [SerializeField] private Button cancelUpdateScoreButton;

    [Header("Update Password Panel")]
    [SerializeField] private TMP_InputField oldPasswordField;
    [SerializeField] private TMP_InputField newPasswordField;
    [SerializeField] private Button updatePasswordButton;
    [SerializeField] private Button cancelUpdatePasswordButton;

    [Header("Delete Account Panel")]
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Button cancelDeleteButton;

    [Header("Status")]
    [SerializeField] private TextMeshProUGUI statusText;

    void Start()
    {
        SetupButtons();
        ShowPanel(registerPanel);
    }

    void SetupButtons()
    {
        // Register
        registerButton.onClick.AddListener(() => StartCoroutine(Register()));
        goToLoginButton.onClick.AddListener(() => ShowPanel(loginPanel));

        // Login
        loginButton.onClick.AddListener(() => StartCoroutine(Login()));
        goToRegisterButton.onClick.AddListener(() => ShowPanel(registerPanel));

        // Score
        goToUpdateScoreButton.onClick.AddListener(() => ShowPanel(updateScorePanel));
        goToUpdatePasswordButton.onClick.AddListener(() => ShowPanel(updatePasswordPanel));
        goToDeleteAccountButton.onClick.AddListener(() => ShowPanel(deleteAccountPanel));
        logoutButton.onClick.AddListener(Logout);

        // Update Score
        updateScoreButton.onClick.AddListener(() => StartCoroutine(UpdateScore()));
        cancelUpdateScoreButton.onClick.AddListener(() => ShowPanel(scorePanel));

        // Update Password
        updatePasswordButton.onClick.AddListener(() => StartCoroutine(UpdatePassword()));
        cancelUpdatePasswordButton.onClick.AddListener(() => ShowPanel(scorePanel));

        // Delete Account
        confirmDeleteButton.onClick.AddListener(() => StartCoroutine(DeleteAccount()));
        cancelDeleteButton.onClick.AddListener(() => ShowPanel(scorePanel));
    }

    void ShowPanel(GameObject panel)
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(false);
        scorePanel.SetActive(false);
        updateScorePanel.SetActive(false);
        updatePasswordPanel.SetActive(false);
        deleteAccountPanel.SetActive(false);

        panel.SetActive(true);
    }

    void Logout()
    {
        loggedInPlayerId = -1;
        loggedInUsername = "";
        UpdateStatus("Logged out");
        ShowPanel(loginPanel);
    }

    //API

    IEnumerator Register()
    {
        if (string.IsNullOrEmpty(registerUsernameField.text) || 
            string.IsNullOrEmpty(registerPasswordField.text))
        {
            UpdateStatus("Username and password required!");
            yield break;
        }

        UpdateStatus("Registering...");

        var body = new { username = registerUsernameField.text, 
                         password = registerPasswordField.text };
        string json = JsonConvert.SerializeObject(body);

        using (UnityWebRequest request = new UnityWebRequest(BASE_URL + "/api/player/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                UpdateStatus("Registered! Please log in.");
                ShowPanel(loginPanel);
            }
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(
                    request.downloadHandler.text);
                UpdateStatus($"Error: {error.error}");
            }
        }
    }

    IEnumerator Login()
    {
        if (string.IsNullOrEmpty(loginUsernameField.text) || 
            string.IsNullOrEmpty(loginPasswordField.text))
        {
            UpdateStatus("Username and password required!");
            yield break;
        }

        UpdateStatus("Logging in...");

        var body = new { username = loginUsernameField.text, 
                         password = loginPasswordField.text };
        string json = JsonConvert.SerializeObject(body);

        using (UnityWebRequest request = new UnityWebRequest(BASE_URL + "/api/player/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<LoginResponse>(
                    request.downloadHandler.text);
                loggedInPlayerId = response.id;
                loggedInUsername = response.username;
                UpdateStatus($"Welcome {loggedInUsername}!");
                StartCoroutine(GetPlayerData());
            }
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(
                    request.downloadHandler.text);
                UpdateStatus($"Error: {error.error}");
            }
        }
    }

    IEnumerator GetPlayerData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(
            BASE_URL + $"/api/player/{loggedInPlayerId}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var player = JsonConvert.DeserializeObject<PlayerResponse>(
                    request.downloadHandler.text);
                usernameText.text = $"{player.username}";
                killsText.text = $"{player.kills}";
                deathsText.text = $"{player.deaths}";
                ShowPanel(scorePanel);
            }
            else
            {
                UpdateStatus("Failed to load player data");
            }
        }
    }

    IEnumerator UpdateScore()
    {
        if (string.IsNullOrEmpty(killsInputField.text) || 
            string.IsNullOrEmpty(deathsInputField.text))
        {
            UpdateStatus("Kills and deaths required!");
            yield break;
        }

        UpdateStatus("Updating score...");

        var body = new { id = loggedInPlayerId,
                         kills = int.Parse(killsInputField.text),
                         deaths = int.Parse(deathsInputField.text) };
        string json = JsonConvert.SerializeObject(body);

        using (UnityWebRequest request = new UnityWebRequest(
            BASE_URL + "/api/player/score", "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                UpdateStatus("Score updated!");
                StartCoroutine(GetPlayerData());
            }
            else
            {
                UpdateStatus("Failed to update score");
            }
        }
    }

    IEnumerator UpdatePassword()
    {
        if (string.IsNullOrEmpty(oldPasswordField.text) || 
            string.IsNullOrEmpty(newPasswordField.text))
        {
            UpdateStatus("Both password fields required!");
            yield break;
        }

        UpdateStatus("Updating password...");

        var body = new { id = loggedInPlayerId,
                         oldPassword = oldPasswordField.text,
                         newPassword = newPasswordField.text };
        string json = JsonConvert.SerializeObject(body);

        using (UnityWebRequest request = new UnityWebRequest(
            BASE_URL + "/api/player/updatePassword", "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                UpdateStatus("Password updated successfully!");
                ShowPanel(scorePanel);
            }
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(
                    request.downloadHandler.text);
                UpdateStatus($"Error: {error.error}");
            }
        }
    }

    IEnumerator DeleteAccount()
    {
        UpdateStatus("Deleting account...");

        using (UnityWebRequest request = UnityWebRequest.Delete(
            BASE_URL + $"/api/player/{loggedInPlayerId}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                UpdateStatus("Account deleted!");
                loggedInPlayerId = -1;
                loggedInUsername = "";
                ShowPanel(registerPanel);
            }
            else
            {
                UpdateStatus("Failed to delete account");
            }
        }
    }

    void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        Debug.Log($"Status: {message}");
    }
}

[System.Serializable]
public class LoginResponse
{
    public int id;
    public string username;
    public string message;
}

[System.Serializable]
public class PlayerResponse
{
    public int id;
    public string username;
    public int kills;
    public int deaths;
}

[System.Serializable]
public class ErrorResponse
{
    public string error;
}
