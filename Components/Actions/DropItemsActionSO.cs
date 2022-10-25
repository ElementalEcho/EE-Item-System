using EE.Core;
using UnityEngine;

namespace EE.ItemSystem.Actions {
    [CreateAssetMenu(menuName = "StateMachine/Action/DropItemAction", fileName = "DropItemAction")]
    public class DropItemsActionSO : GenericActionSO<DropItemAction> {
        [SerializeField]
        private int minNumberOfItemsToDrop = 1;
        internal int MinNumberOfItemsToDrop => minNumberOfItemsToDrop;
        [SerializeField]
        private int maxNumberOfItemsToDrop = 1;
        internal int MaxNumberOfItemsToDrop => maxNumberOfItemsToDrop;

        [SerializeField]
        private bool dropAllItems = false;
        internal bool DropAllItems => dropAllItems;

        [SerializeField]
        private bool destroyItems = false;
        public bool DestroyItems => destroyItems;

        [SerializeField]
        private bool reduceNumberOfItems = false;
        public bool ReduceNumberOfItems => reduceNumberOfItems;
        [SerializeField]
        private bool removeOnExit = false;
        public bool RemoveOnExit => removeOnExit;
    }
    public class DropItemAction : GenericAction {
        private DropItemsActionSO OriginSO => (DropItemsActionSO)_originSO;

        IInventoryUser inventory;
        private int GetRandomNumberToDropItems => Random.Range(OriginSO.MinNumberOfItemsToDrop, OriginSO.MaxNumberOfItemsToDrop);

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryUser>();
        }
        public override void Enter() {
            if (OriginSO.RemoveOnExit) {
                return;
            }
            if (inventory.NumberOfFilledSlots <= 0) {
                return;
            }
            if (OriginSO.DropAllItems) {
                inventory.RemoveAllItems();
            }
            else {
                var numbeOfItemsToDrop = GetRandomNumberToDropItems;
                for (int i = 0; i < numbeOfItemsToDrop; i++) {
                    if (inventory.CurrentItem != null) {
                        continue;
                    }
                    if (OriginSO.ReduceNumberOfItems) {
                        inventory.RemoveItem(OriginSO.DestroyItems, inventory.CurrentItem.ItemInfo, inventory.CurrentItem.NumberOfItems);
                    }
                }
            }

        }
        public override void Exit() {
            if (!OriginSO.RemoveOnExit) {
                return;
            }
            if (inventory.NumberOfFilledSlots <= 0) {
                return;
            }
            if (OriginSO.DropAllItems) {
                inventory.RemoveAllItems();
            }
            else {
                var numbeOfItemsToDrop = GetRandomNumberToDropItems;
                for (int i = 0; i < numbeOfItemsToDrop; i++) {
                    if (OriginSO.ReduceNumberOfItems) {
                        if (inventory.CurrentItem != null) {
                            inventory.RemoveItem(OriginSO.DestroyItems, inventory.CurrentItem.ItemInfo, inventory.CurrentItem.NumberOfItems);
                        }
                    }
                    else {
                        inventory.RemoveItem(OriginSO.DestroyItems, inventory.CurrentItem.ItemInfo, inventory.CurrentItem.NumberOfItems);
                    }
                }
            }

        }
    }
}