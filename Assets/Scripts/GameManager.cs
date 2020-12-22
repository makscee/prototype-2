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

    FieldMatrix Matrix => FieldMatrix.current;
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            Matrix.shapesContainer.SaveToFile("testLevel");
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
    }
}