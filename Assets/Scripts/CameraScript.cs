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

    // Update is called once per frame
    void LateUpdate()
    {
        if (!GameManager.Instance.IsRunning)
            return;

        _Move();
    }
}
