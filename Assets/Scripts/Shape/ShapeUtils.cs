using System;
using UnityEngine;

public static class ShapeUtils
{
    public class ShapeCoordsTranslator
    {
        Shape _shape;
        internal enum Type
        {
            Field, LocalFieldRotation, LocalShapeRotation,   
        }

        Type _from;
        Vector2Int _pos;
        internal ShapeCoordsTranslator(Type from, Shape shape, Vector2Int position)
        {
            _from = from;
            _shape = shape;
            _pos = position;
        }

        public Vector2Int ToField()
        {
            switch (_from)
            {
                case Type.LocalFieldRotation:
                    return _shape.pos + _pos;
                case Type.LocalShapeRotation:
                    return _shape.pos + FromShapeToFieldRotation(_pos);
                case Type.Field:
                    return _pos;
            }
            throw new Exception("Couldn't translate coords");
        }

        public Vector2Int ToLocalShapeRotation()
        {
            switch (_from)
            {
                case Type.LocalFieldRotation:
                    return FromFieldToShapeRotation(_pos);
                case Type.LocalShapeRotation:
                    return _pos;
                case Type.Field:
                    return FromFieldToShapeRotation(_pos - _shape.pos);
            }
            throw new Exception("Couldn't translate coords");
        }

        public Vector2Int ToLocalFieldRotation()
        {
            switch (_from)
            {
                case Type.LocalFieldRotation:
                    return _pos;
                case Type.LocalShapeRotation:
                    return FromShapeToFieldRotation(_pos);
                case Type.Field:
                    return _pos - _shape.pos;
            }
            throw new Exception("Couldn't translate coords");
        }

        Vector2Int FromShapeToFieldRotation(Vector2Int pos)
        {
            var shape = FieldMatrix.current.attachedShape;
            return PositionToRotation(Vector2Int.up, shape.size, pos, shape.UpDirection);
        }

        Vector2Int FromFieldToShapeRotation(Vector2Int pos)
        {
            var shape = FieldMatrix.current.attachedShape;
            return PositionToRotation(shape.UpDirection, shape.size, pos, Vector2Int.up);
        }

        Vector2Int PositionToRotation(Vector2Int originalDir, Vector2Int originalSize, Vector2Int originalPos, Vector2Int targetDir)
        {
            var upDir = originalDir;
            var pos = originalPos;
            var size = Mathf.Abs(originalDir.x) == Mathf.Abs(targetDir.x)
                ? originalSize
                : new Vector2Int(originalSize.y, originalSize.x);
            while (upDir != targetDir)
            {
                upDir = upDir.Rotate90(true);
                pos = new Vector2Int(pos.y, size.x - pos.x - 1);
                // var deltaPos = -upDir * (size.x - 1);
                // pos += deltaPos;
                size = new Vector2Int(size.y, size.x);
            }
            return pos;
        }
    }
    public static ShapeCoordsTranslator FromLocalFieldRotation(this Vector2Int pos, Shape shape)
    {
        return new ShapeCoordsTranslator(ShapeCoordsTranslator.Type.LocalFieldRotation, shape, pos);
    }
    public static ShapeCoordsTranslator FromLocalShapeRotation(this Vector2Int pos, Shape shape)
    {
        return new ShapeCoordsTranslator(ShapeCoordsTranslator.Type.LocalShapeRotation, shape, pos);
    }
}