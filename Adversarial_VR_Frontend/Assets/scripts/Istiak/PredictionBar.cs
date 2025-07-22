//  -----------------------------------------------------------------------
//  <copyright file="PredictionBar.cs" University="UMC">
//   Copyright (c) 2024 UMC All rights reserved.
//  </copyright>
//  <author>Istiak Ahmed</author>
//  -----------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
namespace Istiak
{
    public class PredictionBar : MonoBehaviour
    {
        public Slider slider;
        public Gradient gradient;
        public Image fill;
        public Image reaction;
        public Sprite[] reactionSprites;

        private void Start()
        {
            // SetMaxSliderValue(0);
            // UpdatePredictionBar(0);
        }

        private void SetMaxSliderValue(int sliderValue)
        {
            slider.maxValue = sliderValue;
            slider.value = sliderValue;
            fill.color = gradient.Evaluate(1f);
        }

        private void SetSliderValue(int sliderValue)
        {
            slider.value = sliderValue;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        private void SetReaction(int reactionLevel)
        {
            reaction.sprite = reactionSprites[reactionLevel];
        }

        public void UpdatePredictionBar(int input)
        {
            Debug.Log(input);
            // Assuming input is 0, 1, or 2
            // if (input == 0)
            // {
            //     Debug.LogError("Invalid input. Input must be 1, 2, or 3.");
            //     return;
            // }

            // Set the sliderValue value
            SetSliderValue(input); // Adjusting for sliderValue values being 1, 2, 3

            // Set the reaction sprite
            SetReaction(input);

            // Set the fill color based on the input
            switch (input)
            {
                case 1:
                    fill.color = gradient.Evaluate(0f); // Green at 33%
                    break;
                case 2:
                    fill.color = gradient.Evaluate(0.5f); // Yellow at 66%
                    break;
                case 3:
                    fill.color = gradient.Evaluate(1f); // Red at 100%
                    break;
            }
        }
    }
}
