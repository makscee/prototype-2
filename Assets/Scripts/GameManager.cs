using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject clearProgressButton;

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
            Exit();
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

        if (Input.GetKeyDown(KeyCode.V) && Field == null)
        {
            foreach (var field in FieldPack.active.fields) 
                Progress.SetComplete(field.packId, field.fieldId);
            SceneManager.LoadScene(0);
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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Progress.ResetPackAndSave(FieldPack.active.packId);
            SceneManager.LoadScene(0);
        }

        if (Field != null && !Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Field.Size++;
                Field.CreateCells();
            } else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Field.Size--;
                Field.CreateCells();
            }
        }
#endif
    }

    public void ClearProgress()
    {
        Progress.ResetAndSave();
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        if (FieldMatrix.Active != null)
        {
            FieldMatrix.Active = null;
            SoundsPlayer.instance.PlayFieldClose();
        }
        else
        {
            Application.Quit();
        }
    }
}