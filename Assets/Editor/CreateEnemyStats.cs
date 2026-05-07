using UnityEditor;
using UnityEngine;

public static class CreateEnemyStats
{
    [MenuItem("Chess Invaders/Create Enemy Stats Assets")]
    public static void Create()
    {
        const string folder = "Assets/ScriptableObjects";

        CreateAsset(folder, "RookStats",
            hp: 40,  fireInterval: 4f,   projSpeed: 6f, telegraph: 1.0f,
            alertTiles: 5f, idle: 0.20f, alert: 0.80f);

        CreateAsset(folder, "BishopStats",
            hp: 30,  fireInterval: 3.5f, projSpeed: 5f, telegraph: 1.0f,
            alertTiles: 4f, idle: 0.15f, alert: 0.70f);

        CreateAsset(folder, "KnightStats",
            hp: 25,  fireInterval: 5f,   projSpeed: 3f, telegraph: 1.5f,
            alertTiles: 3f, idle: 0.10f, alert: 0.60f);

        CreateAsset(folder, "QueenStats",
            hp: 60,  fireInterval: 5f,   projSpeed: 5f, telegraph: 1.2f,
            alertTiles: 6f, idle: 0.25f, alert: 0.90f);

        CreateAsset(folder, "KingStats",
            hp: 100, fireInterval: 3f,   projSpeed: 4f, telegraph: 1.0f,
            alertTiles: 4f, idle: 0.40f, alert: 1.00f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Enemy Stats assets created in Assets/ScriptableObjects/");
    }

    private static void CreateAsset(string folder, string name,
        float hp, float fireInterval, float projSpeed, float telegraph,
        float alertTiles, float idle, float alert, int damage = 20)
    {
        string path = $"{folder}/{name}.asset";
        if (AssetDatabase.LoadAssetAtPath<EnemyStats>(path) != null)
        {
            Debug.Log($"Skipped {name} — already exists.");
            return;
        }

        var asset = ScriptableObject.CreateInstance<EnemyStats>();
        var so    = new SerializedObject(asset);

        so.FindProperty("_maxHp").floatValue                 = hp;
        so.FindProperty("_fireInterval").floatValue          = fireInterval;
        so.FindProperty("_projectileSpeedInTiles").floatValue = projSpeed;
        so.FindProperty("_telegraphDuration").floatValue     = telegraph;
        so.FindProperty("_alertThresholdInTiles").floatValue = alertTiles;
        so.FindProperty("_idleShotProbability").floatValue   = idle;
        so.FindProperty("_alertShotProbability").floatValue  = alert;
        so.FindProperty("_projectileDamage").intValue        = damage;

        so.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.CreateAsset(asset, path);
    }
}
