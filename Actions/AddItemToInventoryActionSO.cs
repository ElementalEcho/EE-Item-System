using EE.Core;
using EE.InventorySystem.Core;
using EE.InventorySystem.Impl;
using UnityEngine;


namespace EE.InventorySystem.Actions {
    public class AddItemToInventoryActionSO : GenericActionSO<AddItemToInventoryAction> {

        [SerializeField]
        private Item[] itemToAdd = new Item[0];
        public Item[] ItemToAdd => itemToAdd;

        [SerializeField]
        private bool destroyItems = false;
        public bool DestroyItems => destroyItems;
    }
    public class AddItemToInventoryAction : GenericAction {
        IInventoryComponent inventory;
        private AddItemToInventoryActionSO OriginSO => (AddItemToInventoryActionSO)_originSO;

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryComponent>();
        }
        public override void Enter() {
            if (OriginSO.Reverse) {
                foreach (var item in OriginSO.ItemToAdd) {
                    inventory.RemoveItem(item, OriginSO.DestroyItems);
                }
            }
            else {
                foreach (var item in OriginSO.ItemToAdd) {
                    inventory.AddItem(item);
                }
            }

        }
        protected override bool Decide() {
            foreach (var item in OriginSO.ItemToAdd) {
                if (!inventory.CanAddItem(item)) {
                    return false;
                }
            }
            return true;
        }
    }
}