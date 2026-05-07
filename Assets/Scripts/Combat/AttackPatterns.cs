using System;
using System.Collections.Generic;
using UnityEngine;

public static class AttackPatterns
{
    // forwardDir: direction this piece considers "forward" (toward opponent).
    // For enemies moving toward the player at y=0: pass new Vector2Int(0, -1).
    // isBlocked: optional predicate — sliding rays stop when it returns true (Knights ignore it).
    public static List<Vector2Int> GetThreatenedTiles(
        PieceType type, Vector2Int origin, int boardSize, Vector2Int forwardDir,
        Func<Vector2Int, bool> isBlocked = null)
    {
        return type switch
        {
            PieceType.Pawn   => Pawn(origin, boardSize, forwardDir),
            PieceType.Rook   => Sliding(origin, boardSize, CardinalDirs(), isBlocked),
            PieceType.Bishop => Sliding(origin, boardSize, DiagonalDirs(), isBlocked),
            PieceType.Knight => KnightTiles(origin, boardSize),
            PieceType.Queen  => Sliding(origin, boardSize, AllDirs(), isBlocked),
            PieceType.King   => KingTiles(origin, boardSize),
            _                => new List<Vector2Int>()
        };
    }

    private static List<Vector2Int> Pawn(Vector2Int origin, int boardSize, Vector2Int forward)
    {
        // Diagonal-forward two tiles: rotate forward 90° CW and CCW, add to forward
        var perpCW  = new Vector2Int( forward.y, -forward.x);
        var perpCCW = new Vector2Int(-forward.y,  forward.x);
        var tiles = new List<Vector2Int>(2);
        TryAdd(origin + forward + perpCW,  boardSize, tiles);
        TryAdd(origin + forward + perpCCW, boardSize, tiles);
        return tiles;
    }

    private static List<Vector2Int> Sliding(Vector2Int origin, int boardSize,
        Vector2Int[] directions, Func<Vector2Int, bool> isBlocked)
    {
        var tiles = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            var pos = origin + dir;
            while (InBounds(pos, boardSize))
            {
                if (isBlocked != null && isBlocked(pos)) break;
                tiles.Add(pos);
                pos += dir;
            }
        }
        return tiles;
    }

    private static List<Vector2Int> KnightTiles(Vector2Int origin, int boardSize)
    {
        Vector2Int[] offsets =
        {
            new( 1,  2), new( 2,  1),
            new( 2, -1), new( 1, -2),
            new(-1, -2), new(-2, -1),
            new(-2,  1), new(-1,  2)
        };
        var tiles = new List<Vector2Int>(8);
        foreach (var o in offsets) TryAdd(origin + o, boardSize, tiles);
        return tiles;
    }

    private static List<Vector2Int> KingTiles(Vector2Int origin, int boardSize)
    {
        var tiles = new List<Vector2Int>(8);
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                if (dx != 0 || dy != 0)
                    TryAdd(origin + new Vector2Int(dx, dy), boardSize, tiles);
        return tiles;
    }

    private static Vector2Int[] CardinalDirs() => new[]
    {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };

    private static Vector2Int[] DiagonalDirs() => new[]
    {
        new Vector2Int( 1,  1), new Vector2Int( 1, -1),
        new Vector2Int(-1,  1), new Vector2Int(-1, -1)
    };

    private static Vector2Int[] AllDirs()
    {
        var dirs = new Vector2Int[8];
        CardinalDirs().CopyTo(dirs, 0);
        DiagonalDirs().CopyTo(dirs, 4);
        return dirs;
    }

    private static void TryAdd(Vector2Int pos, int boardSize, List<Vector2Int> list)
    {
        if (InBounds(pos, boardSize)) list.Add(pos);
    }

    // Sliding pieces fire until they leave the board. Fixed-range pieces stop at the target tile.
    public static bool IsSliding(PieceType type) =>
        type is PieceType.Rook or PieceType.Bishop or PieceType.Queen;

    private static bool InBounds(Vector2Int pos, int boardSize) =>
        pos.x >= 0 && pos.x < boardSize && pos.y >= 0 && pos.y < boardSize;
}
