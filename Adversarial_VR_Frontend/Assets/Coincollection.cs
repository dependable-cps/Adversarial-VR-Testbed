using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coincollection : MonoBehaviour
{
    private int coin = 0;
    public GameObject[] thecoins;
    public TextMeshProUGUI scoreText;

    void Update()
    {
        foreach (GameObject thecoin in thecoins)
        {
            if (thecoin != null)
            {
                thecoin.transform.Rotate(0f, 1f, 0f, Space.Self);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Coin")
        {
            Destroy(other.gameObject);
            coin++;
            scoreText.text = "Coin collection score: " + coin.ToString();
        }
    }
}