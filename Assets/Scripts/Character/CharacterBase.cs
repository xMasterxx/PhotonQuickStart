
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_Hp;
    [SerializeField] private float m_AttackValue;
    private Text m_PlayerInfo;
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;


    public float Hp { get => m_Hp; set => m_Hp = value; }
    public float Speed { get => m_Speed; set => m_Speed = value; }
    public float AttackValue { get => m_AttackValue; set => m_AttackValue = value; }
    public Animator Animator { get => m_Animator != null ? m_Animator : m_Animator = GetComponent<Animator>(); }
    public SpriteRenderer SpriteRenderer { get => m_SpriteRenderer != null ? m_SpriteRenderer : m_SpriteRenderer = GetComponent<SpriteRenderer>(); }
    public Text PlayerInfo { get => m_PlayerInfo!= null ? m_PlayerInfo : m_PlayerInfo = GetComponentInChildren<Text>(); }

    public abstract void Move();
    public abstract void Attack();
    public abstract void Die();
    public abstract void GetPlayerStatus();

}
