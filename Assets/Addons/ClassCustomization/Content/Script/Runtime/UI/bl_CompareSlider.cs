using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_CompareSlider : MonoBehaviour
    {
        [SerializeField] private Image bottomImg = null;
        [SerializeField] private Image topImg = null;

        /// <summary>
        /// 
        /// </summary>
        public void Compare(float currentValue, float newValue, float maxValue, bool moreIsBetter)
        {
            float current = currentValue / maxValue;
            float newV = newValue / maxValue;

            SetupSlides(current, newV, moreIsBetter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentValue">0-1</param>
        /// <param name="newValue">0-1</param>
        public void SetupSlides(float currentValue, float newValue, bool moreIsBetter)
        {
            if (!moreIsBetter)
            {
                currentValue = 1 - currentValue;
                newValue = 1 - newValue;
            }

            bool isBetter = newValue > currentValue;

            var currentValueSlide = topImg;
            var diffValueSlide = bottomImg;

            if (!isBetter)
            {
                currentValueSlide = bottomImg;
                diffValueSlide = topImg;
            }

            diffValueSlide.color = isBetter ? new Color(0.3764706f, 0.5294118f, 0f, 1.00f) : Color.white;
            currentValueSlide.color = isBetter ? Color.white : new Color(0.5294118f, 0.2156863f, 0.1921569f, 1.00f);

            currentValueSlide.fillAmount = currentValue;
            diffValueSlide.fillAmount = newValue;
        }
    }
}