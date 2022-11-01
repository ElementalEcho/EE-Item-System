using EE.Core;
using EE.ItemSystem;

namespace EE.UI.Actions {
    public class OpenUIActionSO : GenericActionSO<OpenUIAction> {


    }

    public class OpenUIAction : GenericAction {
        private OpenUIActionSO OriginSO => (OpenUIActionSO)base._originSO;
        IInventoryUser inventoryComponent;
        public override void Init(IHasComponents hasComponents) {
            inventoryComponent = hasComponents.GetComponent<IInventoryUser>();

        }
        public override void Enter() {
            //inventoryComponent.InventoryOpened();
            //OriginSO.CraftingController.CraftingMenuOpened();
        }
    }
}



