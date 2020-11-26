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

    void DebugInput()
    {
        if (ScreenBox.activeBox != null) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.MoveAttachedShape(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.MoveAttachedShape(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.InsertShape();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            FieldMatrix.current.shapesContainer.SaveToFile("testLevel");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            var container = ShapeContainerSerialized
                .LoadFromJson(FileStorage.ReadJsonFile("testLevel"))
                .Deserialize(FieldMatrix.current);
            FieldMatrix.current.AddContainer(container);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            var container = FieldMatrix.current.shapesContainer; 
            container
                .InsertAtCurrent(ShapeSerialized.CreateFromString(new[] {"*"})
                .Deserialize());
            if (FieldMatrix.current.attachedShape == null)
                FieldMatrix.current.AttachShape(container.GetNext());
        }
    }
}