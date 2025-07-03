using System;
using System.Collections.Generic;
using CodeExamples.Codebase.InteractionSystem.Interactions;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Core.Handlers
{
    [Serializable]
    public class InteractionHandlerWithInput : InteractionHandler<IInputInteraction>, IInteractionInputListener
    {
        private readonly List<IInputInteraction> m_availableInteractions;
        private readonly List<IInputInteraction> m_unavailableInteractions;

        private ScriptableEventInputDisplayData m_showInputEvent;
        private ScriptableEventString m_removeInputEvent;

#if UNITY_EDITOR
        [SerializeField, ReadOnly]
        private readonly List<MonoInputInteraction> d_availableInteractions = new();

        [SerializeField, ReadOnly]
        private readonly List<MonoInputInteraction> d_unavailableInteractions = new();
#endif

        public InteractionHandlerWithInput(IInteractor interactor, ScriptableEventInputDisplayData showInputEvent, ScriptableEventString removeInputEvent) : base(interactor)
        {
            m_availableInteractions = new List<IInputInteraction>();
            m_unavailableInteractions = new List<IInputInteraction>();
            m_showInputEvent = showInputEvent;
            m_removeInputEvent = removeInputEvent;
        }

        public override void HandleAvailable(IInputInteraction interaction)
        {
            HandleInteraction(interaction);

#if UNITY_EDITOR
            d_availableInteractions.Add(interaction as MonoInputInteraction);
            d_unavailableInteractions.Remove(interaction as MonoInputInteraction);
#endif
            m_availableInteractions.Add(interaction);
            SubscribeInput(interaction);
        }

        public override void HandleUnavailable(IInputInteraction interaction)
        {
            HandleInteraction(interaction);
            m_unavailableInteractions.Add(interaction);

#if UNITY_EDITOR
            d_availableInteractions.Remove(interaction as MonoInputInteraction);
            d_unavailableInteractions.Add(interaction as MonoInputInteraction);
#endif
        }

        public override void HandleRemove(IInputInteraction interaction)
        {
            if (m_availableInteractions.Contains(interaction))
            {
                m_availableInteractions.Remove(interaction);
                UnsubscribeInput(interaction);

#if UNITY_EDITOR
                d_availableInteractions.Remove(interaction as MonoInputInteraction);
#endif
            }
            else
            {
                m_unavailableInteractions.Remove(interaction);

#if UNITY_EDITOR
                d_unavailableInteractions.Remove(interaction as MonoInputInteraction);
#endif
            }

            interaction.IsAvailable.RemoveChangedListener(OnInteractableStatusChanged);
        }

        private void HandleInteraction(IInputInteraction interaction)
        {
            interaction.IsAvailable.AddChangedListener(OnInteractableStatusChanged);
        }

        private void OnInteractableStatusChanged(ValueReferenceBase value)
        {
            var inputInteraction = value.Owner as IInputInteraction;

            if (inputInteraction.IsAvailable.Value)
            {
                if (m_availableInteractions.Contains(inputInteraction))
                    return;
                
                m_unavailableInteractions.Remove(inputInteraction);

                m_availableInteractions.Add(inputInteraction);
                SubscribeInput(inputInteraction);
            }
            else
            {
                if (m_unavailableInteractions.Contains(inputInteraction))
                    return;

                m_availableInteractions.Remove(inputInteraction);

                m_unavailableInteractions.Add(inputInteraction);
                UnsubscribeInput(inputInteraction);
            }
        }

        private void SubscribeInput(IInputInteraction inputInteraction)
        {
            inputInteraction.Input.Subscribe(this, inputInteraction);

            m_showInputEvent.Raise(
                new PlayerHUDInputDisplay.InputDisplayData
                (
                    inputInteraction.Input.Button.GUID, 
                    inputInteraction.Input.Button.DescriptionLocalizationKey
                )
            );
        }

        private void UnsubscribeInput(IInputInteraction inputInteraction)
        {
            inputInteraction.Input.Unsubscribe(this, inputInteraction);
            m_removeInputEvent.Raise(inputInteraction.Input.Button.GUID);
        }

        #region IInteractionInputListener

        public void InteractionStart(IInteraction interaction)
        {
            interaction.InteractionStart(Interactor);
        }

        public void InteractionEnd(IInteraction interaction)
        {
            interaction.InteractionEnd(Interactor);
        }

        public void InteractionInputDown(IInputInteraction interaction)
        {
            interaction.InteractionInputDown(Interactor);
        }

        public void InteractionInputPressed(IInputInteraction interaction)
        {
            interaction.InteractionInputPressed(Interactor);
        }

        public void InteractionInputUp(IInputInteraction interaction)
        {
            interaction.InteractionInputUp(Interactor);
        }

        #endregion
    }
}
