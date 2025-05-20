using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager instance;
    public static NetworkRunner runnerInstance;

    public TMP_InputField sessionNameInput;
    private string sessionName;

    public TextMeshProUGUI playerListText;  // UI để hiển thị danh sách người chơi
    public event Action onSceneLoaded;
    private List<PlayerRef> players = new List<PlayerRef>();
    public event Action onPlayerListChange;

    public static CustomData customData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        customData = GetComponent<CustomData>();
        sessionNameInput.onValueChanged.AddListener(UpdateSessionName);
    }

    public List<PlayerRef> GetAllPlayers()
    {
        return players;
    }

    void UpdateSessionName(string _sessionName)
    {
        sessionName = _sessionName;
    }

    public async void HostSever()
    {
        runnerInstance = gameObject.AddComponent<NetworkRunner>();
        runnerInstance.ProvideInput = true;
        runnerInstance.AddCallbacks(this);

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex + 1);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 4,
        });
       
        StartCoroutine(TriggerSceneLoaded());
    }

    public async void LoadScene(string sceneName)
    {
        await runnerInstance.LoadScene(sceneName);
        TriggerSceneLoaded();
    }

    IEnumerator TriggerSceneLoaded()
    {
        yield return new WaitForSeconds(1f);
        onSceneLoaded?.Invoke();
    }

    public async void JoinSever()
    {
        runnerInstance = gameObject.AddComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);
        await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
        });

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(!players.Contains(player))
        {
            players.Add(player);
            onPlayerListChange?.Invoke();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            onPlayerListChange?.Invoke();
        }

        if (runner != null)
        {
            runner.Shutdown();
        }

        SceneManager.LoadScene("MainMenu");
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        SceneManager.LoadScene("UI_StartScene");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        SceneManager.LoadScene("UI_StartScene");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
