using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LiveTrialViewManager : MonoBehaviour
{
    [Header("Control Panel")]
    [SerializeField] private Button beginTrialButton;
    public Button BeginTrialButton => beginTrialButton;

    [SerializeField] private TMP_Text timerText;
    public TMP_Text TimerText => timerText;

    [SerializeField] private Button endTrialButton;
    public Button EndTrialButton => endTrialButton;

    [SerializeField] private Button goHomeButton;
    public Button GoHomeButton => goHomeButton;

    [Header("Data Panel")]
    [SerializeField] private TMP_Text experimentNameText;
    public TMP_Text ExperimentNameText => experimentNameText;

    [SerializeField] private TMP_Text participantIDText;
    public TMP_Text ParticipantIDText => participantIDText;

    [Header("Notes Log Settings")]
    [SerializeField] private GameObject notesView; 
    public GameObject NotesView => notesView;

    [SerializeField] private GameObject noteEntryPrefab;
    public GameObject NoteEntryPrefab => noteEntryPrefab;

    [SerializeField] private TMP_InputField noteInput;
    public TMP_InputField NoteInput => noteInput;

    [SerializeField] private Button logButton;
    public Button LogButton => logButton;

    [SerializeField] private XRManager xrManager;

    private double _currentNoteStartTime = -1.0;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    private void Start()
    {
        if (noteInput != null)
            noteInput.onValueChanged.AddListener(OnNoteValueChanged);

        if (logButton != null)
            logButton.onClick.AddListener(OnSaveNoteClicked);
        ResetLiveTrialViewManager();
        
    }

    private void OnEnable()
    {
        EventBus.StartExperiment += StartTimer;
        EventBus.StopExperiment += StopTimer;

        ResetLiveTrialViewManager();

        if (notesView != null)
        {
            Transform parentTransform = notesView.transform;
            ScrollRect scrollRect = notesView.GetComponent<ScrollRect>();
            if (scrollRect != null && scrollRect.content != null)
            {
                parentTransform = scrollRect.content;
            }

            foreach (Transform child in parentTransform)
            {
                Destroy(child.gameObject);
            }
        }

        if (noteInput != null)
        {
            noteInput.text = ""; 
        }

        _currentNoteStartTime = -1.0; 

        if (timerText != null && !isRunning)
        {
            timerText.text = "00:00";
        }
    }

    public void OnDisable() {
        EventBus.StartExperiment -= StartTimer;
        EventBus.StopExperiment -= StopTimer;
    }

    public void OnGoHomeClicked() {
        xrManager.TurnVROff();
        Destroy(SessionManager.Instance);
        SceneManager.LoadScene("HomeScreen");
    }

    private void OnNoteValueChanged(string text)
    {
        if (text.Length > 0 && _currentNoteStartTime < 0)
        {
            _currentNoteStartTime = SessionManager.Instance.GetTrialTime();
        }
        else if (text.Length == 0)
        {
            _currentNoteStartTime = -1.0;
        }
    }

    private void OnSaveNoteClicked()
    {
        if (noteInput != null && !string.IsNullOrEmpty(noteInput.text) && _currentNoteStartTime >= 0)
        {
            EventBus.OnNoteEnter?.Invoke(noteInput.text, _currentNoteStartTime);
        
            TimeSpan t = TimeSpan.FromSeconds(_currentNoteStartTime / 1000);
            string timeString = string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
            AppendToLog($"{timeString} - {noteInput.text}");
        

            noteInput.text = "";
            _currentNoteStartTime = -1.0;
        }
    }

    public void AppendToLog(string text)
    {
        if (notesView == null || noteEntryPrefab == null) return;

        Transform parentTransform = notesView.transform;
        ScrollRect scrollRect = notesView.GetComponent<ScrollRect>();
        if (scrollRect != null && scrollRect.content != null)
        {
            parentTransform = scrollRect.content;
        }

        GameObject newEntry = Instantiate(noteEntryPrefab, parentTransform);

        TMP_Text entryText = newEntry.GetComponent<TMP_Text>();
        if (entryText != null)
        {
            entryText.text = text;
        }

        newEntry.SetActive(true);

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer(Vector3 headsetPosition)
    {
        isRunning = true;

        if (notesView != null)
        {
            Transform parentTransform = notesView.transform;
            ScrollRect scrollRect = notesView.GetComponent<ScrollRect>();
            if (scrollRect != null && scrollRect.content != null)
            {
                parentTransform = scrollRect.content;
            }

            foreach (Transform child in parentTransform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        elapsedTime = 0f;
        UpdateTimerDisplay();

        double time = SessionManager.Instance.GetTrialTime();
        TimeSpan t = TimeSpan.FromSeconds(time / 1000);
        string timeString = string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
        AppendToLog($"{timeString} - Data has been saved to folder");

        endTrialButton.gameObject.SetActive(false);
        goHomeButton.gameObject.SetActive(true);
    }

    public void disableButtonsOnEnd()
    {
        beginTrialButton.interactable = false;
        logButton.interactable = false;
    }

    public void ResetLiveTrialViewManager()
    {
        endTrialButton.gameObject.SetActive(true);
        goHomeButton.gameObject.SetActive(false);
        beginTrialButton.interactable = true;
        logButton.interactable = true;

        TrialSessionInformation trialInfo = SessionManager.Instance.GetTrialSessionInformation();
        if (experimentNameText != null && trialInfo != null)
            experimentNameText.SetText(trialInfo.SessionName);
        if (participantIDText != null && trialInfo != null)
            participantIDText.SetText(trialInfo.ParticipantID);
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}