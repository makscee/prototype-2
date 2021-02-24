using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    public GameObject shapeCellsParticlesContainer;
    void Update()
    {
        Animator.Update();

        // var config = GlobalConfig.Instance;
        // config.thickness = config.thicknessBase + Mathf.Sin(Time.time * config.sinTimeScale) * config.sinScale;
        DebugInput();
    }

    static FieldMatrix Field => FieldMatrix.Active;
    void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Field.MoveAttachedShape(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Field.MoveAttachedShape(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Field.InsertShape();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Field.Undo();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FieldMatrix.Active != null)
            {
                FieldMatrix.Active = null;
            }
            else
            {
                Application.Quit();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        
#if UNITY_EDITOR
        
        if (Input.GetKeyDown(KeyCode.C) && Field != null)
        {
            Progress.SetComplete(Field.packId, Field.fieldId);
            Field.CompleteTransition();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            var container = Field.shapesContainer; 
            container
                .InsertAtCurrent(ShapeSerialized.CreateFromString(new[] {"*"})
                .Deserialize());
            if (Field.attachedShape == null)
                Field.AttachShape(container.GetNext());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ClearProgress();
        }
#endif
    }

    public void ClearProgress()
    {
        Progress.ResetAndSave();
        SceneManager.LoadScene(0);
    }
}