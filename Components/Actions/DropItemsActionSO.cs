using EE.Core;
using EE.InventorySystem.Core;
using UnityEngine;

namespace EE.InventorySystem.Actions {
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

        IInventoryComponent inventory;
        private int GetRandomNumberToDropItems => Random.Range(OriginSO.MinNumberOfItemsToDrop, OriginSO.MaxNumberOfItemsToDrop);

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryComponent>();
        }
        public override void Enter() {
            if (OriginSO.RemoveOnExit) {
                return;
            }
            if (inventory.ItemCount <= 0) {
                return;
            }
            if (OriginSO.DropAllItems) {
                inventory.RemoveAllItems(OriginSO.DestroyItems);
            }
            else {
                var numbeOfItemsToDrop = GetRandomNumberToDropItems;
                for (int i = 0; i < numbeOfItemsToDrop; i++) {
                    if (inventory.CurrentItem != null) {
                        continue;
                    }
                    if (OriginSO.ReduceNumberOfItems && inventory.CurrentItem != null) {
                        inventory.RemoveItem(new Item(inventory.CurrentItem.ItemInfo), OriginSO.DestroyItems);
                    }
                    else {
                        inventory.RemoveItem(inventory.CurrentItem, OriginSO.DestroyItems);
                    }
                }
            }

        }
        public override void Exit() {
            if (!OriginSO.RemoveOnExit) {
                return;
            }
            if (inventory.ItemCount <= 0) {
                return;
            }
            if (OriginSO.DropAllItems) {
                inventory.RemoveAllItems(OriginSO.DestroyItems);
            }
            else {
                var numbeOfItemsToDrop = GetRandomNumberToDropItems;
                for (int i = 0; i < numbeOfItemsToDrop; i++) {
                    if (OriginSO.ReduceNumberOfItems) {
                        if (inventory.CurrentItem != null) {
                            inventory.RemoveItem(new Item(inventory.CurrentItem.ItemInfo), OriginSO.DestroyItems);
                        }
                    }
                    else {
                        inventory.RemoveItem(inventory.CurrentItem, OriginSO.DestroyItems);
                    }
                }
            }

        }
    }
}