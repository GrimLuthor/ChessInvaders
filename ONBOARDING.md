Chess Invaders — ONBOARDING.md

This file is for collaborators joining the project. It is updated regularly as the project evolves.
Last updated: [DATE]
Current stage: Gray box


What Is This Game
Chess × Space Invaders. Player controls a King piece with free movement on an 8×8 board. Enemy chess pieces advance one tile forward on a shared timer and shoot in their chess movement patterns. Kill the enemy King in wave 3 to win.

Repo Setup
bashgit clone [REPO URL]
Open in Unity [VERSION]. No additional package installs required — all dependencies are in the project.
Unity version: [FILL IN]
Render pipeline: 2D (URP / Built-in — fill in)
Target platforms: PC, WebGL

Project Structure
Assets/
├── Scripts/
│   ├── Core/            # GameManager, WaveManager, BoardManager
│   ├── Player/          # PlayerMovement, PlayerShooting, PlayerHealth
│   ├── Enemies/         # EnemyBase, per-piece components, ShootingAI
│   ├── Combat/          # Projectile, AttackPatterns, IntentIndicator
│   ├── UI/              # HUD, WaveRosterUI, ScreenManager
│   └── Utils/           # Constants, Enums, GridHelpers
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Projectiles/
│   └── UI/
├── ScriptableObjects/
│   ├── PlayerStats.asset
│   ├── EnemyStats_[PieceType].asset  (one per enemy type)
│   ├── WaveData_[1-3].asset
│   ├── GamePalette.asset
│   └── AudioLibrary.asset (when audio is added)
├── Art/
│   ├── Placeholder/     # Gray box sprites — do not overwrite
│   └── Final/           # Designer assets go here when delivered
└── Scenes/
└── Game.unity

Current State
SystemStatusBoard generation✅ DoneGameManager✅ DonePlayer movement✅ DonePlayer shooting✅ DonePlayer health[UPDATE]Enemy base[UPDATE]Enemy AI & shooting[UPDATE]Wave system[UPDATE]UI[UPDATE]

Key Architecture Decisions
Read these before writing any code.
BoardManager is the grid authority
All world↔tile coordinate conversion goes through BoardManager helpers. BoardManager.tileSize is the single value that controls all spatial scaling. Never write raw grid arithmetic anywhere else.
Visual child pattern
Every entity prefab has a Visual child GameObject holding the SpriteRenderer (and eventually Animator). Gameplay scripts live on the root. This means art can be swapped by replacing the Visual child — no script changes needed.
EnemyPawn (root)       ← EnemyBase, ShootingAI, Collider2D
└── Visual           ← SpriteRenderer, Animator
└── HPBarAnchor
ScriptableObjects for all tuning
HP, speeds, cooldowns, damage, fire rates — all live in ScriptableObject assets in ScriptableObjects/. Never hardcode numeric values in scripts.
Object pooling for projectiles and indicators
Player projectiles, enemy projectiles, and intent indicators all use Unity's ObjectPool<T>. Never Instantiate/Destroy these. PoolManager holds all pools with serialized size fields.
GamePalette for all colors
All colors (projectile colors, intent highlight, UI accent) are defined in GamePalette.asset. Edit that one asset to retheme anything visual.

Integrating Designer Assets
When you receive final art assets:

Place sprites/sheets in Assets/Art/Final/ — do not touch Assets/Art/Placeholder/
For each entity: open its prefab → select the Visual child → update the SpriteRenderer sprite reference
Add an Animator to the Visual child and assign the animation controller
Adjust HPBarAnchor child position if the new sprite has different dimensions
If tile size changes: update BoardManager.tileSize — everything rescales automatically
To retheme colors: edit GamePalette.asset only

No script changes should be needed for an art swap.

Working on This Repo

Scenes cause merge conflicts. Talk to the team before editing Game.unity at the same time as someone else.
Adding a new enemy type: Subclass EnemyBase (or add a component alongside it), create an EnemyStats ScriptableObject asset, create a prefab following the Visual child pattern, add it to the relevant WaveData asset. No changes to GameManager or WaveManager needed.
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
