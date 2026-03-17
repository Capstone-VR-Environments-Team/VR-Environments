using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomizeSessionManager : MonoBehaviour
{
    [Header("Configuration Name")]

    [SerializeField] private TMP_InputField configurationNameInput;
    public TMP_InputField ConfigurationNameInput => configurationNameInput;

    [Header("Visibility Settings")]
    [SerializeField] private Toggle showTargetsInput;
    public Toggle ShowTargetsInput => showTargetsInput;

    [SerializeField] private TMP_InputField targetsVisibleTimeInput;
    public TMP_InputField TargetsVisibleTimeInput => targetsVisibleTimeInput;

    [SerializeField] private Toggle showHandsInput;
    public Toggle ShowHandsInput => showHandsInput;

    [SerializeField] private TMP_InputField handsVisibleTimeInput;
    public TMP_InputField HandsVisibleTimeInput => handsVisibleTimeInput;

    [Header("Offset Settings")]
    [SerializeField] private TMP_Dropdown typeInput;
    public TMP_Dropdown TypeInput => typeInput;

    [SerializeField] private TMP_InputField xInput;
    public TMP_InputField XInput => xInput;

    [SerializeField] private TMP_InputField yInput;
    public TMP_InputField YInput => yInput;

    [SerializeField] private TMP_InputField zInput;
    public TMP_InputField ZInput => zInput;

    [SerializeField] private TMP_InputField targetRangeInput;
    public TMP_InputField TargetRangeInput => targetRangeInput;

    [Header("Buttons")]
    [SerializeField] private Button saveConfigurationButton;
    public Button SaveConfigurationButton => saveConfigurationButton;

    [SerializeField] private Button cancelButton;
    public Button CancelButton => cancelButton;

    [SerializeField] private Button uploadTargetLocationsButton;
    public Button UploadTargetLocationsButton => uploadTargetLocationsButton;

    [SerializeField] private TMP_Text uploadedFileNameText;
    public TMP_Text UploadedFileNameText => uploadedFileNameText;

    [SerializeField] private Button modifyConfigurationButton;
    public Button ModifyConfigurationButton => modifyConfigurationButton;

    public void ResetInputs()
    {
        configurationNameInput.text = "";
        showTargetsInput.isOn = true;
        targetsVisibleTimeInput.text = "0";
        showHandsInput.isOn = true;
        handsVisibleTimeInput.text = "0";
        typeInput.value = 0;
        xInput.text = "0";
        yInput.text = "0";
        zInput.text = "0";
        targetRangeInput.text = "0";
        uploadedFileNameText.text = "No file uploaded";
    }

    public void OnGoHomeClicked() {
        ResetInputs();
        SceneManager.LoadScene("HomeScreen");
    }

}
