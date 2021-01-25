using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        Animator.Update();

        var config = GlobalConfig.Instance;
        config.thickness = config.thicknessBase + Mathf.Sin(Time.time * config.sinTimeScale) * config.sinScale;
        DebugInput();
    }

    FieldMatrix Matrix => FieldMatrix.Active;
    void DebugInput()
    {
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FieldMatrix.Active != null)
            {
                FieldMatrix.Active.SetState(FieldScreenState.OnSelectScreen);
                FieldMatrix.Active = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Progress.ResetAndSave();
            SceneManager.LoadScene(0);
        }
    }
}