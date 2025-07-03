using System;
using CodeExamples.Codebase.InteractionSystem.Core;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage
{
    public class ObjectLockInteraction : MonoInputInteraction, IInteractionBlocker
    {
        [TabGroup("Lock Properties")]
        [SerializeField]
        private ObjectLockStatus m_lockStatus = ObjectLockStatus.Locked;

        [TabGroup("Lock Events")]
        [SerializeField]
        private UnityEventArray m_onLockedEvent;

        [TabGroup("Lock Events")]
        [SerializeField]
        private UnityEventArray m_onUnlockedEvent;

        public ValueReference<bool> IsBlocking { get; private set; }
        public ObjectLockStatus LockStatus => m_lockStatus;
        
        public ObservableNoParam OnOpened { get; } = new ObservableNoParam();
        public ObservableNoParam OnClosed { get; } = new ObservableNoParam();

        #region IInitialziation

        public override void EarlyInitialize(EESceneInitializationManager manager)
        {
            base.EarlyInitialize(manager);
            IsBlocking = new ValueReference<bool>(true, m_lockStatus == ObjectLockStatus.Locked);
        }

        #endregion

        #region Input Interaction

        public override void InteractionInputDown(IInteractor interactor)
        {
            base.InteractionStart(interactor);

            if (m_lockStatus == ObjectLockStatus.Locked)
                Open();
            else
                Close();
        }

        protected override bool ShouldIgnoreInteractor(IInteractor interactor)
        {
            return interactor is not PlayerHoldableInventory;
        }

        #endregion

        public void Close()
        {
            m_lockStatus = ObjectLockStatus.Locked;
            IsBlocking.Value = true;
            m_onLockedEvent.Invoke();
            OnClosed.Invoke();
        }

        public void Open()
        {
            m_lockStatus = ObjectLockStatus.Unlocked;
            IsBlocking.Value = false;
            m_onUnlockedEvent.Invoke();
            OnOpened.Invoke();
        }

        [Serializable]
        public enum ObjectLockStatus
        {
            Locked = 0,
            Unlocked = 1
        }
    }
}
