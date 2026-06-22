using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    public enum Act {Introduction ,Act1_GroundFloor, Act2_UpperFloor, Act3_Exterior, Epilogue }
    public Act CurrentAct { get; private set; } = Act.Introduction;

    public static event Action<Act> OnActChanged;

    private bool _hasOfficeKey = false;
    private bool _hasCheckedCar = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        PlayerInventory.OnItemCollected += HandleItemCollected;
        ObjectiveManager.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        PlayerInventory.OnItemCollected -= HandleItemCollected;
        ObjectiveManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    private void HandleItemCollected(PlayerInventory.ItemType item)
    {
        if (item == PlayerInventory.ItemType.OfficeKey) _hasOfficeKey = true;

        EvaluateProgression();
    }

    private void HandleObjectiveCompleted(ObjectiveManager.ObjectiveId objective)
    {

        if (objective == ObjectiveManager.ObjectiveId.MerrinCar) _hasCheckedCar = true;


        EvaluateProgression();
    }

    // Game Flow Logic
    private void EvaluateProgression()
    {
        switch (CurrentAct)
        {
            case Act.Introduction:
                if (_hasCheckedCar)
                {
                    AdvanceToAct(Act.Act1_GroundFloor);
                }
                break;

            case Act.Act1_GroundFloor:
                if (_hasOfficeKey)
                {
                    AdvanceToAct(Act.Act2_UpperFloor);
                }
                break;

            case Act.Act2_UpperFloor:
                // Aquí pondrás tus condiciones para pasar al acto 3 en el futuro...
                // if (_hasReadFinalNote) AdvanceToAct(Act.Act3_Exterior);
                break;
        }
    }

    private void AdvanceToAct(Act newAct)
    {
        CurrentAct = newAct;

        // 1. Limpiamos la libreta para empezar un capítulo nuevo
        ObjectiveManager.Instance.ClearAllObjectives();

        // 2. Le damos su primer objetivo del nuevo Acto y un tutorial narrativo
        if (newAct == Act.Act2_UpperFloor)
        {
            ObjectiveManager.Instance.UpdateObjective(ObjectiveManager.ObjectiveId.UpstairsExploration, "Explorar la planta alta");
            //TutorialManager.RequestTimedTutorial?.Invoke("ACTO II\nEl horror asciende", 5f);
        }

        OnActChanged?.Invoke(CurrentAct);

        Debug.Log($"<color=cyan><b>GAME FLOW: Avanzando al {newAct}</b></color>");
    }
}