using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : CharacterBase, IPunObservable
{
    private GameObject m_AttackPrefab;
    private GameManager m_GameManagerScript;
    private PhotonView m_PhotonView;
    private float m_InputDirection;
    private static GameObject m_LocalPlayerInstance;
    public GameObject AttackPrefab { get => m_AttackPrefab != null ? m_AttackPrefab : m_AttackPrefab = Resources.Load<GameObject>("AttackPrefab"); }

    public float InputDirection { get => m_InputDirection; set => m_InputDirection = value; }
    public PhotonView PhotonView { get => m_PhotonView != null ? m_PhotonView : m_PhotonView = GetComponent<PhotonView>(); }
    public GameManager GameManagerScript { get => m_GameManagerScript != null ? m_GameManagerScript : m_GameManagerScript = GameObject.FindWithTag("MainCamera").GetComponent<GameManager>(); }
    public static GameObject LocalPlayerInstance { get => m_LocalPlayerInstance; set => m_LocalPlayerInstance = value; }

    private void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (PhotonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        AddObservable();
    }


    void Update()
    {
        if (PhotonView.IsMine && SceneManager.GetActiveScene().name == "Multiplayer")
        {
            if (Hp > 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Attack();
                }

                Move();
            }
            GetPlayerStatus();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data 
            stream.SendNext(Hp);
            stream.SendNext(SpriteRenderer.flipX);
            stream.SendNext(PlayerInfo.text);
        }
        else
        {
            // Network player, receive data
            Hp = (float)stream.ReceiveNext();
            SpriteRenderer.flipX = (bool)stream.ReceiveNext();
            PlayerInfo.text = (string)stream.ReceiveNext();
        }
    }


    public override void Attack()
    {

        if (SpriteRenderer.flipX)
        {
            var attackPrefab = PhotonNetwork.Instantiate(AttackPrefab.name, new Vector2(transform.position.x - 2, transform.position.y), AttackPrefab.transform.rotation);
            attackPrefab.GetComponent<PhotonView>().RPC("ChangeDirection", RpcTarget.AllBuffered);
        }
        else
        {
          PhotonNetwork.Instantiate(AttackPrefab.name, new Vector2(transform.position.x + 2, transform.position.y), AttackPrefab.transform.rotation);
        }

    }

    public override void Die()
    {
        GameManagerScript.DeadScreen.gameObject.SetActive(true);
        this.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 90), .5f);
        StartCoroutine(PhotonDestroy(gameObject, 3f));
        Animator.SetInteger("State", 3);
    }



    public override void Move()
    {
        InputDirection = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * Speed * InputDirection);

        if (InputDirection > 0 && SpriteRenderer.flipX || InputDirection < 0 && !SpriteRenderer.flipX)
        {
            Flip();
        }

        if (InputDirection > .4 || InputDirection < -.4)
        {
            Animator.SetInteger("State", 1);
        }
        else
        {
            Animator.SetInteger("State", 2);
        }
    }

    public override void GetPlayerStatus()
    {
        if (Hp <= 0)
        {
            Die();
            PlayerInfo.text = $"{PlayerPrefs.GetString("PlayerName")} : 0";
        }
        else if (Hp > 0)
        {
            PlayerInfo.text = $"{PlayerPrefs.GetString("PlayerName")} : {Hp}";
        }
    }
    private void AddObservable()
    {
        if (!PhotonView.ObservedComponents.Contains(this))
        {
            PhotonView.ObservedComponents.Add(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!PhotonView.IsMine)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Hp -= 5;
        }
    }
    private IEnumerator PhotonDestroy(GameObject prefab, float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(prefab);
    }

    private bool Flip() => SpriteRenderer.flipX = !SpriteRenderer.flipX;

}
