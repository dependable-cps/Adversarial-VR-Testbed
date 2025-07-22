using UnityEngine;
using UnityEngine.UI;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel;  // Reference to the panel GameObject

    private bool isPanelActive = true;  // Track the panel's state

    void Start()
    {
        // Ensure the panel is active at the start if initialized as active
        if (panel != null)
        {
            isPanelActive = panel.activeSelf;
        }
    }

    void Update()
    {
        // Toggle the panel on/off when Space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPanelActive = !isPanelActive;
            panel.SetActive(isPanelActive);
        }
    }
}
