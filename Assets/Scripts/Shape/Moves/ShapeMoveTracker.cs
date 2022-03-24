using System.Collections.Generic;

public class MoveTracker
{
    readonly List<ShapeMove> _moves = new();

    public void AddMove(ShapeMove move)
    {
        _moves.Add(move);
    }

    public Shape Undo()
    {
        if (_moves.Count == 0) return null;
        var move = _moves[^1];
        _moves.RemoveAt(_moves.Count - 1);
        return move.Undo();
    }

    public ShapeMove Last => _moves[^1];
    public ShapeMove First => _moves[0];
    public int Moves => _moves.Count;
}