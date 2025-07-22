using UnityEngine;

public class AutoClose : MonoBehaviour
{
    public float timeToClose = 10f; // 300 seconds = 5 minutes

    private float elapsedTime = 0f;

    void Update()
    {
        // Accumulate time elapsed
        elapsedTime += Time.deltaTime;

        // Check if the elapsed time has reached the specified limit
        if (elapsedTime >= timeToClose)
        {
            // Call the method to close the program or perform your desired action
            CloseProgram();
        }
    }

    // Method to close the program or perform your desired action
    private void CloseProgram()
    {
#if UNITY_EDITOR
        // In the editor, you can stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // In a standalone build, you can use Application.Quit() to close the application
        Application.Quit();
#endif
    }
}
