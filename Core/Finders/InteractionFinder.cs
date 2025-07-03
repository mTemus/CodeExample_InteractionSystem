using System;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Core.Finders
{
    [Serializable]
    public abstract class InteractionFinder
    {
        [SerializeField, ReadOnly]
        protected ValueReference<InteractableObject> m_currentInteractable;

        public ValueReference<InteractableObject> CurrentInteractable => m_currentInteractable;

        protected InteractionFinder()
        {
            m_currentInteractable = new ValueReference<InteractableObject>(true, this);
        }

        public abstract void Initialize(object owner);
        public abstract void Uninitialize(object owner);

        public abstract void TryToFindInteraction();

        protected virtual void SetInteractable(InteractableObject interactable)
        {
            if (m_currentInteractable.Value != null)
                ResetInteractable();

            m_currentInteractable.Value = interactable;
        }

        protected virtual void ResetInteractable()
        {
            m_currentInteractable.Value = null;
        }
    }
}