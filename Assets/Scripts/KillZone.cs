using UnityEngine;

public class KillZone : MonoBehaviour
{
    public float DamagePoints = 100f;

    private void OnTriggerStay(Collider other)
    {
        Health H = other.GetComponent<Health>();

        if (H == null) return;

        H.HealthPoints -= DamagePoints;
    }

}
