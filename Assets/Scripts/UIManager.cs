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

    void _OnStart(object o, EventArgs e)
    {
        StartCoroutine(StartPanelCoroutine());
    }

    void _OnValueChanged(object o, EventArgs e)
    {
        _SetInfoTexts();
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

    void _SetInfoTexts()
    {
        if (null == GameManager.Instance.Player)
            return;

        levelText.text = GameManager.Instance.Player.Level.ToString();
        damageText.text = GameManager.Instance.Player.AttackDamage.ToString();
        attackSpeedText.text = GameManager.Instance.Player.AttackSpeed.ToString();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStart += _OnStart;
        GameManager.Instance.Player.OnValueChanged += _OnValueChanged;
    }

    void Update()
    {
        _SetSliderValues();
    }
}
