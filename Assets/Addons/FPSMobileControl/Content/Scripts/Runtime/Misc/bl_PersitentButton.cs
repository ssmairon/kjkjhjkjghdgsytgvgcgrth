using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFPS.Mobile
{
    public class bl_PersitentButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public bool Clamped = true;
        public float ClampArea = 100;

        private Vector2 defaultPosition, defaultRawPosition;
        private RectTransform m_Transform;
        private int touchID = 0;
        private Touch m_Touch;
        private bool init = false;
        private Vector2 sizeDelta;
        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            m_Transform = GetComponent<RectTransform>();
            init = true;
            FetchOrigin();
        }

        /// <summary>
        /// 
        /// </summary>
        public void FetchOrigin()
        {
            defaultPosition = m_Transform.anchoredPosition;
            defaultRawPosition = m_Transform.position;
            sizeDelta = m_Transform.sizeDelta * 0.5f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!init) return;
            touchID = eventData.pointerId;
#if !UNITY_EDITOR
          //  StartCoroutine(OnUpdate());
#endif
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!init) return;
#if !UNITY_EDITOR
           // StopAllCoroutines();
#endif
            m_Transform.anchoredPosition = defaultPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!init) return;

            var pos = eventData.position;
            pos.x += (sizeDelta.x * m_Transform.pivot.x);
            pos.y -= sizeDelta.y + (sizeDelta.y * m_Transform.pivot.y);

            if (Clamped)
            {
                Vector2 v = pos - defaultRawPosition;
                v = Vector2.ClampMagnitude(v, ClampArea);
                pos = defaultRawPosition + v;
            }

            m_Transform.position = pos;
        }

        IEnumerator OnUpdate()
        {
            while (true)
            {
                Follow();
                yield return null;
            }
        }

        void Follow()
        {
            m_Touch = Input.GetTouch(touchID);
            m_Transform.position = new Vector3(m_Touch.position.x + (m_Transform.sizeDelta.x * m_Transform.pivot.x), m_Touch.position.y + (m_Transform.sizeDelta.y * m_Transform.pivot.y), transform.position.z);
        }
    }
}