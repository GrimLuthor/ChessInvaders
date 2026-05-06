# Chess Invaders — Collaborator Onboarding

Welcome. This doc is everything you need to understand the project and get running.

---

## What Is This Game

Chess Invaders is a top-down 2D game jam project combining chess and Space Invaders.

The player controls a **King piece** that moves freely (not tile-locked) across a standard 8×8 chess board. Waves of enemy chess pieces advance toward the player one tile at a time on a shared timer. The player shoots projectiles aimed with the mouse and has a burst special attack. The enemies shoot back using their chess movement patterns as attack patterns — a Rook shoots in straight lines, a Bishop along diagonals, a Knight in L-shapes, etc. Before each shot, a visible warning flashes on the target tiles so the player can dodge.

There are 3 waves. The final wave includes an enemy King — killing it is the win condition. The player losing all HP is the lose condition.

**Current phase:** gray box — placeholder art only, building gameplay systems.

---

## Current Progress

| System | Status |
|--------|--------|
| Board generation (8×8 grid, world↔tile coordinate helpers) | Done |
| GameManager (singleton access hub) | Done |
| Player free movement (WASD, board-bounded) | Done |
| Player shooting | Done |
| Player burst attack | Deferred — post gray box |
| Player health | Not started |
| Enemy base class | Not started |
| Enemy AI & shooting patterns | Not started |
| Wave system | Not started |
| UI (HP bar, timer, wave roster) | Not started |

---

## Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| Unity Editor | **6000.3.11f1** | Install via Unity Hub. Match this exactly — Unity 6 asset serialization is not backwards-compatible with 2022/2023. |
| Git | Any recent | LFS not currently used. |
| IDE | Your choice | Rider, VS 2022, or VS Code with the Unity extension all work. Solution/project files are gitignored — your editor regenerates them on first open. |

---

## Getting Started

```bash
git clone <repo-url>
cd ChessInvaders
```

Open **Unity Hub → Open → Add project from disk** and point it at the repo root.
Let Unity import — the `Library/` cache builds on first open (~1-2 min).

> **Do not commit `Library/`, `Temp/`, or `UserSettings/`.** They are gitignored — machine-local and regenerated automatically.

---

## Project Layout

```
Assets/
├── Scripts/
│   ├── Core/       # GameManager, WaveManager, BoardManager
│   ├── Player/     # Movement, Shooting, Health
│   ├── Enemies/    # EnemyBase + per-piece subclasses
│   ├── Combat/     # Projectile, DamageSystem, AttackPatterns
│   ├── UI/         # HUD, HealthBar, WaveIndicator, TimerDisplay
│   └── Utils/      # Constants, Enums, GridHelpers
├── Prefabs/        # All spawnable objects — prefabs are the source of truth
├── Scenes/
│   ├── MainMenu.unity
│   └── Game.unity
└── Placeholder/    # Gray-box art. Final art goes in Assets/Art/ — don't overwrite these
```

---

## Key Architecture Rules

These are load-bearing — don't change them without syncing:

- **`BoardManager.TileSize`** — one serialized float that all grid-to-world math flows through. Never hardcode tile sizes or assume `1 unit = 1 tile`. Always go through `BoardManager.GridToWorld()` / `WorldToGrid()` helpers.
- **`GameManager`** — singleton access point for `BoardManager`, `WaveManager`, `PlayerHealth`. No `FindObjectOfType` anywhere else.
- **`WaveManager`** — central orchestrator for enemy step timer and spawning. Sync before modifying.
- **Prefabs are the source of truth** — all spawnable objects are prefabs. Nothing is built via `new GameObject()` in code. Designers swap art by updating the prefab, not touching scripts.
- **Entity prefab structure** — every entity (player, enemies) has a `Visual` child GameObject holding the SpriteRenderer. Gameplay scripts live on the root. This lets art be swapped by replacing the `Visual` child without touching any code.
- **Object pooling** — projectiles and intent indicators use `ObjectPool<T>`. Never call `Destroy()` on them; always return to pool.

---

## Branch & Workflow Conventions

- **`main`** — stable, always builds. PRs only, no direct pushes.
- **`dev`** — integration branch. Merge feature branches here first.
- **Feature branches** — `feature/<short-name>` (e.g. `feature/enemy-rook-ai`).
- Commit messages: imperative mood, lowercase (`add pawn shooting pattern`, `fix burst cooldown UI`).

### Scene merge conflicts

Unity scene files are YAML and merge badly.
**Only one person edits a scene at a time.** Coordinate before opening `Game.unity`.
If a change can be made inside a prefab instead of the scene, do it there.

---

## Adding a New Enemy Type

1. Subclass `EnemyBase` in `Assets/Scripts/Enemies/`
2. Create a ScriptableObject stat asset (HP, fire rate, speed) in `Assets/Data/`
3. Build a prefab in `Assets/Prefabs/Enemies/` with the `Visual` child structure
4. Add it to the relevant `WaveData` ScriptableObject — no code changes to `WaveManager`

---

## Questions / Sync Points

- Ping the primary dev before touching `GameManager`, `WaveManager`, or any scene file.
- New art or animations: replace the `Visual` child on the relevant prefab. Root scripts stay untouched.
