using CodeExamples.Codebase.InteractionSystem.Core;
using CodeExamples.Codebase.InteractionSystem.Player;
using UnityEngine;
using UnityEngine.Events;

namespace CodeExamples.Codebase.InteractionSystem.Interactions
{
    public class GenericMonoInteraction : MonoInputInteraction
    {
        [TabGroup("Properties")]
        [SerializeField] 
        private bool m_isSingleInteraction;

        [TabGroup("Generic Events")]
        [SerializeField]
        private UnityEvent m_onInputDown;

        [TabGroup("Generic Events")]
        [SerializeField]
        private UnityEvent m_onInputPressed;

        [TabGroup("Generic Events")]
        [SerializeField]
        private UnityEvent m_onInputUp;

        [TabGroup("Debug")]
        [SerializeField, ReadOnly]
        private bool m_interacted;

        protected override bool ShouldIgnoreInteractor(IInteractor interactor)
        {
            return interactor is not PlayerInteractionAbility;
        }

        public override void InteractionInputDown(IInteractor interactor)
        {
            m_onInputDown.Invoke();
        }

        public override void InteractionInputPressed(IInteractor interactor)
        {
            m_onInputPressed.Invoke();
        }

        public override void InteractionInputUp(IInteractor interactor)
        {
            m_onInputUp.Invoke();
        }

        protected override bool CheckAvailability()
        {
            return base.CheckAvailability() && !(m_isSingleInteraction && m_interacted);
        }

        public override void InteractionEnd(IInteractor interactor)
        {
            base.InteractionEnd(interactor);

            if (m_isSingleInteraction)
            {
                m_interacted = true;
                m_isAvailable.Value = false;
            }
        }
    }
}
