# FiveGatesOfAruna

Unity 2D action-platformer prototype for Bagian B. The prototype demonstrates reusable enemy prefabs configured with `EnemyStats` ScriptableObject assets and a singleton `CoinManager` connected to the HUD.

## Open the project

1. Add this repository folder in Unity Hub.
2. Open `Assets/Scenes/Prototype.unity`.
3. Use a Unity 2023.1 editor and press **Play**.

## Gameplay controls

| Input | Action |
| --- | --- |
| A/D or Left/Right Arrow | Move |
| Left Shift | Sprint |
| Space | Jump |
| J or Left Mouse Button | Attack |
| U | Buy a damage upgrade when enough coins are available |

## Enemy data

The three prefabs in `Assets/Prefabs` each reference an `EnemyStats` asset in `Assets/Data`:

| Enemy | Health | Damage | Move speed | Attack cooldown | Armor | Coin reward |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Grunt | 50 | 10 | 2.0 | 1.5 s | 20% | 5 |
| Brute | 100 | 20 | 1.5 | 1.3 s | 30% | 10 |
| Stalker | 180 | 35 | 1.0 | 1.0 s | 40% | 20 |

`CoinManager` grants the configured coin reward when an enemy is defeated and updates the HUD. The `cost` field is stored in enemy data but enemy purchasing is not implemented in this prototype.

## Deliverables

See `Deliverables` for the Unity package, report, live-sheet workbook, and Unity Editor screenshots. The optional WebGL/Windows build is not included.
