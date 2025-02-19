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

    public void GameOver()
    {

    }

    private void Awake()
    {
        _isRunning = true;
    }
}
