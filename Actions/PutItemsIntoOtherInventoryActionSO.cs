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

        IInventoryUser inventory;
        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryUser>();

        }

        public override void Enter(IHasComponents hasComponents) {
            if (_originSO.Reverse) {
                if (hasComponents.TryGetComponent(out IInventoryUser inventoryComponent) && inventoryComponent.CurrentItem != null) {
                    if (OriginSO.AllÍtems) {
                        foreach (var item in inventoryComponent.GetItems()) {
                            inventory.Add(item);
                        }
                        inventoryComponent.RemoveAll();
                    }
                    else {
                        inventory.Add(inventoryComponent.CurrentItem);
                        inventoryComponent.Remove(inventory.CurrentItem.ItemInfo, inventory.CurrentItem.NumberOfItems);
                    }

                }
            }
            else {
                if (hasComponents.TryGetComponent(out IInventoryUser inventoryComponent) && inventory.CurrentItem != null) {
                    if (OriginSO.AllÍtems) {
                        foreach (var item in inventory.GetItems()) {
                            inventoryComponent.Add(item);
                        }
                        inventory.RemoveAll();
                    }
                    else {
                        inventoryComponent.Add(inventory.CurrentItem);
                        inventory.Remove(inventory.CurrentItem.ItemInfo, inventory.CurrentItem.NumberOfItems);
                    }
                }
            }


        }
    }
}
