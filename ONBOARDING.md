Chess Invaders — ONBOARDING.md

This file is for collaborators joining the project. It is updated regularly as the project evolves.
Last updated: 2026-05-08
Current stage: Gray box — Phases 1–3 complete, Phase 4 (wave system) in progress


What Is This Game
Chess × Space Invaders. The player controls a King piece with free continuous movement on an 8×8 board. Enemy chess pieces spawn at the far end and advance one tile toward the player on a shared global timer. Each enemy shoots in their chess movement pattern, telegraphed by a visible indicator before firing. Kill all enemies to clear a wave, survive 3 waves, kill the enemy King = win. Die at any point = instant game over.

Repo Setup
  git clone [REPO URL]
Open in Unity 6 (6000.3.11f1). No additional package installs required.
Render pipeline: Built-in 2D
Target platforms: PC (primary), WebGL (secondary)

Project Structure
Assets/
├── Scripts/
│   ├── Core/            # GameManager, WaveManager, BoardManager, PoolManager
│   ├── Player/          # PlayerMovement, PlayerShooting, PlayerHealth
│   ├── Enemy/           # EnemyBase, ShootingAI, AttackPatterns, IntentIndicator
│   ├── Combat/          # Projectile
│   ├── Data/            # WaveData ScriptableObject
│   ├── UI/              # StepTimerUI
│   └── Utils/           # PlaceholderLabel, Layers
├── Prefabs/
│   ├── Player/
│   └── Enemies/         # EnemyPawn, EnemyRook, EnemyBishop, EnemyKnight, EnemyQueen, EnemyKing
├── ScriptableObjects/
│   ├── PlayerStats.asset
│   ├── EnemyStats_[PieceType].asset  (one per piece type)
│   ├── WaveData_[1-3].asset
│   └── GamePalette.asset
└── Scenes/
    └── SampleScene.unity


Current State

| System | Status |
|--------|--------|
| Board generation (8×8) | ✅ Done |
| GameManager | ✅ Done |
| Player movement + shooting | ✅ Done |
| Player health + HP bar | ✅ Done |
| Enemy base (all 6 pieces) | ✅ Done |
| Enemy AI + shooting patterns | ✅ Done |
| Wave system (WaveManager) | 🔄 In progress |
| Rank breach + pawn promotion | ✅ Done |
| Step timer UI | ✅ Done |
| Wave roster UI | ❌ Not started |
| Win / game over screens | ❌ Not started (stubs only) |


Key Architecture Decisions
Read these before writing any code.

--- GameManager is the static access hub ---
GameManager is a singleton with static properties for every top-level system:
  GameManager.Board        → BoardManager
  GameManager.Pool         → PoolManager
  GameManager.Palette      → GamePalette
  GameManager.Player       → player Transform
  GameManager.PlayerHealth → PlayerHealth
  GameManager.Waves        → WaveManager

All cross-system references go through GameManager. There is no FindObjectOfType anywhere at runtime. Wire everything in the inspector.

--- BoardManager is the grid authority ---
All world↔tile coordinate conversion goes through BoardManager helpers. BoardManager.tileSize is the single value that controls all spatial scaling. Never write raw grid arithmetic anywhere else. Changing tileSize rescales the entire game.

--- Visual child pattern ---
Every entity prefab splits gameplay and visuals into two GameObjects:

  EnemyPawn (root)        ← EnemyBase, ShootingAI, Collider2D
    └── Visual            ← SpriteRenderer, PlaceholderLabel
          └── HPBarAnchor ← world-space HP bar canvas

Gameplay scripts live on root. Visual scripts live on the Visual child. Swapping art = replace the Visual child. No script changes needed.

--- ScriptableObjects for all tuning ---
HP, speeds, cooldowns, damage, fire rates, telegraph durations — all live in ScriptableObject assets in Assets/ScriptableObjects/. Never hardcode numeric values in scripts. Each enemy piece has its own EnemyStats asset. Player has PlayerStats.

--- Object pooling for all projectiles and indicators ---
Player projectiles, enemy projectiles, and intent indicators all use Unity's ObjectPool<T> via PoolManager. Never Instantiate/Destroy these objects. PoolManager holds all pools with serialized size fields. Pooled objects implement OnSpawn() and OnDespawn() to reset state.

--- Enemy AI: coroutine-based probability ramp ---
ShootingAI runs a shoot loop coroutine (not Update). Each iteration it:
1. Gets the set of tiles threatened by this piece type from AttackPatterns (static utility)
2. Measures sqrMagnitude from the player to the nearest threatened tile
3. Lerps shoot probability between idleProbability and alertProbability based on proximity
4. On a successful roll: shows intent indicator, waits telegraphDuration, fires projectile

This means enemies are more aggressive when the player is in their line of fire, and passive otherwise. All thresholds and probabilities are in EnemyStats.

--- Wave system ---
WaveData (ScriptableObject) defines one wave: a list of InitialSpawns (prefab + tile) and a list of ReinforcementBatches. Batch[0] spawns after the first step tick, batch[1] after the second, etc., always at the back rank.

WaveManager owns the global step timer. On each tick, it moves ALL enemies forward one tile in a single loop — enemies do not run their own timers. Wave clear is detected by comparing _deadEnemies against _spawnedEnemies (only actually-spawned enemies count, so killing all on-board enemies before reinforcements arrive correctly ends the wave).

--- Rank breach and pawn promotion ---
If an enemy steps off the bottom of the board (rank 0), it:
  1. Deals _rankBreachDamage to the player
  2. Resets to full HP
  3. Teleports to the back rank at the same column
  4. If it was a Pawn: promotes to Queen (ShootingAI.Promote() changes firing pattern immediately; PlaceholderLabel.Refresh() updates color)

--- GamePalette for all colors ---
All runtime colors (projectile colors, intent highlight) are defined in GamePalette.asset. Edit that one asset to retheme anything. PlaceholderLabel is the only exception — it uses hardcoded placeholder colors by design and should be removed when final art arrives.


Integrating Designer Assets
When you receive final art assets:

1. Place sprites/sheets in Assets/Art/Final/ — do not touch Assets/Art/Placeholder/
2. For each entity: open its prefab → select the Visual child → update the SpriteRenderer sprite reference
3. Add an Animator to the Visual child and assign the animation controller
4. Adjust HPBarAnchor child position if the new sprite has different dimensions
5. Remove PlaceholderLabel from the Visual child
6. If tile size changes: update BoardManager.tileSize — everything rescales automatically
7. To retheme colors: edit GamePalette.asset only

No script changes should be needed for an art swap.


Working on This Repo

Scenes cause merge conflicts. Talk to the team before editing SampleScene.unity at the same time as someone else.

Adding a new enemy type: Create an EnemyStats asset in Assets/ScriptableObjects/, create a prefab following the Visual child pattern (root: EnemyBase + ShootingAI + Collider2D, Visual child: SpriteRenderer + PlaceholderLabel), set PieceType on ShootingAI, add it to the relevant WaveData asset. No changes to GameManager or WaveManager needed. You can also run Chess Invaders → Create Enemy Prefabs from the menu bar to regenerate all standard piece prefabs from scratch.

Don't modify GameManager or WaveManager without syncing with the team — these are central orchestrators.

Prefer prefab variants over runtime parameterization. If wave 3 needs a stronger pawn, make a prefab variant with a different EnemyStats asset — don't add conditional logic to the pawn script.


What's Out of Scope (for now)

Player burst attack (deferred post gray box)
Sound / music
Main menu
Save/load
Difficulty settings
Gamepad support


Questions / Contact
[FILL IN — who to ping for what]
