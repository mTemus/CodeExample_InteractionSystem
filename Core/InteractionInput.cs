using System;
using System.Collections.Generic;
using CodeExamples.Codebase.InteractionSystem.Core.Handlers;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Core
{
    [Serializable]
    public class InteractionInput
    {
        [SerializeField] 
        private ButtonInputAction m_input;

        //TODO: display/hide events

        private List<InputListenerData> m_inputListeners = new List<InputListenerData>();

        public ButtonInputAction Button => m_input;

        public void Subscribe(IInteractionInputListener listener, IInputInteraction interaction)
        {
            var inputs = new List<InputListenerData>
            {
                new InputListenerData(
                    listener,
                    () => listener.InteractionStart(interaction),
                    RWPlayerButtonInputListener.ButtonListenerState.ButtonDown,
                    interaction),
                new InputListenerData(
                    listener,
                    () => listener.InteractionEnd(interaction),
                    RWPlayerButtonInputListener.ButtonListenerState.ButtonUp,
                    interaction)
            };

            if (m_input.SubscribeDown)
                inputs.Add(new InputListenerData(
                    listener, 
                    () => listener.InteractionInputDown(interaction), 
                    RWPlayerButtonInputListener.ButtonListenerState.ButtonDown, 
                    interaction)
                );
        
            if (m_input.SubscribePressed)
                inputs.Add(new InputListenerData(
                    listener, 
                    () => listener.InteractionInputPressed(interaction), 
                    RWPlayerButtonInputListener.ButtonListenerState.ButtonPressed, 
                    interaction)
                );
        
            if (m_input.SubscribeUp)
                inputs.Add(new InputListenerData(
                    listener, 
                    () => listener.InteractionInputUp(interaction), 
                    RWPlayerButtonInputListener.ButtonListenerState.ButtonUp, 
                    interaction)
                );
        
            foreach (var input in inputs)
                m_input.Input.Value.Subscribe(input.Action, input.ButtonState);

            m_inputListeners.AddRange(inputs);
        }

        public void Unsubscribe(IInteractionInputListener listener, IInputInteraction interaction)
        {
            for (var i = m_inputListeners.Count - 1; i >= 0; i--)
            {
                if (m_inputListeners[i].Listener != listener)
                    continue;

                if (m_inputListeners[i].Interaction != interaction)
                    continue;

                m_input.Input.Value.Unsubscribe(m_inputListeners[i].Action, m_inputListeners[i].ButtonState);
                m_inputListeners.RemoveAt(i);
            }
        }

        private readonly struct InputListenerData
        {
            public IInteractionInputListener Listener { get; }
            public Action Action { get; }
            public RWPlayerButtonInputListener.ButtonListenerState ButtonState { get; }
            public IInteraction Interaction { get; }

            public InputListenerData(IInteractionInputListener listener, Action action, RWPlayerButtonInputListener.ButtonListenerState buttonState, IInteraction interaction)
            {
                Listener = listener;
                Action = action;
                ButtonState = buttonState;
                Interaction = interaction;
            }
        }
    }
}
