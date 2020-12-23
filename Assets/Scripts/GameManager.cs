using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        Animator.Update();
        
        DebugInput();
    }

    FieldMatrix Matrix => FieldMatrix.Active;
    void DebugInput()
    {
        if (ScreenBox.activeBox != null) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Matrix.MoveAttachedShape(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Matrix.MoveAttachedShape(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Matrix.InsertShape();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Matrix.Undo();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            var container = Matrix.shapesContainer; 
            container
                .InsertAtCurrent(ShapeSerialized.CreateFromString(new[] {"*"})
                .Deserialize());
            if (Matrix.attachedShape == null)
                Matrix.AttachShape(container.GetNext());
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            var field = FieldMatrix.Create();
            var container = new ShapeContainer(field) {matrixSize = 5};
            field.SetContainer(container);
            FieldMatrix.Active = field;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FieldMatrix.Active != null)
            {
                FieldMatrix.Active.SetState(FieldState.OnSelectScreen);
                FieldMatrix.Active = null;
            }
        }
    }
}