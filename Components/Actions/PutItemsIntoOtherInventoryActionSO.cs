using EE.Core;
using UnityEngine;

namespace EE.ItemSystem.Actions {
    public class PutItemsIntoOtherInventoryActionSO : GenericActionSO<PutItemsIntoOtherInventoryAction> {
        [SerializeField]
        private bool allItems = false;
        public bool AllÍtems => allItems;


    }
    public class PutItemsIntoOtherInventoryAction : GenericAction {
        private PutItemsIntoOtherInventoryActionSO OriginSO => (PutItemsIntoOtherInventoryActionSO)_originSO;

        IInventoryComponent inventory;
        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryComponent>();

        }

        public override void Enter(IHasComponents hasComponents) {
            if (_originSO.Reverse) {
                if (hasComponents.TryGetComponent(out IInventoryComponent inventoryComponent) && inventoryComponent.CurrentItem != null) {
                    if (OriginSO.AllÍtems) {
                        foreach (var item in inventoryComponent.Items) {
                            inventory.AddItem(item);
                        }
                        inventoryComponent.RemoveAllItems(true);
                    }
                    else {
                        inventory.AddItem(inventoryComponent.CurrentItem);
                        inventoryComponent.RemoveItem(inventory.CurrentItem, true);
                    }

                }
            }
            else {
                if (hasComponents.TryGetComponent(out IInventoryComponent inventoryComponent) && inventory.CurrentItem != null) {
                    if (OriginSO.AllÍtems) {
                        foreach (var item in inventory.Items) {
                            inventoryComponent.AddItem(item);
                        }
                        inventory.RemoveAllItems(true);
                    }
                    else {
                        inventoryComponent.AddItem(inventory.CurrentItem);
                        inventory.RemoveItem(inventory.CurrentItem, true);
                    }
                }
            }


        }
    }
}
