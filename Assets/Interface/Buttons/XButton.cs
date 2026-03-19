using UnityEngine;
using UnityEngine.UI;

public class XButton : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Application.Quit);
    }
}
