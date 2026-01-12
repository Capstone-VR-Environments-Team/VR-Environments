using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveTrialViewManager : MonoBehaviour
{
    [Header("Control Panel")]
    [SerializeField] private Button beginTrialButton;
    public Button BeginTrialButton => beginTrialButton;

    [SerializeField] private TMP_Text timerText;
    public TMP_Text TimerText => timerText;

    [SerializeField] private Button endTrialButton;
    public Button EndTrialButton => endTrialButton;

    [Header("Data Panel")]
    [SerializeField] private TMP_Text experimentNameText;
    public TMP_Text ExperimentNameText => experimentNameText;

    [SerializeField] private TMP_Text participantIDText;
    public TMP_Text ParticipantIDText => participantIDText;

    [SerializeField] private GameObject notesView;
    public GameObject NotesView => notesView;

    [SerializeField] private TMP_InputField noteInput;
    public TMP_InputField NoteInput => noteInput;

    [SerializeField] private Button logButton;
    public Button LogButton => logButton;

    private double _currentNoteStartTime = -1.0;

    private void Start()
    {
        // Hook up listeners
        if (noteInput != null)
            noteInput.onValueChanged.AddListener(OnNoteValueChanged);
        
        if (logButton != null)
            logButton.onClick.AddListener(OnSaveNoteClicked);
    }

    private void OnNoteValueChanged(string text)
    {
        // If text was empty and now isn't, record start time
        if (text.Length > 0 && _currentNoteStartTime < 0)
        {
            _currentNoteStartTime = LoggingManager.Instance.GetTrialTime();
        }
        // If text becomes empty (user deleted everything), reset time
        else if (text.Length == 0)
        {
            _currentNoteStartTime = -1.0;
        }
    }

    private void OnSaveNoteClicked()
    {
        if (noteInput != null && !string.IsNullOrEmpty(noteInput.text) && _currentNoteStartTime >= 0)
        {
            // Log the note with the time it STARTED being typed
            LoggingManager.Instance.LogNote(noteInput.text, _currentNoteStartTime);
            
            // Clear input and reset time
            noteInput.text = "";
            _currentNoteStartTime = -1.0;
        }
    }
}
