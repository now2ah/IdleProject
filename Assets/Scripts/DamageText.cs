using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    float _maxY = 1.2f;
    TextMeshPro _text;

    public GameObject TextObj;
    public string Text 
    {
        get
        {
            return _text.text;
        }

        set
        {
            _text.text = value;
        }
    }

    IEnumerator EffectCoroutine()
    {
        float alpha = 1f;
        while(TextObj.transform.localPosition.y < _maxY)
        {
            TextObj.transform.position += TextObj.transform.up * Time.deltaTime;
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha -= Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void Awake()
    {
        _text = TextObj.GetComponent<TextMeshPro>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EffectCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
