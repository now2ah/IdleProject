using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
    Animator _animator;
    Rigidbody _rigidbody;
    Collider _collider;
    bool _isIdle = true;
    bool _isMove = false;
    bool _isMoveBWD = false;
    bool _isAttack = false;
    bool _isDead = false;
    float _lastAttackTime;

    [SerializeField]
    int _exp;
    [SerializeField]
    int _maxExp;
    [SerializeField]
    int _maxHP;

    public bool IsDead => _isDead;
    public int Exp => _exp;
    public int MaxExp => _maxExp;
    public int MaxHP => _maxHP;

    public int Level;
    public int HP;
    public int AttackDamage;
    public float Speed;
    public float RotateSpeed;
    public float AttackSpeed;

    public event EventHandler OnValueChanged;

    public void ApplyDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            _Die();
        }
        Debug.Log("Get Damage (" + HP + "/" + _maxHP + ")");
    }

    public void GetExp(int exp)
    {
        _exp += exp;

        if (_exp >= _maxExp)
        {
            do
            {
                _LevelUp();
            }
            while (_exp >= _maxExp);

        }
        Debug.Log("Get Exp (" + _exp + "/" + _maxExp + ")");
    }

    void _Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _isIdle = false;
            _isMove = true;
            transform.position += (transform.forward * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            _isIdle = false;
            _isMoveBWD = true;
            transform.position += (transform.forward * -1f * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0f, RotateSpeed, 0f));
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0f, RotateSpeed * -1f, 0f));
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            _isIdle = true;
            _isMove = false;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            _isIdle = true;
            _isMoveBWD = false;
        }
    }

    void _Attack(Monster monster)
    {
        StartCoroutine(AttackCoroutine(monster));
    }

    IEnumerator AttackCoroutine(Monster monster)
    {
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        _isIdle = false;
        _isMove = false;
        _isMoveBWD = false;
        _isAttack = true;
        transform.LookAt(monster.transform.position);
        yield return new WaitForSeconds(length/2);
        monster.ApplyDamage(AttackDamage);
        yield return new WaitForSeconds(length / 2);
        _isIdle = true;
        _isMove = false;
        _isMoveBWD = false;
        _isAttack = false;
        _lastAttackTime = Time.time;
    }

    bool _CanAttack()
    {
        if ((Time.time - _lastAttackTime) > AttackSpeed)
            return true;
        else
            return false;
    }

    void _SetAnimationParams()
    {
        _animator.SetBool("isIdle", _isIdle);
        _animator.SetBool("isMove", _isMove);
        _animator.SetBool("isMoveBWD", _isMoveBWD);
        _animator.SetBool("isAttack", _isAttack);
        _animator.SetBool("isDead", _isDead);
    }

    void _Die()
    {
        _isIdle = false;
        _isMove = false;
        _isMoveBWD = false;
        _isAttack = false;
        _isDead = true;
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        GameManager.Instance.GameOver();
    }

    void _LevelUp()
    {
        _exp -= _maxExp;
        _maxExp *= 2;
        Level++;

        _maxHP += 2;
        HP = _maxHP;

        OnValueChanged.Invoke(this, EventArgs.Empty);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            if (collision.gameObject.TryGetComponent<Monster>(out Monster monster))
            {
                if (!_isDead)
                    ApplyDamage(monster.AttackDamage);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Monster")
        {
            if (other.TryGetComponent<Monster>(out Monster monster))
            {
                if (_CanAttack() && !_isMove && !_isMoveBWD)
                    _Attack(monster);
            }
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _isIdle = true;
        _isMove = false;
        _isAttack = false;
        _isDead = false;
        Level = 1;
        HP = 10;
        AttackDamage = 10;
        Speed = 2.2f;
        RotateSpeed = 1.0f;
        AttackSpeed = 1.0f;
        _exp = 0;
        _maxExp = 5;
        _maxHP = 10;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.Player = this;
        OnValueChanged.Invoke(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsRunning)
            return;

        if (!_isAttack)
        {
            _Move();
        }

        _SetAnimationParams();
    }
}
