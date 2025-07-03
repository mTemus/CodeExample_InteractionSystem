using System;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Other.Raycast
{
    [Serializable]
    public class CameraComponentRaycaster<TTarget> : Raycaster<TTarget> where TTarget : EEMonoBehaviour
    {
        [SerializeField, ReadOnly]
        private readonly RayCasterData m_data;

        public CameraComponentRaycaster(RayCasterData data)
        {
            m_data = data;
            m_ray = new Ray(m_data.Camera.transform.position, m_data.Camera.transform.forward);
        }

        public override bool CastRay()
        {
            CreateRay();

            if (!UnityEngine.Physics.Raycast(m_ray, out m_hit, m_data.MaxDistance, m_data.LayerMask))
            {
                Clear();
                return false;
            }

            if (m_previousHit.transform != null && m_hit.transform.gameObject == m_previousHit.transform.gameObject)
                return true;

            m_target.Value = m_hit.transform.GetComponent<TTarget>();

            m_previousHit = m_hit;
            return m_target.Value != null;
        }

        public override bool CastRayForAll()
        {
            throw new NotImplementedException("Update code later if needed!");
        }

        protected override void CreateRay()
        {
            m_ray.origin = m_data.Camera.transform.position;
            m_ray.direction = m_data.Camera.transform.forward;
        }
    }
}