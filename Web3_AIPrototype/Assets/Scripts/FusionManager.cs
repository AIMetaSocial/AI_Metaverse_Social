using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FusionManager : MonoBehaviour
{
    public NetworkRunner RunnerPrefab;
    public int MaxPlayerCount = 8;


    [Header("UI Setup")]
    public TMP_Text StatusText;
    [SerializeField] Button playBTN;
    [SerializeField] private NetworkRunner _runnerInstance;
    private static string _shutdownStatus;

    public void PlayBTN()
    {   
        playBTN.interactable = false;
        StartGame("openworld");
    }   
    
    public void OpenSettings(){
        SettingsPanel.Instance?.OpenSettings();
    }
   


    public async UniTaskVoid StartGame(string gameModeIdentifier, GameMode gameMode = GameMode.Shared)
    {
        await Disconnect();

        _runnerInstance = Instantiate(RunnerPrefab);

        // Add listener for shutdowns so we can handle unexpected shutdowns
        var events = _runnerInstance.GetComponent<NetworkEvents>();
        events.OnShutdown.AddListener(OnShutdown);


        

        NetworkSceneManagerDefault networkSceneManagerDefault = _runnerInstance.GetComponent<NetworkSceneManagerDefault>();
        if (networkSceneManagerDefault == null)
        {
            networkSceneManagerDefault = _runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        int maxPlayers = MaxPlayerCount;

      
        

        var startArguments = new StartGameArgs()
        {
            GameMode = gameMode,
            //SessionName = gameModeIdentifier,
            //SessionName = "Match" + UnityEngine.Random.Range(1, 9999).ToString(),
            PlayerCount = maxPlayers,
            Scene = SceneRef.FromIndex(GeneralSettings.GAMESCENE),       

            
            SceneManager = networkSceneManagerDefault
        };

        StatusText.text = "Connecting...";

        var startTask = _runnerInstance.StartGame(startArguments);
        await startTask;

        if (startTask.Result.Ok)
        {
            StatusText.text = "Joining World";
            //CONNECTED 

            Debug.Log("CONNECTED TO SERVER");
          
        }
        else
        {
            Debug.Log("Connection Error" + startTask.Result.ShutdownReason);
            StatusText.text = "Connection Failed";
            playBTN.interactable = true;
        }
    }


    public async void DisconnectClicked()
    {
        await Disconnect();
    }

    public async void BackToMenu()
    {
        await Disconnect();
        SceneManager.LoadScene(0);
    }

    private void OnEnable()
    {
        // Try to load previous shutdown status
        StatusText.text = _shutdownStatus != null ? _shutdownStatus : string.Empty;
        _shutdownStatus = null;
    }

    public async UniTask Disconnect(bool realoadScene = true)
    {
        if (_runnerInstance == null)
            return;

        StatusText.text = "Disconnecting...";

        // Remove shutdown listener since we are disconnecting deliberately
        var events = _runnerInstance.GetComponent<NetworkEvents>();
        events.OnShutdown.RemoveListener(OnShutdown);

        await _runnerInstance.Shutdown();
        _runnerInstance = null;

        if (realoadScene)
        {
            // Reset of scene network objects is needed, reload the whole scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        // Unexpected shutdown happened (e.g. Host disconnected)

        // Save status into static variable, it will be used in OnEnable after scene load
        _shutdownStatus = $"Shutdown: {reason}";
        Debug.LogWarning(_shutdownStatus);

        // Reset of scene network objects is needed, reload the whole scene
        SceneManager.LoadScene(GeneralSettings.MENUSCENE);
    }
}
