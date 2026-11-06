using UnityEngine;
using UnityEngine.UI;

public class PrototypeHUD : MonoBehaviour
{
    [SerializeField] private Text statusText;
    [SerializeField] private PlayerController player;

    private void Update()
    {
        if (statusText == null || player == null)
            return;

        statusText.text = $"HP: {player.Health}/{player.MaxHealth}   DMG: {player.AttackDamage}\n" +
                          $"A/D: Gerak   SHIFT: Lari   SPACE: Lompat   J/klik kiri: Serang\n" +
                          $"U: Upgrade DMG ({player.NextUpgradeCost} koin)";
    }
}
