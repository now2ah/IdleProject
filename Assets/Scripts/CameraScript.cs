using System.Collections;
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

    void LateUpdate()
    {
        if (!GameManager.Instance.IsRunning)
            return;

        _Move();
    }
}
