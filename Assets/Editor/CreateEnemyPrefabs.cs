using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class CreateEnemyPrefabs
{
    private const string PrefabFolder = "Assets/Prefabs/Enemies";
    private const string StatsFolder  = "Assets/ScriptableObjects";
    // Same sprite the Pawn prefab uses for its SpriteRenderer, Background, and Fill
    private const string SpriteGuid   = "5653e1b4421b1834396b8e733b3cad9c";

    [MenuItem("Chess Invaders/Create Enemy Prefabs")]
    public static void Create()
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            AssetDatabase.GUIDToAssetPath(SpriteGuid));

        if (sprite == null)
        {
            Debug.LogError("Could not find the shared sprite. Check SpriteGuid in CreateEnemyPrefabs.cs.");
            return;
        }

        CreatePrefab("EnemyPawn",   PieceType.Pawn,   "PawnStats",   sprite);
        CreatePrefab("EnemyRook",   PieceType.Rook,   "RookStats",   sprite);
        CreatePrefab("EnemyBishop", PieceType.Bishop, "BishopStats", sprite);
        CreatePrefab("EnemyKnight", PieceType.Knight, "KnightStats", sprite);
        CreatePrefab("EnemyQueen",  PieceType.Queen,  "QueenStats",  sprite);
        CreatePrefab("EnemyKing",   PieceType.King,   "KingStats",   sprite);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Enemy prefabs created in Assets/Prefabs/Enemies/");
    }

    private static void CreatePrefab(string name, PieceType pieceType, string statsName, Sprite sprite)
    {
        string path = $"{PrefabFolder}/{name}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.Log($"Skipped {name} — already exists. Delete it first to regenerate.");
            return;
        }

        EnemyStats stats = AssetDatabase.LoadAssetAtPath<EnemyStats>($"{StatsFolder}/{statsName}.asset");
        if (stats == null)
        {
            Debug.LogError($"Could not find {statsName}.asset — run Chess Invaders/Create Enemy Stats Assets first.");
            return;
        }

        // ── Root ────────────────────────────────────────────────────────────
        var root = new GameObject(name);
        root.layer = LayerMask.NameToLayer("Enemy");
        root.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        var enemyBase  = root.AddComponent<EnemyBase>();
        var col        = root.AddComponent<CircleCollider2D>();
        col.radius     = 0.35f;
        var shootingAI = root.AddComponent<ShootingAI>();

        // ── Visual ──────────────────────────────────────────────────────────
        var visual = new GameObject("Visual");
        visual.transform.SetParent(root.transform, false);
        visual.layer = root.layer;

        var sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Color.black;
        sr.sortingOrder = 1;

        var label = visual.AddComponent<PlaceholderLabel>();

        // ── HPBarAnchor (child of Visual) ───────────────────────────────────
        var hpAnchor = new GameObject("HPBarAnchor");
        hpAnchor.transform.SetParent(visual.transform, false);
        hpAnchor.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        hpAnchor.layer = root.layer;

        // ── Canvas (world space, child of HPBarAnchor) ──────────────────────
        var canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(hpAnchor.transform, false);
        canvasGO.layer = root.layer;

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGO.AddComponent<GraphicRaycaster>();

        var canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(0.8f, 0.1f);
        canvasRT.localScale = new Vector3(1.1111112f, 1.1111112f, 1f);

        // ── Background image ────────────────────────────────────────────────
        var bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        bg.layer = root.layer;
        var bgRT = bg.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;
        var bgImage = bg.AddComponent<Image>();
        bgImage.sprite = sprite;
        bgImage.color = new Color(0.157f, 0.157f, 0.157f, 1f);

        // ── Fill image ──────────────────────────────────────────────────────
        var fill = new GameObject("Fill");
        fill.transform.SetParent(canvasGO.transform, false);
        fill.layer = root.layer;
        var fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;
        var fillImage = fill.AddComponent<Image>();
        fillImage.sprite = sprite;
        fillImage.color = Color.red;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;
        fillImage.fillAmount = 1f;

        var hpBar = canvasGO.AddComponent<EnemyHPBar>();

        // ── Wire serialized fields ──────────────────────────────────────────
        var soBase = new SerializedObject(enemyBase);
        soBase.FindProperty("_stats").objectReferenceValue = stats;
        soBase.FindProperty("_spriteRenderer").objectReferenceValue = sr;
        soBase.ApplyModifiedPropertiesWithoutUndo();

        var soAI = new SerializedObject(shootingAI);
        soAI.FindProperty("_pieceType").intValue = (int)pieceType;
        soAI.FindProperty("_forwardDir").FindPropertyRelative("x").intValue = 0;
        soAI.FindProperty("_forwardDir").FindPropertyRelative("y").intValue = -1;
        soAI.ApplyModifiedPropertiesWithoutUndo();

        var soLabel = new SerializedObject(label);
        soLabel.FindProperty("_pieceType").intValue = (int)pieceType;
        soLabel.FindProperty("_spriteRenderer").objectReferenceValue = sr;
        soLabel.ApplyModifiedPropertiesWithoutUndo();

        var soHpBar = new SerializedObject(hpBar);
        soHpBar.FindProperty("_enemy").objectReferenceValue = enemyBase;
        soHpBar.FindProperty("_fill").objectReferenceValue = fillImage;
        soHpBar.FindProperty("_hideDelay").floatValue = 2f;
        soHpBar.ApplyModifiedPropertiesWithoutUndo();

        // ── Save prefab and clean up scene ──────────────────────────────────
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }
}
