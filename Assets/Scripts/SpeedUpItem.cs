using UnityEngine;

public class SpeedUpItem : MonoBehaviour
{
    public float rotateSpeed = 3f;
    public float speedUpAmount = 2.5f;
    public float speedUpTime = 3f;

    public GameObject ImageObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other is CapsuleCollider)
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            player.SpeedUp(speedUpAmount, speedUpTime);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        ImageObject.transform.Rotate(0f, rotateSpeed, 0f);
    }
}
