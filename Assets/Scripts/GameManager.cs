using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindFirstObjectByType<GameManager>();
            }
            return _instance;
        }
    }

    bool _isRunning;
    PlayerScript _player;
    Spawner _spawner;
    public bool IsRunning { get { return _isRunning; } }
    public PlayerScript Player { get { return _player; } set { _player = value; } }
    public Spawner Spawner { get { return _spawner; } set { _spawner = value; } }

    public event EventHandler OnGameStart;

    IEnumerator GameStartCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        
        _isRunning = true;
        OnGameStart.Invoke(this, EventArgs.Empty);
    }

    public void GameOver()
    {

    }

    private void Start()
    {
        StartCoroutine(GameStartCoroutine());
    }
}
