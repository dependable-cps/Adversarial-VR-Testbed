using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageToggleController : MonoBehaviour
{
    public Sprite onSprite; // Reference to the "On" sprite
    public Sprite offSprite; // Reference to the "Off" sprite

    // Master Image references
    public Image masterImage1;
    public Image masterImage2;

    // Child Image arrays under each master image
    public Image[] childImages1;
    public Image[] childImages2;

    // Panel arrays under each master image
    public GameObject[] panels1; // 4 panels related to 1st master image
    public GameObject[] panels2; // 4 panels related to 2nd master image

    private bool isMasterImage1On = false; // To track state of the first master image
    private bool isMasterImage2On = false; // To track state of the second master image

    private bool[] isChildImage1On = new bool[6]; // To track state of the child images under masterImage1
    private bool[] isChildImage2On = new bool[3]; // To track state of the child images under masterImage2
    public static Action<bool> ActiveTimer;

    public GameObject panel; // Reference to the panel GameObject
    public GameObject welcomePanel; // Reference to the panel GameObject

    private bool isPanelActive = false; // Track the panel's state
    public GameObject LaserPointer;

    void Start()
    {
        panel.SetActive(isPanelActive);
        LaserPointer.SetActive(isPanelActive);
        welcomePanel.SetActive(isPanelActive);
        masterImage1.sprite = isMasterImage1On ? onSprite : offSprite;
        masterImage2.sprite = isMasterImage2On ? onSprite : offSprite;
        // Initialize child images to "off"
        for (int i = 0; i < isChildImage1On.Length; i++)
        {
            isChildImage1On[i] = false;
            
        }

        for (int i = 0; i < isChildImage2On.Length; i++)
        {
            isChildImage2On[i] = false;
           
        }

        isMasterImage1On = true;
        isMasterImage2On = true;
        ToggleMasterImage(masterImage1, ref isMasterImage1On, childImages1, isChildImage1On, panels1);
        ToggleMasterImage(masterImage2, ref isMasterImage2On, childImages2, isChildImage2On, panels2);
    }

    private void HideWelcomePanel()
    {
        welcomePanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            welcomePanel.SetActive(true);
            Invoke(nameof(HideWelcomePanel),3f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPanelActive = !isPanelActive;
            panel.SetActive(isPanelActive);
            LaserPointer.SetActive(isPanelActive);
            ActiveTimer.Invoke(!isPanelActive && isMasterImage2On && isChildImage2On[0]);
        }

        if (!isPanelActive) return;
        // Toggle first master image and its child images with Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleMasterImage(masterImage1, ref isMasterImage1On, childImages1, isChildImage1On, panels1);
        }

        // Toggle second master image and its child images with X
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleMasterImage(masterImage2, ref isMasterImage2On, childImages2, isChildImage2On, panels2);
        }

        // Toggle individual child images under masterImage1
        if (Input.GetKeyDown(KeyCode.Q)) ToggleChildImage(0, childImages1, isChildImage1On, panels1);
        if (Input.GetKeyDown(KeyCode.W)) ToggleChildImage(1, childImages1, isChildImage1On, panels1);
        if (Input.GetKeyDown(KeyCode.E)) ToggleChildImage(2, childImages1, isChildImage1On, panels1);
        if (Input.GetKeyDown(KeyCode.R)) ToggleChildImage(3, childImages1, isChildImage1On, panels1);
        if (Input.GetKeyDown(KeyCode.T)) ToggleChildImage(4, childImages1, isChildImage1On, panels1);
        if (Input.GetKeyDown(KeyCode.Y)) ToggleChildImage(5, childImages1, isChildImage1On, panels1);

        // Toggle individual child images under masterImage2
        if (Input.GetKeyDown(KeyCode.A)) ToggleChildImage(0, childImages2, isChildImage2On, panels2);
        if (Input.GetKeyDown(KeyCode.S)) ToggleChildImage(1, childImages2, isChildImage2On, panels2);
        if (Input.GetKeyDown(KeyCode.D)) ToggleChildImage(2, childImages2, isChildImage2On, panels2);
    }

    // Function to toggle a master image, its child images, and panels
    private void ToggleMasterImage(Image masterImage, ref bool isMasterOn, Image[] childImages, bool[] isChildOnArray,
        GameObject[] panels)
    {
        isMasterOn = !isMasterOn;
        masterImage.sprite = isMasterOn ? onSprite : offSprite;

        for (int i = 0; i < childImages.Length; i++)
        {
            isChildOnArray[i] = isMasterOn; // Sync child images with master state
            childImages[i].sprite = isMasterOn ? onSprite : offSprite;
            panels[i].SetActive(isMasterOn); // Sync panel state with master state
        }
    }

    // Function to toggle individual child images and corresponding panels
    private void ToggleChildImage(int index, Image[] childImages, bool[] isChildOnArray, GameObject[] panels)
    {
        isChildOnArray[index] = !isChildOnArray[index];
        childImages[index].sprite = isChildOnArray[index] ? onSprite : offSprite;
        panels[index].SetActive(isChildOnArray[index]); // Toggle panel based on the child image state
    }
    
}