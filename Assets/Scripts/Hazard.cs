using UnityEngine;

public class HazardTrap : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {

        void OnCollisionEnter(Collision other)
        {
            Debug.Log($"Collision with: {other.gameObject.name}");
        }


        Debug.Log($"Trigger entered by: {other.gameObject.name} tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            DissolveEffect dissolve = other.GetComponentInChildren<DissolveEffect>();
            if (dissolve != null)
                dissolve.StartDissolve();
            else
                GameManager.Instance?.OnPlayerDeath();
        }

        if (other.CompareTag("Enemy"))
            Destroy(other.gameObject);
    }
}