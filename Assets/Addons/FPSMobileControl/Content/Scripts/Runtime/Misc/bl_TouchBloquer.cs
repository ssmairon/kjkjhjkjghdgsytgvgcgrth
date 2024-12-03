using UnityEngine;
using UnityEngine.EventSystems;

namespace MFPS.Mobile
{
    public class bl_TouchBloquer : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!bl_MobileUtility.ActiveIgnoreTouches.Contains(eventData.pointerId))
            {
                bl_MobileUtility.ActiveIgnoreTouches.Add(eventData.pointerId);
            }
            else
            {
                Debug.LogWarning("Touch " + eventData.pointerId + " is already in the list of active touches to ignore!");
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (bl_MobileUtility.ActiveIgnoreTouches.Contains(eventData.pointerId))
            {
                bl_MobileUtility.ActiveIgnoreTouches.Remove(eventData.pointerId);
            }
            else
            {
                Debug.LogWarning("Touch " + eventData.pointerId + " is not in the list of active touches to ignore!");
            }
        }
    }
}