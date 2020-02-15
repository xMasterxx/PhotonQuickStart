using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_Hp;
    [SerializeField] private float m_AttackValue;
    private Animator m_Animator;



    public float Hp { get => m_Hp; set => m_Hp = value; }
    public float Speed { get => m_Speed; set => m_Speed = value; }
    public float AttackValue { get => m_AttackValue; set => m_AttackValue = value; }
    public Animator Animator { get => m_Animator != null ? m_Animator : m_Animator = GetComponent<Animator>(); }
    

    public abstract void Move();
    public abstract void Attack();
    public abstract void Die();

}
