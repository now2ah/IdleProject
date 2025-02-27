using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public float rotateSpeed = 3f;
    public int healAmount = 1;

    public GameObject ImageObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other is CapsuleCollider)
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            player.Heal(healAmount);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        ImageObject.transform.Rotate(0f, rotateSpeed, 0f);
    }
}
