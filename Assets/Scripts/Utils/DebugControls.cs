using UnityEngine;
using UnityEngine.InputSystem;

public class DebugControls : MonoBehaviour
{
    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.jKey.wasPressedThisFrame)
            KillAllEnemies();

        if (kb.kKey.wasPressedThisFrame)
            WaveManager.Instance.SkipStep();
    }

    private void KillAllEnemies()
    {
        // Copy to list — ActiveEnemies shrinks as enemies die during iteration.
        var enemies = new System.Collections.Generic.List<EnemyBase>(EnemyBase.ActiveEnemies);
        foreach (var enemy in enemies)
            enemy.TakeDamage(float.MaxValue);
    }
}
