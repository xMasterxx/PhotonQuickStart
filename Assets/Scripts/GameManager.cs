using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    private Button m_LeaveButton;
    private GameObject m_PlayerPrefab;
    public Button LeaveButton { get => m_LeaveButton != null ? m_LeaveButton : m_LeaveButton = GameObject.Find("LeaveRoomButton").GetComponent<Button>(); set => m_LeaveButton.onClick.AddListener(OnLeftRoom); }
    public GameObject PlayerPrefab { get => m_PlayerPrefab != null ? m_PlayerPrefab : m_PlayerPrefab = Resources.Load<GameObject>("Player"); }
    


    private void Start()
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector2(Random.Range(-7, 7), transform.position.y), Quaternion.identity);
    }


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Main");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }

    private void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level");
        PhotonNetwork.LoadLevel("Multiplayer");
    }
}
