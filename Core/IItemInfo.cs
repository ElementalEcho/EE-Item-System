using EE.Core;
using EE.Core.PoolingSystem;
namespace EE.InventorySystem {
    public interface IItemInfo {
        PoolableComponent ItemToDrop { get; }
        int ManaToGive { get; }
        int MaxItemStack { get; }
        string PrefabGuid { get; }
        GenericActionSO[] StartItemUseEffects { get; }
        GenericActionSO[] AttackItemUseEffects { get; }
        GenericActionSO[] ThrownItemEffects { get; }

    }


}


