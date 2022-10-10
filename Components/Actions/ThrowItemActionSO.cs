using EE.Core;
namespace EE.InventorySystem.Actions {
    public class ThrowItemActionSO : GenericActionSO<ThrowItemAction> {

    }
    public class ThrowItemAction : GenericAction {
        IInventoryComponent inventory;
        private ThrowItemActionSO OriginSO => (ThrowItemActionSO)_originSO;

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryComponent>();
        }
        public override void Enter() {

        }

    }
}

//TODO move this to its own action
//public void ThrowItemInHand(IItemUser stateController) {
//    stateController.StartIEnumerator(ThrowItemNumerator(itemInHand, Vector2.one));
//    if (itemInHand != null) {
//        itemInHand.RemovedFromHand();
//        itemInHand.PoolableComponent.transform.SetParent(null);
//    }
//    itemInHand = null;
//}

//private IEnumerator ThrowItemNumerator(IItem item, Vector2 direction) {
//    Vector3 vector3 = dropRange * direction;
//    vector3 += item.PoolableComponent.transform.position;

//    while (Vector2.Distance(item.PoolableComponent.transform.position, vector3) > 0.1f) {
//        float step = dropForce * Time.deltaTime;
//        item.PoolableComponent.transform.position = Vector3.MoveTowards(item.PoolableComponent.transform.position, vector3, step);

//        yield return null;
//    }

//}

