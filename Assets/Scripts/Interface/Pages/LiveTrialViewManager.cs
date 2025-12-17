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


}
