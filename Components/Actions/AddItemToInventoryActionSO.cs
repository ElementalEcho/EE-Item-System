using EE.Core;
using UnityEngine;


namespace EE.ItemSystem.Actions {
    public class AddItemToInventoryActionSO : GenericActionSO<AddItemToInventoryAction> {

        [SerializeField]
        private Item[] itemToAdd = new Item[0];

        [SerializeField]
        private InspectorItem[] itemsToAdd = new InspectorItem[0];
        public Item[] ItemsToAdd => itemsToAdd.GetItems();

        [SerializeField]
        private bool destroyItems = false;
        public bool DestroyItems => destroyItems;
    }
    public class AddItemToInventoryAction : GenericAction {
        IInventoryUser inventory;
        private AddItemToInventoryActionSO OriginSO => (AddItemToInventoryActionSO)_originSO;

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryUser>();
        }
        public override void Enter() {
            if (OriginSO.Reverse) {
                foreach (var item in OriginSO.ItemsToAdd) {
                    inventory.RemoveItem(OriginSO.DestroyItems, item.ItemInfo, item.NumberOfItems);
                }
            }
            else {
                foreach (var item in OriginSO.ItemsToAdd) {
                    inventory.AddItem(item);
                }
            }

        }
        protected override bool Decide() {
            //foreach (var item in OriginSO.ItemsToAdd) {
            //    if (!inventory.CanAddItem(item)) {
            //        return false;
            //    }
            //}
            Debug.LogError("Not implemented");
            return true;
        }
    }
}