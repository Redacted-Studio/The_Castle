using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyList : MonoBehaviour
{
    [HideInInspector] public string lobbyName = "Test Lobby";
    [HideInInspector] public int lobbyPlayerCount = 3;
    [HideInInspector] public string lobbyID;

    [SerializeField] private TMP_Text labelLobbyName;
    [SerializeField] private TMP_Text labelLobbyPlayer;
    [SerializeField] private Button buttonJoinLobby;

    private void Start()
    {
        UpdateDisplay();
    }

    public void JoinLobby()
    {
        LobbySystem.Instance.JoinLobbyByID(lobbyID);
    }

    public void UpdateDisplay()
    {
        labelLobbyName.text = lobbyName;
        labelLobbyPlayer.text = lobbyPlayerCount.ToString() + " / 4";
    }
}
