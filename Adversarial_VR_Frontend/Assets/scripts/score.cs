using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class score : MonoBehaviour
{
    public static long Score = 0 ;
    public HealthBar healthBar;
    public static string proba= "Probabilities:\nNone: 1.0\n" +
                $"Low: 0.0\nMedium: 0.0\nHigh: 0.0";
   
   int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        healthBar.SetMaxHealth(3);
    }

    // Update is called once per frame
    void Update()
    {
       // healthBar.SetHealth(Convert.ToInt32(Score));
        healthBar.fill.color = healthBar.gradient.Evaluate(Score / 3f);
        if (Score == 0)
        {
            healthBar.SetReaction(0);
        }
        else if (Score == 1)
        {
            healthBar.SetReaction(1);
        }
        else if (Score == 2)
        {
            healthBar.SetReaction(2);
        }
        else if (Score == 3)
        {
            healthBar.SetReaction(3);
        }

        

        // for i =0 to 100 set healthbar color to i/3f and reaction to 0, for i = 100 to 200 set healthbar color to i/3f and reaction to 1, for i = 200 to 300 set healthbar color to i/3f and reaction to 2, for i = 300 to 400 set healthbar color to i/3f and reaction to 3
        /*if (i < 1000)
        {
            healthBar.fill.color = healthBar.gradient.Evaluate(0 / 3f);
            healthBar.SetReaction(0);
        }
        else if (i < 2000 )
        {
            healthBar.fill.color = healthBar.gradient.Evaluate(1 / 3f);
            healthBar.SetReaction(1);
        }
        else if (i < 3000)
        {
            healthBar.fill.color = healthBar.gradient.Evaluate(2 / 3f);
            healthBar.SetReaction(2);
        }
        else if (i < 4000)
        {
            healthBar.fill.color = healthBar.gradient.Evaluate(3 / 3f);
            healthBar.SetReaction(3);
        }
        else if(i == 5000){
            i = 0;
        }
        i++;*/


    }
    void OnGUI (){

//GUI.Box (new Rect (200, 200, 200, 200), "Cyber sickness Level \n" + Score.ToString ());
//GUI.skin.box.normal.textColor = Color.red;

}
}
