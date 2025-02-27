using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackSpeedText;
    public Slider hpSlider;
    public Slider expSlider;
    public Image gameStartImage;
    public Image gameOverImage;
    public Image baseAttackImage;
    public Slider baseAttackSlider;
    public Image skillImage;
    public Slider skillSlider;

    void _OnStart(object o, EventArgs e)
    {
        StartCoroutine(StartPanelCoroutine());
    }

    void _OnValueChanged(object o, EventArgs e)
    {
        _SetInfoTexts();
    }

    void _OnDie(object o, EventArgs e)
    {
        gameOverImage.gameObject.SetActive(true);
    }

    IEnumerator StartPanelCoroutine()
    {
        gameStartImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        gameStartImage.gameObject.SetActive(false);
    }

    void _SetSliderValues()
    {
        if (null == GameManager.Instance.Player)
            return;

        float hpRatio = (float)GameManager.Instance.Player.HP / (float)GameManager.Instance.Player.MaxHP;
        hpSlider.value = hpRatio;

        float expRatio = (float)GameManager.Instance.Player.Exp / (float)GameManager.Instance.Player.MaxExp;
        expSlider.value = expRatio;
    }

    void _SetSkillValues()
    {
        if (null == GameManager.Instance.Player)
            return;

        baseAttackImage.gameObject.SetActive(GameManager.Instance.Player.CanAttack);
        baseAttackSlider.value = GameManager.Instance.Player.NextAttackRatio;

        skillImage.gameObject.SetActive(GameManager.Instance.Player.CanSkill);
        skillSlider.value = GameManager.Instance.Player.NextSkillRatio;
    }

    void _SetInfoTexts()
    {
        if (null == GameManager.Instance.Player)
            return;

        levelText.text = "Level : " + GameManager.Instance.Player.Level.ToString();
        damageText.text = "Damage : " + GameManager.Instance.Player.AttackDamage.ToString();
        attackSpeedText.text = "AttackSpeed : " + GameManager.Instance.Player.AttackSpeed.ToString();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStart += _OnStart;
        GameManager.Instance.Player.OnValueChanged += _OnValueChanged;
        GameManager.Instance.Player.OnDie += _OnDie;
    }

    void Update()
    {
        _SetSliderValues();
        _SetSkillValues();
    }
}
