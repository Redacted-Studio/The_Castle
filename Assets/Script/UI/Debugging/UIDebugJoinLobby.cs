using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDebugJoinLobby : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;

    public void DebugJoinLobby()
    {
        LobbySystem.Instance.DebugJoinLobbyByCode(input.text);
    }
}
