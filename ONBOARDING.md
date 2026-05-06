# Chess Invaders — Collaborator Onboarding

Welcome. This doc gets you to a running project in under 10 minutes.  
Full design and architecture lives in **[CLAUDE.md](CLAUDE.md)** — read that after this.

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

> **Do not commit `Library/`, `Temp/`, or `UserSettings/`.**  
> They are gitignored for a reason — they are machine-local and regenerated automatically.

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

See `CLAUDE.md §Architecture Guidelines` for naming conventions, ScriptableObject usage, object pooling rules, and the `BoardManager.tileSize` contract that every system depends on.

---

## Branch & Workflow Conventions

- **`main`** — stable, always builds. PRs only, no direct pushes.
- **`dev`** — integration branch. Merge feature branches here first.
- **Feature branches** — `feature/<short-name>` (e.g. `feature/enemy-rook-ai`).
- Commit messages: imperative mood, lowercase (`add pawn shooting pattern`, `fix burst cooldown UI`).

### Scene merge conflicts

Unity scene files (`.unity`) are YAML and merge badly.  
**Rule: only one person edits a scene at a time.** Coordinate in chat before opening `Game.unity`.  
Prefer prefab-based workflows — if a change can be made in a prefab instead of the scene, do it there.

---

## Key Architecture Contracts

A few things that everything else depends on — don't change these without a sync:

- **`BoardManager.tileSize`** — single serialized float that all grid-to-world math flows through. Never assume `1 unit = 1 tile` in code; always go through `BoardManager` helpers.
- **`GameManager`** — singleton-style access point for `BoardManager`, `WaveManager`, `PlayerHealth`. Don't add `FindObjectOfType` calls elsewhere.
- **`WaveManager`** — central orchestrator for the step timer and enemy spawning. Sync before modifying.
- **Object pooling** — projectiles and intent indicators use `ObjectPool<T>` from day one. Never call `Destroy()` on them; always return to pool.

---

## Using Claude Code (optional)

The primary dev uses [Claude Code](https://claude.ai/code) (an AI coding assistant CLI) with project instructions in `CLAUDE.md`.  
You don't need to use it, but if you do:
- Install: `npm install -g @anthropic-ai/claude-code`
- Run `claude` in the repo root — it picks up `CLAUDE.md` automatically.
- Your personal Claude settings live in `.claude/` (gitignored) — they won't affect others.

---

## Questions / Sync Points

- Ping the primary dev before touching `GameManager`, `WaveManager`, or any scene file.
- New enemy type? Subclass `EnemyBase`, create a `ScriptableObject` stat asset, add to the relevant `WaveData`. See `CLAUDE.md §Wave System`.
- New art or animations? Replace the `Visual` child GameObject on the relevant prefab. Root gameplay scripts stay untouched.
