using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private Text coinText;
    private int coins;

    public int Coins => coins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateDisplay();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        coins += amount;
        UpdateDisplay();
    }

    public bool TrySpend(int amount)
    {
        if (amount < 0)
            return false;

        if (coins < amount)
            return false;

        coins -= amount;
        UpdateDisplay();
        return true;
    }

    private void UpdateDisplay()
    {
        if (coinText != null)
            coinText.text = $"COIN: {coins}";
    }
}
