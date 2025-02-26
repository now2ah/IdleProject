using UnityEngine;

public class Skill : MonoBehaviour
{
    public int damage;

    public int Damage { get { return damage; } set { damage = value; } }

    private void Start()
    {
        Destroy(this.gameObject, 1.2f);
    }
}
