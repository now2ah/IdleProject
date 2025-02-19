using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Vector3 PositionOffset;
    //public Vector3 RotationOffset;

    void _Move()
    {
        transform.position = GameManager.Instance.Player.transform.position + PositionOffset;
    }

    private void Awake()
    {
        PositionOffset = transform.position;
        //RotationOffset = transform.rotation.eulerAngles;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _Move();
    }
}
