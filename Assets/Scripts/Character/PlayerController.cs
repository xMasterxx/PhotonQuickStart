using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    [SerializeField] private Rigidbody2D m_AttackPrefab;
    private PhotonView m_PhotonView;
    private bool m_LookLeft;
    private float m_InputDirection;
    
    public Rigidbody2D AttackPrefab { get => m_AttackPrefab != null ? m_AttackPrefab : m_AttackPrefab = Resources.Load<Rigidbody2D>("AttackPrefab"); }
    public bool LookLeft { get => m_LookLeft; set => m_LookLeft = value; }
    public float InputDirection { get => m_InputDirection; set => m_InputDirection = value; }
    public PhotonView PhotonView { get => m_PhotonView != null ? m_PhotonView : m_PhotonView = GetComponent<PhotonView>(); }
   



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        Move();

        if (Hp <= 0)
        {
            Die();
        }
    }

    public override void Attack()
    {
        Rigidbody2D attackPrefabInstance;


        if (LookLeft)
        {
            attackPrefabInstance = Instantiate(AttackPrefab, new Vector2(transform.position.x - 3, transform.position.y), AttackPrefab.transform.rotation) as Rigidbody2D;
            attackPrefabInstance.AddForce(-gameObject.transform.right * 20, ForceMode2D.Impulse);
        }
        else
        {
            attackPrefabInstance = Instantiate(AttackPrefab, new Vector2(transform.position.x + 3, transform.position.y), AttackPrefab.transform.rotation) as Rigidbody2D;
            attackPrefabInstance.AddForce(gameObject.transform.right * 20, ForceMode2D.Impulse);
        }

        StartCoroutine(Destroy(attackPrefabInstance, 3f));

    }

    public override void Die()
    {
        Destroy(gameObject);

    }



    public override void Move()
    {
        InputDirection = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * Speed * InputDirection);
        if (InputDirection > 0 && LookLeft || InputDirection < 0 && !LookLeft)
        {
            Flip();
        }

        if (InputDirection > .6 || InputDirection < -.6)
        {
            Animator.Play("Run");
        }
        else
        {
            Animator.Play("Idle");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Hp -= 5;
            Destroy(collision.gameObject);
        }
    }
    private IEnumerator Destroy(Rigidbody2D prefab, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(prefab.gameObject);
    }

    private void Flip()
    {
        LookLeft = !LookLeft;
        transform.localScale = new Vector3(transform.localScale.x * -1,
            transform.localScale.y, transform.localScale.z);
    }

}
