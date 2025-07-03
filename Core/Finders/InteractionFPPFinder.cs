using System;
using CodeExamples.Codebase.InteractionSystem.Other.Raycast;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Core.Finders
{
    [Serializable]
    public class InteractionFPPFinder : InteractionFinder
    {
        [TabGroup("Debug")]
        [SerializeField, ReadOnly]
        private readonly IRaycaster m_raycaster;

        public InteractionFPPFinder(RayCasterData raycasterData)
        {
            m_raycaster = new CameraComponentRaycaster<InteractableObject>(raycasterData);
        }

        public override void Initialize(object owner)
        {
            m_raycaster.TargetReference.AddChangedListener(OnRaycastTargetChanged);
        }

        public override void Uninitialize(object owner)
        {
            m_raycaster.TargetReference.RemoveChangedListener(OnRaycastTargetChanged);
        }

        private void OnRaycastTargetChanged(ValueReferenceBase newValue)
        {
            if (newValue.GetValueAsObject() == null)
                ResetInteractable();
            else
                SetInteractable(m_raycaster.GetRayCastResultAs<InteractableObject>(false));
        }

        public override void TryToFindInteraction()
        {
            m_raycaster.CastRay();
        }
    }
}