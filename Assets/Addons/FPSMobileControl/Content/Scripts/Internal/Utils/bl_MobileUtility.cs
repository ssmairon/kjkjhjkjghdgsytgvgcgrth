using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MFPS.Mobile
{
    public static class bl_MobileUtility
    {
        public static List<int> ActiveIgnoreTouches { get; private set; }
        private static int m_Touch = -1;
        private static List<int> m_Touches;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            ActiveIgnoreTouches = new List<int>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetUsableTouch()
        {
            if (Input.touches.Length <= 0)
            {
                m_Touch = -1;
                return m_Touch;
            }
            List<int> list = GetValuesFromTouches(Input.touches).Except<int>(ActiveIgnoreTouches).ToList<int>();
            if (list.Count <= 0)
            {
                m_Touch = -1;
                return m_Touch;
            }
            if (!list.Contains(m_Touch))
            {
                m_Touch = list[0];
            }
            return m_Touch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="touches"></param>
        /// <returns></returns>
        public static List<int> GetValuesFromTouches(Touch[] touches)
        {
            if (m_Touches == null)
            {
                m_Touches = new List<int>();
            }
            else
            {
                m_Touches.Clear();
            }
            for (int i = 0; i < touches.Length; i++)
            {
                m_Touches.Add(touches[i].fingerId);
            }
            return m_Touches;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1f;
            float num = 114.5916f * Mathf.Atan(q.x);
            num = Mathf.Clamp(num, min, max);
            q.x = Mathf.Tan(0.008726646f * num);
            return q;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Canvas"></param>
        /// <param name="touchID"></param>
        /// <returns></returns>
        public static Vector3 TouchPosition(this Canvas _Canvas, int touchID)
        {
            Vector3 Return = Vector3.zero;

            if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
            Return = Input.GetTouch(touchID).position;
#else
                Return = Input.mousePosition;
#endif
            }
            else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 tempVector = Vector2.zero;
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
           Vector3 pos = Input.GetTouch(touchID).position;
#else
                Vector3 pos = Input.mousePosition;
#endif
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_Canvas.transform as RectTransform, pos, _Canvas.worldCamera, out tempVector);
                Return = _Canvas.transform.TransformPoint(tempVector);
            }

            return Return;
        }
    }
}