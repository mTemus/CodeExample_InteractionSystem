using System;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Other.Raycast
{
    [Serializable]
    //TODO: add frames count to cast ray to optimize
    public abstract class Raycaster<TTarget> : IRaycaster where TTarget : class
    {
        protected Ray m_ray;

        [SerializeField, ReadOnly]
        protected RaycastHit m_hit;

        [SerializeField, ReadOnly]
        protected RaycastHit m_previousHit;

        [SerializeField, ReadOnly]
        protected ValueReference<TTarget> m_target = new ValueReference<TTarget>(true);

        [SerializeField, ReadOnly]
        protected TTarget[] m_targets;

        public ValueReferenceBase TargetReference => m_target;

        #region IRaycaster

        public abstract bool CastRay();
        public abstract bool CastRayForAll();
        public virtual bool HasTarget() => m_target.Value != null || m_targets != null;

        public virtual T GetRayCastResultAs<T>(bool clearResult = true) where T : class
        {
            if (typeof(T) != typeof(TTarget))
            {
                Debug.LogError($"{GetType().Name} --- T of type {typeof(T).Name} is not the same as TTarget of type {typeof(TTarget).Name}. Can't cast!");
                return null;
            }

            T result = null;

            if (m_target.Value != null)
            {
                result = m_target.Value as T;

                if (clearResult)
                    Clear();
            }

            if (m_targets != null)
            {
                if (typeof(T).IsArray)
                {
                    result = m_targets as T;
                    
                    if (clearResult)
                        Clear();
                }
                else
                {
                    Debug.LogError($"{GetType().Name} --- T of type {typeof(T)} is a single Type, not an array. Can't cast!");
                }
            }
            
            return result;
        }

        public virtual void Clear()
        {
            if (m_target.Value == null)
                return;
            
            m_hit = default;
            m_previousHit = default;
            m_target.Value = null;
            m_targets = null;
        }

        public virtual Type GetTargetType() => typeof(TTarget);

        #endregion

        protected abstract void CreateRay();
    }

    [Serializable]
    public class RayCasterData
    {
        public UnityEngine.Camera Camera { get; }
        public LayerMask LayerMask { get; }
        public float MaxDistance { get; }

        public RayCasterData(UnityEngine.Camera camera, LayerMask layerMask, float maxDistance)
        {
            Camera = camera;
            LayerMask = layerMask;
            MaxDistance = maxDistance;
        }
    }
}
