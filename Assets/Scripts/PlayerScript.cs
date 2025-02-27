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
    bool _canAttack = false;
    float _lastAttackTime;
    float _nextAttackRatio;
    bool _canSkill = false;
    float _lastSkilltime;
    float _nextSkillRatio;

    [SerializeField]
    int _exp;
    [SerializeField]
    int _maxExp;
    [SerializeField]
    int _maxHP;

    public bool IsDead => _isDead;
    public bool CanAttack => _canAttack;
    public float NextAttackRatio => _nextAttackRatio;
    public bool CanSkill => _canSkill;
    public float NextSkillRatio => _nextSkillRatio;
    public int Exp => _exp;
    public int MaxExp => _maxExp;
    public int MaxHP => _maxHP;

    public int Level;
    public int HP;
    public int AttackDamage;
    public float Speed;
    public float RotateSpeed;
    public float AttackSpeed;
    public int SkillDamage;
    public float SkillCooltime;

    public GameObject LevelUpParticlePrefab;
    public GameObject AttackParticlePrefab;
    public GameObject SkillParticlePrefab;
    public GameObject LoseHPParticlePrefab;

    public event EventHandler OnValueChanged;
    public event EventHandler OnLevelUp;
    public event EventHandler OnDie;

    public void ApplyDamage(int damage)
    {
        HP -= damage;

        GameObject loseHPParticle = Instantiate(LoseHPParticlePrefab, transform.position + transform.up, Quaternion.identity);

        if (HP <= 0)
        {
            _Die();
        }
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
    }

    public void Heal(int amount)
    {
        if (_isDead)
            return;

        HP += amount;

        if (HP > _maxHP)
            HP = _maxHP;
    }

    public void SpeedUp(float amount, float time)
    {
        if (_isDead)
            return;

        StartCoroutine(SpeedUpCoroutine(amount, time));
    }

    IEnumerator SpeedUpCoroutine(float amount, float time)
    {
        float originSpeed = Speed;
        Speed *= amount;
        yield return new WaitForSeconds(time);
        Speed = originSpeed;
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
            transform.Rotate(new Vector3(0f, RotateSpeed * Speed * Time.deltaTime, 0f));
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0f, RotateSpeed * Speed * Time.deltaTime * -1f, 0f));
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
        _lastAttackTime = Time.time;
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        _isIdle = false;
        _isMove = false;
        _isMoveBWD = false;
        _isAttack = true;
        transform.LookAt(monster.transform.position);
        yield return new WaitForSeconds(length/2);
        monster.ApplyDamage(AttackDamage);
        GameObject effect = Instantiate(AttackParticlePrefab, transform.position + transform.up, transform.rotation);
        yield return new WaitForSeconds(length / 2);
        Destroy(effect);
        _isIdle = true;
        _isMove = false;
        _isMoveBWD = false;
        _isAttack = false;
    }

    bool _CanAttack()
    {
        if ((Time.time - _lastAttackTime) > AttackSpeed)
        {
            return true;
        }
        else
        {
            _nextAttackRatio = (Time.time - _lastAttackTime) / AttackSpeed;
            return false;
        }
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
        yield return new WaitForEndOfFrame();
        _SetAnimationParams();
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        OnDie.Invoke(this, EventArgs.Empty);
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
        Monster.monsterScale *= 1.5f;
        Monster.maxHP += Level;
        OnLevelUp.Invoke(this, EventArgs.Empty);
        

        if (null != LevelUpParticlePrefab)
        {
            Instantiate(LevelUpParticlePrefab, transform.position + transform.up * 2f, Quaternion.identity);
        }
    }

    void _GetSkillInput()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_canSkill)
            {
                _ActivateSkill();
            }
        }
    }

    void _ActivateSkill()
    {
        StartCoroutine(SkillCoroutine());
    }

    IEnumerator SkillCoroutine()
    {
        GameObject skillObj = Instantiate(SkillParticlePrefab, transform.position, Quaternion.identity);
        Skill skill = skillObj.GetComponent<Skill>();
        skill.damage = SkillDamage;
        _lastSkilltime = Time.time;
        yield return null;
    }

    bool _CanSkill()
    {
        if ((Time.time - _lastSkilltime) > SkillCooltime)
        {
            return true;
        }
        else
        {
            _nextSkillRatio = (Time.time - _lastSkilltime) / SkillCooltime;
            return false;
        }
    }

    void _OnStart(object o, EventArgs e)
    {
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
        if (_isDead)
            return;

        if (other.tag == "Monster")
        {
            if (other.TryGetComponent<Monster>(out Monster monster))
            {
                if (_CanAttack() && !_isMove && !_isMoveBWD && !monster.IsDead)
                {
                    _Attack(monster);
                }
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
        _lastAttackTime = 0f;
        _nextAttackRatio = 1f;
        _lastSkilltime = 0f;
        _nextSkillRatio = 1f;
        Level = 1;
        HP = 10;
        AttackDamage = 10;
        Speed = 2.2f;
        RotateSpeed = 100.0f;
        AttackSpeed = 1.0f;
        SkillDamage = 10;
        SkillCooltime = 1f;
        _exp = 0;
        _maxExp = 5;
        _maxHP = 10;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnGameStart += _OnStart;
        GameManager.Instance.Player = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsRunning)
            return;

        if (_isDead)
            return;

        if (!_isAttack)
        {
            _Move();
        }

        _canAttack = _CanAttack();
        _canSkill = _CanSkill();

        if (_canSkill)
            _GetSkillInput();

        _SetAnimationParams();
    }
}
