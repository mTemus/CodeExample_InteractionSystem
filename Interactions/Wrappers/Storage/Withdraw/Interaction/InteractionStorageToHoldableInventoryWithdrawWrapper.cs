using System;
using CodeExamples.Codebase.InteractionSystem.Core;
using UnityEngine;

namespace CodeExamples.Codebase.InteractionSystem.Interactions.Wrappers.Storage.Withdraw.Interaction
{
    public class InteractionStorageToHoldableInventoryWithdrawWrapper : MonoInputInteraction
    {
        [TabGroup("Requirements")]
        [SerializeField, Required]
        private InterfaceReference<IStorage> m_storage;

        #region Initialization

        public override void EarlyInitialize(EESceneInitializationManager manager)
        {
            base.EarlyInitialize(manager);
            m_storage.Value.OnStorageStockChanged.AddListener(OnStockChanged);
        }

        public override void Uninitialize(EESceneInitializationManager manager)
        {
            m_storage.Value.OnStorageStockChanged.RemoveListener(OnStockChanged);
        }

        #endregion

        #region Observers

        private void OnStockChanged(IStorage storage)
        {
            m_isAvailable.Value = CheckAvailability();
        }

        #endregion

        #region Interaction

        protected override bool CheckAvailability()
        {
            return base.CheckAvailability() && !m_storage.Value.IsEmpty;
        }

        protected override bool ShouldIgnoreInteractor(IInteractor interactor)
        {
            return interactor.As<EEMonoBehaviour>().Parent.GetEEInterface<IHoldableInventory>() == null;
        }

        protected override bool CanBeInteracted(IInteractor interactor)
        {
            if (m_storage.Value.IsEmpty)
                return false;

            var inventory = interactor.MainObject.GetEEInterface<IHoldableInventory>();

            if (inventory.IsFull)
                return false;

            return base.CanBeInteracted(interactor);
        }

        #endregion

        #region Input Interaction

        public override void InteractionInputDown(IInteractor interactor)
        {
            var storables = m_storage.Value.WithdrawAll(false);

            if (storables.Count == 1)
            {
                interactor.MainObject.GetEEInterface<IHoldableInventory>().Store(m_storage.Value.Withdraw());
                interactor.UnregisterAvailableInteraction(this);

            }
            else
            {
                var cameraPos = UnityEngine.Camera.main.transform.position;
                var cameraForward = UnityEngine.Camera.main.transform.forward;

                var storedHoldable = storables
                    .Select(s => s.As<EEMonoBehaviour>().Parent)
                    .Select(obj => new { obj, distance = Vector3.Dot(cameraForward, (obj.transform.position - cameraPos).normalized) })
                    .Where(entry => entry.distance > 0)
                    .OrderByDescending(entry => entry.distance) 
                    .Select(entry => entry.obj.GetEEComponent<HoldableObject>() as IStorable)
                    .FirstOrDefault();

                if (storedHoldable == null)
                    throw new Exception("There is no holdable to withdraw for some reason.");

                if(m_storage.Value.TryWithdraw(storable => storable == storedHoldable, out IStorable withdrawableHoldable))
                {
                    interactor.MainObject.GetEEInterface<IHoldableInventory>().Store(withdrawableHoldable);
                    interactor.UnregisterAvailableInteraction(this);
                }
                else
                {
                    Debug.LogError($"{GetType().Name} --- Could not withdraw {storedHoldable.GetType().Name} from {m_storage.Value} by TryWithdraw().");
                }
            }
        }

        #endregion
    }
}
