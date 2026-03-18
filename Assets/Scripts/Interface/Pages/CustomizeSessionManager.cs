using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomizeSessionManager : MonoBehaviour
{
    [Header("Configuration Name")]
    [SerializeField] private TMP_InputField configurationNameInput;

    [Header("Visibility Settings")]
    [SerializeField] private Toggle showTargetsInput;
    [SerializeField] private TMP_InputField targetsVisibleTimeInput;
    [SerializeField] private Toggle showHandsInput;
    [SerializeField] private TMP_InputField handsVisibleTimeInput;

    [Header("Offset Settings")]
    [SerializeField] private TMP_Dropdown typeInput;
    [SerializeField] private TMP_InputField xInput;
    [SerializeField] private TMP_InputField yInput;
    [SerializeField] private TMP_InputField zInput;
    [SerializeField] private TMP_InputField targetRangeInput;

    [Header("Buttons")]
    [SerializeField] private Button saveConfigurationButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button uploadTargetLocationsButton;
    [SerializeField] private TMP_Text uploadedFileNameText;
    [SerializeField] private Button modifyConfigurationButton;

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
