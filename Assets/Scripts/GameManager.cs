using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    private Button m_LeaveButton;
    private GameObject m_PlayerPrefab;
    private Image m_DeadScreen;
    public Button LeaveButton { get => m_LeaveButton != null ? m_LeaveButton : m_LeaveButton = GameObject.Find("Canvas/LeaveRoomButton").GetComponent<Button>(); }
    public GameObject PlayerPrefab { get => m_PlayerPrefab != null ? m_PlayerPrefab : m_PlayerPrefab = Resources.Load<GameObject>("Player"); }
    public Image DeadScreen { get => m_DeadScreen != null ? m_DeadScreen : m_DeadScreen = GameObject.Find("Canvas/DeadScreen").GetComponent<Image>(); }


    private void Start()
    {
        LeaveButton.onClick.AddListener(LeaveRoom);
        DeadScreen.gameObject.SetActive(false);

        if (PlayerController.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector2(Random.Range(-7, 7), transform.position.y), Quaternion.identity);
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }

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
