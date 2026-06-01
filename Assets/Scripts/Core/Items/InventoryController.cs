using UnityEngine;

public abstract class InventoryController : MonoBehaviour
{
	[SerializeField] protected InventoryController _transferTarget;

	public InventoryController TransferTarget { get => _transferTarget; set => _transferTarget = value; }

	public abstract void InitializeView();
	public abstract bool HandleClick(int slotIndex);
	public abstract bool HandleUpdate(int changedSlotIndex);
	public abstract bool HandleReceive(ItemStack stack);
}
