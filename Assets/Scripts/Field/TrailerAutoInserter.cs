using System;
using UnityEngine;

public class TrailerAutoInserter : MonoBehaviour
{
    [SerializeField] int[] moves;
    [SerializeField] FieldMatrix field;
    [SerializeField] int ind;

    public bool isDone;

    public void MoveToNext()
    {
        for (var i = 0; i < Mathf.Abs(moves[ind]); i++)
            field.MoveAttachedShape(moves[ind] > 0);
        ind++;
    }

    public void Insert()
    {
        field.InsertShape();
        if (ind >= moves.Length) isDone = true;
    }
}