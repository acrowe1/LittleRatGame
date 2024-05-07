using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
    {
        // Check if there is any slot with room left in its stack for the item
        if (ContainsItem(itemToAdd, out List<InventorySlot> invSlot))
        {
            foreach (var slot in invSlot)
            {
                if (slot.RoomLeftInStack(amountToAdd))
                {
                    slot.AddToStack(amountToAdd);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }

        // Check if there are any free slots available
        if (HasFreeSlot(out InventorySlot freeSlot))
        {
            freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
            OnInventorySlotChanged?.Invoke(freeSlot);
            return true;
        }

        return false;
    }

    public bool RemoveFromInventory(InventoryItemData itemToRemove, int amountToRemove, InventorySlot_UI uiSlot)
    {
        // Check if the item exists in the inventory
        if (ContainsItem(itemToRemove, out List<InventorySlot> invSlot))
        {
            foreach (var slot in invSlot)
            {
                if (slot.ItemData == itemToRemove)
                {
                    // If there are enough items in the slot
                    if (slot.StackSize >= amountToRemove)
                    {
                        slot.RemoveFromStack(amountToRemove);
                        OnInventorySlotChanged?.Invoke(slot);
                        Debug.Log("Amount left in stack: " + slot.StackSize);
                        uiSlot.UpdateUISlot(slot); // Update the UI slot
                        return true;
                    }
                    else
                    {
                        // If there are not enough items in the slot, remove all and continue
                        amountToRemove -= slot.StackSize;
                        slot.RemoveFromStack(slot.StackSize);
                        OnInventorySlotChanged?.Invoke(slot);
                        Debug.Log("Amount left in stack: 0");
                        uiSlot.UpdateUISlot(slot); // Update the UI slot
                    }
                }
            }
        }

        return false; // Item not found or not enough quantity to remove
    }

    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
    {
        invSlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
        // Debug.Log("invSlot Count: " + invSlot.Count);
        return invSlot.Count > 0;
    }


    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = inventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot != null;
    }
}
