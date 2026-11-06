using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int value = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;

        if (CoinManager.Instance != null)
            CoinManager.Instance.AddCoins(value);

        Destroy(gameObject);
    }
}
