using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CodeExamples.Codebase.InteractionSystem.Core
{
    [RequireComponent(typeof(InteractableSelectionController))]
    public class InteractableObject : MonoBehaviour
    {
        [TabGroup("Interactions")]
        [SerializeField, Required] 
        private List<InterfaceReference<IInteraction>> m_interactions = new();

        [TabGroup("Events")]
        [SerializeField] 
        private UnityEvent m_onSelect;

        [TabGroup("Events")]
        [SerializeField] 
        private UnityEvent m_onDeselect;

        private List<IInteractor> m_selectors = new();

        private void OnValidate()
        {
            var interactions = GetComponentsInChildren<EEMonoBehaviour>();
            
            foreach (var interaction in interactions.OfType<IInteraction>())
            {
                if (m_interactions.Any(i => i.Value == interaction))
                    continue;

                m_interactions.Add(new InterfaceReference<IInteraction> { Value = interaction });
            }

            for (var i = m_interactions.Count - 1; i >= 0; i--)
            {
                if (m_interactions[i].Value != null)
                    continue;

                m_interactions.RemoveAt(i);
            }
        }

        public void AddToInteractor(IInteractor interactor)
        {
            for (int i = 0; i < m_interactions.Count; i++)
                m_interactions[i].Value.TryToInteract(interactor);
        }

        public void RemoveFromInteractor(IInteractor interactor)
        {
            for (int i = 0; i < m_interactions.Count; i++)
                m_interactions[i].Value.RemoveInteraction(interactor);
        }

        public void Select(IInteractor interactor)
        {
            m_selectors.Add(interactor);

            if (m_selectors.Count > 1)
                return;

            for (var i = 0; i < m_interactions.Count; i++)
            {
                if (m_interactions[i].Value is not IObjectSelectionListener selectionListener)
                    continue;
                
                selectionListener.OnObjectSelected(interactor);
            }

            m_onSelect.Invoke();
        }

        public void Deselect(IInteractor interactor)
        {
            m_selectors.Remove(interactor);

            if (m_selectors.Count > 0)
                return;

            for (var i = 0; i < m_interactions.Count; i++)
            {
                if (m_interactions[i].Value is not IObjectSelectionListener selectionListener)
                    continue;

                selectionListener.OnObjectDeselected(interactor);
            }

            m_onDeselect.Invoke();
        }
    }
}
