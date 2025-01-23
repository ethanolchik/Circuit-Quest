using UnityEngine;

public class EndOfMap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            HealthSystem.Instance.FallOffMap();
        }
    }
}
