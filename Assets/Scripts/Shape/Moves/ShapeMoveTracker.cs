using System.Collections.Generic;

public class MoveTracker
{
    List<ShapeMove> _moves = new List<ShapeMove>();

    public void AddMove(ShapeMove move)
    {
        _moves.Add(move);
    }

    public Shape Undo()
    {
        if (_moves.Count == 0) return null;
        var move = _moves[_moves.Count - 1];
        _moves.RemoveAt(_moves.Count - 1);
        return move.Undo();
    }

    public ShapeMove Last => _moves[_moves.Count - 1];
    public int Moves => _moves.Count;
}