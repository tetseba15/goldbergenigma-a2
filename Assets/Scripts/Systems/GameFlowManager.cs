using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    public enum Act {Introduction ,Act1_GroundFloor, Act2_UpperFloor, Act3_Exterior, Epilogue }
    public Act CurrentAct { get; private set; } = Act.Introduction;

    // Events
    public static event Action<string, string, Func<bool>> OnChapterTitleRequested;

    public static event Action<Act> OnActChanged;

    [Header("Condiciones de Introducción (Acto 0)")]
    private bool _introHasFlashlight = false;
    private bool _introHasKey = false;
    private bool _introNoteRead = false;
    private bool _introHasLighter = false;

    [Header("Condiciones de Acto Final (Acto 3)")]
    private bool _finalNoteRead = false;
    private bool _personalDiaryRead = false;



    private bool _hasOfficeKey = false;
    private bool _hasCheckedCar = false;
    private bool _hasUsedOuija = false;
    private bool _hasBottle = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        OuijaBoard.OnOuijaUse += HandleItemCollected;
        PlayerInventory.OnItemCollected += HandleItemCollected;
        ObjectiveManager.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        OuijaBoard.OnOuijaUse -= HandleItemCollected;
        PlayerInventory.OnItemCollected -= HandleItemCollected;
        ObjectiveManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    private void HandleItemCollected(PlayerInventory.ItemType item)
    {
        // --- ÍTEMS DEL ACTO 0 (INTRO) ---
        if (item == PlayerInventory.ItemType.Flashlight) _introHasFlashlight = true;
        if (item == PlayerInventory.ItemType.Lighter) _introHasLighter = true;
        if (item == PlayerInventory.ItemType.MansionKey) _introHasKey = true; 

        // --- ÍTEMS DE ACTOS AVANZADOS ---
        if (item == PlayerInventory.ItemType.OfficeKey) _hasOfficeKey = true;
        if (item == PlayerInventory.ItemType.Bottle) _hasBottle = true;
        if (item == PlayerInventory.ItemType.OuijaBoard) _hasUsedOuija = true;

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
                if (_introHasFlashlight && _introHasKey && _introNoteRead && _introHasLighter)
                {
                    AdvanceToAct(Act.Act1_GroundFloor);
                }
                break;

            case Act.Act1_GroundFloor:
                if (_hasOfficeKey && _hasUsedOuija && _hasBottle)
                {
                    AdvanceToAct(Act.Act2_UpperFloor);
                }
                break;

            case Act.Act2_UpperFloor:
                if (ObjectiveManager.Instance.CompletedAllObjectives())
                {
                    AdvanceToAct(Act.Act3_Exterior);
                }
                // Act 3 condiciones
                // if (_hasReadFinalNote) AdvanceToAct(Act.Act3_Exterior); Si tiene la cruz | Leyó la nota | Leyó el diario personal
                break;
        }
    }

    private void AdvanceToAct(Act newAct)
    {
        CurrentAct = newAct;

        ObjectiveManager.Instance.ClearAllObjectives();

        // 2. CONTROL CENTRALIZADO DE MISIONES INICIALES POR ACTO
        // El director decide qué misión arranca en cada parte de la historia
        switch (newAct)
        {
            case Act.Act1_GroundFloor:
                // Cuando arranca el Acto 1, forzamos la misión de la mansión desde acá
                ObjectiveManager.Instance.UpdateObjective(
                    ObjectiveManager.ObjectiveId.MansionExploration,
                    "Explorar la mansión"
                );
                OnChapterTitleRequested?.Invoke("ACTO I", "La Mansión Goldberg", () => !UIManager.Instance.IsReadingNote);
                break;

            case Act.Act2_UpperFloor:
                ObjectiveManager.Instance.CompleteObjective(ObjectiveManager.ObjectiveId.MansionExploration);

                ObjectiveManager.Instance.UpdateObjective(
                    ObjectiveManager.ObjectiveId.UpstairsExploration,
                    "Explorar la planta alta"
                );
                OnChapterTitleRequested?.Invoke("ACTO II", "El horror asciende", null);
                break;

            case Act.Act3_Exterior:
                // Ejemplo para el futuro:
                // ObjectiveManager.Instance.UpdateObjective(ObjectiveManager.ObjectiveId.Workshop, "Buscar la llave del quincho");
                break;
        }

        // 3. Notificamos al mundo (bloqueadores de puertas, luces, IA, etc.)
        OnActChanged?.Invoke(CurrentAct);

        Debug.Log($"<color=cyan><b>GAME FLOW: Avanzando al {newAct} y asignando misión inicial.</b></color>");
    }

    public void NotifyIntroNoteRead()
    {
        _introNoteRead = true;
        EvaluateProgression();
    }

    public void NotifyFinalNoteRead()
    {
        _finalNoteRead = true;
        EvaluateProgression();
    }

    public void NotifyPersonalDiaryRead()
    {
        _personalDiaryRead = true;
        EvaluateProgression();
    }
}