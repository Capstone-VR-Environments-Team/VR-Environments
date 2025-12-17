using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewPastSessionsManager : MonoBehaviour
{
    [Header("Session Data")]
    [SerializeField] private TMP_Text nameText;
    public TMP_Text NameText => nameText;

    [SerializeField] private TMP_Text participantIDText;
    public TMP_Text ParticipantIDText => participantIDText;

    [SerializeField] private TMP_Text notesText;
    public TMP_Text NotesText => notesText;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    public Button CancelButton => cancelButton;

    [SerializeField] private Button statisticalViewButton;
    public Button StatisticalViewButton => statisticalViewButton;

    [SerializeField] private Button interactiveViewButton;
    public Button InteractiveViewButton => interactiveViewButton;

    public void SetSessionInfo(SessionInfo sessionInfo)
    {
        nameText.SetText(sessionInfo.Name.ToString());
        participantIDText.SetText(sessionInfo.ParticipantID.ToString());
        notesText.SetText(sessionInfo.Notes.ToString());
    }
}
