using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator _animator;
    Rigidbody _rigidbody;
    Collider _collider;
    bool _isIdle = true;
    bool _isMove = false;
    bool _isDead = false;

    public int HP;
    public float Speed;
    public int AttackDamage;
    public int Exp;

    public void ApplyDamage(int damage)
    {
        if (HP <= 0)
            return;

        HP -= damage;

        if (HP <= 0)
        {
            _Die();
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    void _Move()
    {
        _isIdle = true;
        _isMove = false;
        transform.LookAt(GameManager.Instance.Player.transform.position);
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    void _Die()
    {
        _isDead = true;
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        GameManager.Instance.Player.GetExp(Exp);
        GameManager.Instance.Spawner.Pool.ReturnObject(this.gameObject);
    }

    void _SetAnimatorParams()
    {
        _animator.SetBool("isIdle", _isIdle);
        _animator.SetBool("isMove", _isMove);
        _animator.SetBool("isDead", _isDead);
    }

    private void OnEnable()
    {
        _isIdle = true;
        _isMove = false;
        _isDead = false;
        HP = 10;
        Speed = 1f;
        AttackDamage = 1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isIdle = true;
        _isMove = false;
        _isDead = false;
        HP = 10;
        Speed = 1f;
        AttackDamage = 1;
        Exp = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.Player.IsDead)
            _Move();

        _SetAnimatorParams();
    }
}
