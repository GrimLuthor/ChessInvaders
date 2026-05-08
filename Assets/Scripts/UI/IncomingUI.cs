using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncomingUI : MonoBehaviour
{
    [SerializeField] private Transform  _container;      // VerticalLayoutGroup
    [SerializeField] private GameObject _batchRowPrefab; // empty GameObject with HorizontalLayoutGroup
    [SerializeField] private GameObject _pieceIconPrefab; // Image root + TMP_Text child

    private readonly List<GameObject> _rows = new();

    private void Start()
    {
        WaveManager.Instance.OnWaveStarted += Rebuild;
        WaveManager.Instance.OnStepFired   += Rebuild;
        Rebuild();
    }

    private void OnDestroy()
    {
        if (WaveManager.Instance == null) return;
        WaveManager.Instance.OnWaveStarted -= Rebuild;
        WaveManager.Instance.OnStepFired   -= Rebuild;
    }

    private void Rebuild()
    {
        foreach (var row in _rows)
            Destroy(row);
        _rows.Clear();

        bool any = false;
        foreach (var batch in WaveManager.Instance.GetUpcomingBatches())
        {
            if (batch.Spawns.Count == 0) continue;
            any = true;

            var row = Instantiate(_batchRowPrefab, _container);
            _rows.Add(row);

            foreach (var spawn in batch.Spawns)
            {
                var ai = spawn.Prefab.GetComponent<ShootingAI>();
                if (ai == null) continue;

                var icon = Instantiate(_pieceIconPrefab, row.transform);
                icon.GetComponent<Image>().color                  = ColorFor(ai.PieceType);
                icon.GetComponentInChildren<TMP_Text>().text      = LetterFor(ai.PieceType);
            }
        }

        gameObject.SetActive(any);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_container as RectTransform);
    }

    private static Color ColorFor(PieceType type) => type switch
    {
        PieceType.Pawn   => new Color(0.6f, 0.6f, 0.6f),
        PieceType.Rook   => new Color(0.2f, 0.6f, 1.0f),
        PieceType.Bishop => new Color(0.2f, 0.9f, 0.4f),
        PieceType.Knight => new Color(1.0f, 0.8f, 0.2f),
        PieceType.Queen  => new Color(0.9f, 0.2f, 0.9f),
        PieceType.King   => new Color(1.0f, 0.3f, 0.3f),
        _                => Color.white
    };

    private static string LetterFor(PieceType type) => type switch
    {
        PieceType.Pawn   => "P",
        PieceType.Rook   => "R",
        PieceType.Bishop => "B",
        PieceType.Knight => "N",
        PieceType.Queen  => "Q",
        PieceType.King   => "K",
        _                => "?"
    };
}
