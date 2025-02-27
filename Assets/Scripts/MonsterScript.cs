using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public static Vector3 monsterScale = Vector3.one;
    public static int maxHP = 10;

    Animator _animator;
    Rigidbody _rigidbody;
    Collider _collider;
    bool _isIdle = true;
    bool _isMove = false;
    bool _isDead = false;

    public bool IsDead => _isDead;

    public int HP;
    public float Speed;
    public int AttackDamage;
    public int Exp;

    public GameObject DamageTextPrefab;
    public GameObject HealthItemPrefab;
    public GameObject SpeedUpItemPrefab;

    public void ApplyDamage(int damage)
    {
        if (HP <= 0)
            return;

        HP -= damage;
        if (null != DamageTextPrefab)
        {
            GameObject dmgTextObj = Instantiate(DamageTextPrefab, transform.position + transform.up * 1.5f, Quaternion.identity);
            DamageText dmgText = dmgTextObj.GetComponent<DamageText>();
            dmgText.Text = damage.ToString();
        }
            

        if (HP <= 0)
        {
            _Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerSkill")
        {
            Skill skill = other.gameObject.GetComponent<Skill>();
            ApplyDamage(skill.Damage);
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
        int randNum = UnityEngine.Random.Range(0, 5);
        StartCoroutine(DieCoroutine(randNum));
    }

    IEnumerator DieCoroutine(int randNum)
    {
        float length = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);

        if (randNum == 3)
            Instantiate(HealthItemPrefab, transform.position + transform.up * 0.5f, Quaternion.identity);
        else if (randNum == 4)
            Instantiate(SpeedUpItemPrefab, transform.position + transform.up * 0.5f, Quaternion.identity);

        GameManager.Instance.Player.GetExp(Exp);
        GameManager.Instance.Spawner.Pool.ReturnObject(this.gameObject);
    }

    void _SetAnimatorParams()
    {
        _animator.SetBool("isIdle", _isIdle);
        _animator.SetBool("isMove", _isMove);
        _animator.SetBool("isDead", _isDead);
    }

    void _OnPlayerLevelUp(object o, EventArgs e)
    {
        transform.localScale = monsterScale;
    }

    private void OnEnable()
    {
        _isIdle = true;
        _isMove = false;
        _isDead = false;
        HP = maxHP;
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

        GameManager.Instance.Player.OnLevelUp += _OnPlayerLevelUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsRunning)
            return;

        if (!GameManager.Instance.Player.IsDead)
            _Move();

        _SetAnimatorParams();
    }
}
