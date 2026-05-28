// ============================================================
//  UPGRADE SYSTEM - SETUP GUIDE
//  Read this top to bottom. Each step builds on the last.
// ============================================================


// ────────────────────────────────────────────────────────────
//  STEP 1 — Player GameObject
// ────────────────────────────────────────────────────────────
//  Find your Player in the scene and add the PlayerStats component.
//
//  Inspector settings on PlayerStats:
//    - Helper Prefab        → leave empty, come back after Step 4
//    - Helper Orbit Radius  → 2       (distance helpers orbit from player)
//    - Helper Orbit Speed   → 60      (degrees per second)


// ────────────────────────────────────────────────────────────
//  STEP 2 — Projectile Prefab
// ────────────────────────────────────────────────────────────
//  Find your projectile prefab in the Project window.
//  Add the ProjectileDamageApplier component to it.
//
//  No fields to set — it auto-finds the Projectile component on
//  the same object and patches the damage field on every spawn.


// ────────────────────────────────────────────────────────────
//  STEP 3 — Create Upgrade ScriptableObject Assets
// ────────────────────────────────────────────────────────────
//  Right-click in Project window → Create → Upgrades → one of each:
//
//    Menu Path                  Suggested Asset Name
//    ─────────────────────────  ────────────────────
//    Upgrades/Attack Speed      SO_AttackSpeed
//    Upgrades/Move Speed        SO_MoveSpeed
//    Upgrades/Damage            SO_Damage
//    Upgrades/Health & Regen    SO_HealthRegen
//    Upgrades/Extra Helper      SO_ExtraHelper
//    Upgrades/Melee Orbit       SO_MeleeOrbit
//    Upgrades/Bomb Orbit        SO_BombOrbit
//    Upgrades/Laser             SO_Laser
//
//  Click each asset and fill in:
//    - Upgrade Name  → text shown on the card  (e.g. "Attack Speed")
//    - Description   → one line on the card    (e.g. "+25% fire rate")
//    - Icon          → optional sprite
//    - Prefab/radius fields → come back after Steps 4–7


// ────────────────────────────────────────────────────────────
//  STEP 4 — Helper Prefab  (for Extra Helper upgrade)
// ────────────────────────────────────────────────────────────
//  Create a new prefab. Add these components:
//
//    SpriteRenderer      → assign any sprite (small player copy or different)
//    Rigidbody2D         → Body Type: Kinematic
//    HelperOrbit         → no fields, initialized at runtime
//    HelperShooter       → set:
//                            Projectile Prefab  = same prefab as the player
//                            Enemy Layer        = your enemy layer
//                            Attack Radius      = 5
//                            Base Fire Rate     = 2
//                            Spawn Radius       = 0.5
//
//  Then on the Player → PlayerStats, assign this prefab to Helper Prefab.
//  SO_ExtraHelper needs no prefab field — it uses the one on PlayerStats.


// ────────────────────────────────────────────────────────────
//  STEP 5 — Melee Weapon Prefab  (for Melee Orbit upgrade)
// ────────────────────────────────────────────────────────────
//  Create a new prefab. Add these components:
//
//    SpriteRenderer          → assign a sword / blade sprite
//    CircleCollider2D        → check Is Trigger
//    Rigidbody2D             → Body Type: Kinematic
//    HelperOrbit             → no fields, initialized at runtime
//    MeleeOrbitWeapon        → set:
//                                Base Damage     = 2
//                                Damage Cooldown = 0.5   (sec between hits per enemy)
//                                Enemy Layer     = your enemy layer
//
//  On SO_MeleeOrbit, assign:
//    - Melee Weapon Prefab  = this prefab
//    - Orbit Radius         = 1.5
//    - Orbit Speed          = 150


// ────────────────────────────────────────────────────────────
//  STEP 6 — Bomb Prefab  (for Bomb Orbit upgrade)
// ────────────────────────────────────────────────────────────
//  Create a new prefab. Add these components:
//
//    SpriteRenderer      → assign a bomb sprite
//    CircleCollider2D    → check Is Trigger, radius ≈ 0.3
//    Rigidbody2D         → Body Type: Kinematic
//    HelperOrbit         → no fields, initialized at runtime
//    BombOrbitWeapon     → set:
//                            Base Damage       = 5
//                            Explosion Radius  = 1.5
//                            Respawn Delay     = 3
//                            Enemy Layer       = your enemy layer
//                            Explosion VFX     = optional, leave empty for now
//
//  On SO_BombOrbit, assign:
//    - Bomb Prefab   = this prefab
//    - Bomb Count    = 3      (spawns 3 bombs evenly spaced)
//    - Orbit Radius  = 2.5
//    - Orbit Speed   = 90


// ────────────────────────────────────────────────────────────
//  STEP 7 — Laser Prefab  (for Laser upgrade)
// ────────────────────────────────────────────────────────────
//  The laser sits at the player position and rotates a beam outward.
//
//  Create a new prefab. Add these components:
//
//    SpriteRenderer        → thin stretched rectangle/beam sprite
//    BoxCollider2D         → check Is Trigger, then set:
//                              Offset X = half laser length  (e.g. 2.5)
//                              Offset Y = 0
//                              Size X   = full laser length  (e.g. 5)
//                              Size Y   = 0.25
//    Rigidbody2D           → Body Type: Kinematic
//    LaserOrbitWeapon      → set:
//                              Base Damage      = 1
//                              Rotation Speed   = 180   (deg/sec)
//                              Damage Cooldown  = 0.15
//                              Enemy Layer      = your enemy layer
//
//  NOTE: The X offset is critical — it shifts the collider so the beam
//  starts at the center and extends outward, sweeping like a clock hand.
//
//  On SO_Laser, assign:
//    - Laser Prefab = this prefab


// ────────────────────────────────────────────────────────────
//  STEP 8 — Upgrade UI  (Canvas setup)
// ────────────────────────────────────────────────────────────
//  In your scene, create or find your Canvas. Inside it:
//
//  A) Create the panel:
//       - Add a Panel or Image → name it "UpgradePanel"
//       - Add UpgradeUI component to it
//       - Set it INACTIVE in the Inspector (manager shows/hides it)
//
//  B) Inside UpgradePanel, create 3 card child objects. Each card needs:
//       - Button component
//       - Child TextMeshPro object → upgrade name
//       - Child TextMeshPro object → description
//       - Child Image              → icon (optional)
//       - UpgradeCard component → wire up nameText, descriptionText,
//                                 iconImage, and button in the Inspector
//
//  C) On the UpgradeUI component, assign:
//       - Panel  → drag UpgradePanel itself
//       - Cards  → set size to 3, drag in the 3 card objects


// ────────────────────────────────────────────────────────────
//  STEP 9 — UpgradeManager GameObject
// ────────────────────────────────────────────────────────────
//  Create a new empty GameObject → name it "UpgradeManager".
//  Add the UpgradeManager component. In the Inspector:
//
//    - Upgrade Pool  → set size to 8, drag in all 8 SO assets from Step 3
//    - Upgrade UI    → drag the UpgradePanel from your Canvas


// ────────────────────────────────────────────────────────────
//  STEP 10 — Connect WaveManager  (done by teammate)
// ────────────────────────────────────────────────────────────
//  When a wave clears, WaveManager should call:
//
//    UpgradeManager.Instance.TriggerUpgradeSelection(StartNextWave);
//
//  TriggerUpgradeSelection pauses the game, shows 3 random upgrades,
//  and calls StartNextWave automatically after the player picks one.


// ────────────────────────────────────────────────────────────
//  QUICK CHECKLIST
// ────────────────────────────────────────────────────────────
//  [ ] PlayerStats on player, helperPrefab assigned
//  [ ] ProjectileDamageApplier on projectile prefab
//  [ ] 8 upgrade SO assets created and filled in
//  [ ] Helper prefab    (HelperOrbit + HelperShooter)
//  [ ] Melee prefab     (HelperOrbit + MeleeOrbitWeapon + trigger collider)
//  [ ] Bomb prefab      (HelperOrbit + BombOrbitWeapon  + trigger collider)
//  [ ] Laser prefab     (LaserOrbitWeapon + BoxCollider2D with X offset)
//  [ ] Each weapon SO has its prefab assigned
//  [ ] Canvas → UpgradePanel (inactive) with 3 wired UpgradeCard children
//  [ ] UpgradeManager GameObject → 8 SOs in pool + UpgradeUI assigned
//  [ ] WaveManager calls TriggerUpgradeSelection when wave clears
