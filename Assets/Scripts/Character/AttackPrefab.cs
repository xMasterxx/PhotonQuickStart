using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrefab : MonoBehaviourPun
{
    private bool m_LookLeft;
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_DestroyTime = 3f;

    public bool LookLeft { get => m_LookLeft; set => m_LookLeft = value; }
    public float Speed { get => m_Speed; set => m_Speed = value; }
    public float DestroyTime { get => m_DestroyTime; set => m_DestroyTime = value; }

    private void Start()
    {
        StartCoroutine(DestroyPrefab(DestroyTime));
    }

    private void Update()
    {

        if (LookLeft)
        {
            transform.Translate(Vector2.left * Time.deltaTime * Speed);
        }
        else
        {
            transform.Translate(Vector2.right * Time.deltaTime * Speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroyPrefab(0f));
        }
    }

    private IEnumerator DestroyPrefab(float _time)
    {
        yield return new WaitForSeconds(_time);
        GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void Destroy() => Destroy(gameObject);

    [PunRPC]
    public void ChangeDirection() => LookLeft = !LookLeft;
}
