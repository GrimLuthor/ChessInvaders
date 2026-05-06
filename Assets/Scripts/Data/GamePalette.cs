using UnityEngine;

[CreateAssetMenu(menuName = "Chess Invaders/Game Palette", fileName = "GamePalette")]
public class GamePalette : ScriptableObject
{
    [SerializeField] private Color _playerProjectileColor  = Color.white;
    [SerializeField] private Color _enemyProjectileColor   = Color.red;
    [SerializeField] private Color _intentHighlightColor   = new Color(1f, 0f, 0f, 0.5f);

    public Color PlayerProjectileColor => _playerProjectileColor;
    public Color EnemyProjectileColor  => _enemyProjectileColor;
    public Color IntentHighlightColor  => _intentHighlightColor;
}
