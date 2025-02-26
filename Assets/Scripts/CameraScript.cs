using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Vector3 PositionOffset;
    //public Vector3 RotationOffset;

    public int shakeCount = 3;

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        for (int i=0; i<shakeCount; i++)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

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
