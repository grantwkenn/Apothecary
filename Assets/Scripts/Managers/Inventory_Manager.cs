using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//TODO Find where items are added or discarded and only use those functions as gate keepers,
// these functions are responsible for checking quest progression and calling refresh on the GUI Menus.

//Redo the offer item schema in favor of the quantity stacking schema


public class Inventory_Manager : MonoBehaviour
{
    Quest_Manager qm;
    Scene_Manager sm;
    Player player;
    Data_Persistence dp;
    Tile_Manager tm;
    Menu_Manager mm;
    Inventory_Bar_Menu invBar;

    Item emptyItem;
    int emptyID;
    string emptyName;

    [SerializeField]
    Item_Catalogue itemCatalogue;

    //items may not neccessarilly be in index order in the catalogue collection
    Dictionary<int, Item_Data> itemData_by_ID;
    Dictionary<string, Item_Data> itemData_by_Name;

    
    //TODO these references should not exist in new menu manager schema
    //public RectTransform inv;
    

    
    //Transform menuSelector;

    [SerializeField]
    Item[] inventory;


    byte inventorySize = 44;
    byte rowSize = 11;


    //int occupiedSlots;
    int freeSlots;



    byte barSelection;

    //public Item_List itemList;

    AudioSource gmAudioSource;

    private void Start()
    {
        
        loadInventory();
        freeSlots = inventory.Length;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].getData() == null)
                inventory[i] = emptyItem;

            else if (!inventory[i].isEmpty())
                freeSlots--;
        }
    }

    private void OnEnable()
    {
        qm = GetComponentInParent<Quest_Manager>();
        sm = GetComponentInParent<Scene_Manager>();
        tm = GetComponentInParent<Tile_Manager>();
        mm = GetComponentInParent<Menu_Manager>();
        invBar = GameObject.FindGameObjectWithTag("HUD").transform.Find("Inventory Bar").GetComponent<Inventory_Bar_Menu>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        gmAudioSource = GetComponentInParent<AudioSource>();

        //persistenceData = this.GetComponentInParent<Scene_Manager>().sp;

        //mappings of item catalogue
        itemData_by_ID = new Dictionary<int, Item_Data>();
        itemData_by_Name = new Dictionary<string, Item_Data>();

        //populate the mapping of item No to Item_Data
        foreach (Item_Data item_data in itemCatalogue.getCatalogue())
        {
            itemData_by_ID.Add(item_data.getItemNo(), item_data);
            itemData_by_Name.Add(item_data.getName(), item_data);
        }

        emptyItem = new Item(itemData_by_Name["Empty"],0);
        emptyName = emptyItem.getName();
        emptyID = emptyItem.getItemNo();

        //This line may change if we have variable bag size
        inventory = new Item[inventorySize];

        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = emptyItem;
        }

        dp = sm.getDataPersistence();


    }




    /// <summary>
    /// /////PERSISTENCE ////////////////
    /// </summary>

    void loadInventory()
    {
        //get serializedInventory from PP
        List<SerializableItem> sInv = dp.getSerializableInventory();

        //set inventory from pp data

        foreach(SerializableItem sItem in sInv)
        {
            Item_Data data = itemData_by_ID[sItem.itemID];
            inventory[sItem.index] = new Item(data, sItem.quantity);
            freeSlots--;
        }
        
    }
    

    //TODO maybe this can be done in real time as each item is updated instead of all on scene exit? 
    public void storePersistenceData()
    {
        //pp.setItems(inventory);
        dp.setItems(serializeInventory());
        dp.setInvSelection(barSelection);
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// </summary>


    //TODO need to change the offer item system in favor of the quantity stacking schema below
    //TODO refresh menus here
    private void addItem(Item item)
    {
        int quantityAdded = item.getQuantity();
        
        if (item.getData().getStackLimit() > 1)
        {
            //first look for this item already in stack
            for (int i = 0; i < inventorySize; i++)
            {
                Item invItem = inventory[i];
                if (isSameItem(invItem, item))
                {
                    //attempt to stack onto this item
                    int space = (invItem.getData().getStackLimit() - invItem.getQuantity());
                    if (space > item.getQuantity())
                    {
                        invItem.addQuantity(item.getQuantity());
                        item.subtractQuantity(item.getQuantity());
                        break;
                    }
                    else
                    {
                        invItem.addQuantity(space);
                        item.subtractQuantity(space);
                    }
                }
            }
        }

        if (item.getQuantity() > 0)
        {
            //prefer the selected slot first
            if (inventory[barSelection].isEmpty())
            {
                inventory[barSelection] = item;
                freeSlots--;
            }

            else
            {
                //find the next empty slot
                for (int i = 0; i < inventorySize; i++)
                {
                    if (inventory[i].isEmpty())
                    {
                        inventory[i] = item;
                        freeSlots--;
                        break;
                    }
                }
            }
        }

        //playSound();

        //notify the Quest Manager
        //qm.itemPickedUp(item.itemNo);

        dp.setItems(serializeInventory());

        qm.itemAdded(item.getData().getItemNo(), quantityAdded);

        mm.refresh();
        
        //evaluate the selector in case we just picked up a tool. Is this neccessary?
        evaluateSelector();
    }

    public Item[] getSelectedRow()
    {
        Item[] firstRow = new Item[11];
        Array.Copy(inventory, 0, firstRow, 0, 11);

        return firstRow;

    }

    int countItem(Item item)
    {
        int count = 0;
        for (int i = 0; i < inventorySize; i++)
        {
            if(isSameItem(inventory[i], item))
            {
                count += inventory[i].getQuantity();
            }
        }
        return count;
    }


    //TODO fix this with new stacking schema
    public void discardBarSelection()
    {
        Item item = inventory[barSelection];
        int remaining = countItem(item) - item.getQuantity();
        qm.itemRemoved(inventory[barSelection].getItemNo(), remaining);
        inventory[barSelection] = emptyItem;
        freeSlots++;


        mm.refresh();
        dp.setItems(serializeInventory());

        //in case we just set down a tool
        evaluateSelector();
    }

    public void discardMenuSelection(int selection)
    {
        Item item = inventory[selection];
        int remaining = countItem(item) - item.getQuantity();
        qm.itemRemoved(inventory[selection].getItemNo(), remaining);
        inventory[selection] = emptyItem;
        freeSlots++;


        mm.refresh();
        dp.setItems(serializeInventory());

        //in case we just set down a tool
        evaluateSelector();
    }


    void playSound()
    {
        //TODO add sound back in
        //gmAudioSource.Play();
    }


    //will need to fix for stackables...
    public bool offerItems(List<Item> items)
    {
        if (!isSpaceForItems(items)) return false;

        foreach (Item i in items)
        {
            addItem(i);
        }

        return true;
    }

    public bool offerItem(Item item)
    {
        if (slotNeededForItem(item) && freeSlots == 0) return false;
        addItem(item);
        return true;
    }

    public void removeObjectives(List<Gather_Objective> gathers)
    {
        foreach (Gather_Objective ob in gathers)
        {
            removeItem(ob.getData().getItemID(), ob.getData().getNumToGather());
        }
    }

    public void useItem()
    {
        Item item = inventory[barSelection];


        if (item.getData() is Consumable_Data)
        {
            Consumable_Data cd = (Consumable_Data)item.getData();
            player.heal(cd.getHealth());
            if(item.deplete())
            {
                //delete this item
                consumeSelection();
            }
            mm.refresh();

            //perform a bounce 'animation' on the inventory bar
            //TODO make this a coroutine so the item deletes after the animation finishes?
            invBar.bounceItem(barSelection);
        }


        string itemName = inventory[barSelection].getData().getName();

        if (itemName == "Sword") player.executePlayerFn(0);
        else if (itemName == "Hoe") player.executePlayerFn(1);
        else if (itemName == "Watering Can" && tm.canWater())
        {
            player.executePlayerFn(2);
        }

        else if (itemName == "Red Potion")
        {
            //TODO add subclass to Item_Data to Consumable, extra fields for healing value
            // Hard Coded healing HP = 3 for now
            player.heal(3);
            consumeSelection();
        }

        else if (itemName == "Watermelon Seeds")
        {
            //tile manager needs to execute
            if(tm.plant(inventory[barSelection].getName()))
                consumeSelection();
        }
    }

    void consumeSelection()
    {
        removeItemHelper(barSelection, 1);

        dp.setItems(serializeInventory());
        //refresh UI
        mm.refresh();
        evaluateSelector();
    }

    public void removeItem(int itemID, int quantity)
    {
        //check each item and remove until quantity has been discarded.
        //if insufficient quantity, item will be converted to empty

        int quantityToRemove = quantity;

        //search every item for match, then subtract amount
        for (int i = inventory.Length-1; i>=0; i--)
        {
            Item item = inventory[i];
            if (item.getData().getItemNo() != itemID) continue;

            //quantity of this item
            int itemQuantity = item.getQuantity();

            if (quantityToRemove < itemQuantity)
            {
                removeItemHelper(i, quantityToRemove);
                quantityToRemove = 0;
            }
                             
            else
            {
                removeItemHelper(i, itemQuantity);
                quantityToRemove -= itemQuantity;
            }
                
                      
            if (quantityToRemove == 0) break;
        }

        dp.setItems(serializeInventory());
        //refresh UI
        mm.refresh();
        evaluateSelector();
    }

    void removeItemHelper(int index, int quantity)
    {        
        int remainder = inventory[index].getQuantity() - quantity;

        //delete item
        if (remainder == 0)
        {
            inventory[index] = emptyItem;
            freeSlots--;
        }
        else //some remains
        {
            inventory[index].subtractQuantity(quantity);
        }
        qm.itemRemoved(inventory[index].getItemNo(), quantity);
    }

    public Item[] getItems()
    {
        return inventory;
    }

    public int getInvSize() { return this.inventorySize; }

    public void setItems(Item[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            inventory[i] = new Item(items[i].getData(), items[i].getQuantity());
        }
    }


    //check if there is space for all items in the list, including stacking
    //This code is not efficient, but it should only work on sets of constant size N = 1-5
    //Quest Rewards, etc. will not be very long lists.
    public bool isSpaceForItems(List<Item> items)
    {
        if (items.Count == 0) return true;

        int emptySpace = freeSlots;
        
        //b/c of the nested loop traversal, keep track of which items have been searched
        bool[] searched = new bool[items.Count];

        for(int i=0; i< items.Count; i++)
        {
            //Skip the extra manipulation for items that don't stack
            if(items[i].getData().getStackLimit() < 2 && slotNeededForItem(items[i])) // why AND slotNeeded?
            {                
                emptySpace--;
                continue;
            }               
            
            List<Item> sameItems = new List<Item>();
            //Construct a list of similar items to i
            for(int j=0; j< items.Count; j++)
            {
                if(isSameItem(items[i], items[j]))
                {
                    sameItems.Add(items[i]);
                    searched[j] = true;
                }
                    
            }
            if(sameItems.Count > 1)
            {
                //this list includes similar items, need to calculate with separate helper Fn:
                emptySpace -= slotsNeededSameItems(sameItems);
            }
            else if(slotNeededForItem(items[i]))
                emptySpace--;

            searched[i] = true;
        }

        return emptySpace >= 0;
    }

    //Determine if a slot is needed for this item after stacking
    bool slotNeededForItem(Item item)
    {
        int quantity = item.getQuantity();
        int stackLimit = item.getData().getStackLimit();
        for(int i=0; i<inventorySize; i++)
        {
            Item indexItem = inventory[i];
            if (isSameItem(indexItem, item))
            {
                quantity -= stackLimit - indexItem.getQuantity();
            }
        }
        if (quantity > 0) return true; //we need a slot for this item
        return false;
    }

    //This helper function will take a list of sameItems and evaluate the number of empty spaces needed
    // given current stacks in inventory
    int slotsNeededSameItems(List<Item> sameItems)
    {
        int quantity = 0;
        foreach(Item i in sameItems)
        {
            quantity += i.getQuantity();
        }
        Item item = sameItems[0];
        int stackLimit = item.getData().getStackLimit();
        for (int i = 0; i < inventorySize; i++)
        {
            Item indexItem = inventory[i];
            if (isSameItem(indexItem, item))
            {
                quantity -= stackLimit - indexItem.getQuantity();
            }
        }

        return quantity % stackLimit;
    }
    

    //count of an ItemID on hand
    public int countItem(int itemID)
    {
        int count = 0;
        foreach(Item item in inventory)
        {
            if (item == null || item.getData().getItemNo() != itemID) continue;

            count += (int)item.getQuantity();

        }

        return count;
    }

    public void evaluateSelector()
    {
        tm.setSelectionHilight(inventory[barSelection].getData().isTileSelector());
    }

    public void setBarSelection(byte selection) { this.barSelection = selection; }

    bool isSameItem(Item i1, Item i2) { return i1.getName() == i2.getName(); }
    
    public List<SerializableItem> serializeInventory()
    {
        List<SerializableItem> list = new List<SerializableItem>();

        for(int i=0; i<inventory.Length; i++)
        {

            Item item = inventory[i];
            if (item.isEmpty()) continue;
            list.Add(new SerializableItem(i, item.getItemNo(), item.getQuantity()));
        }

        return list;
    }

}

[System.Serializable]
public class SerializableInventory
{
    int containerID;
    List<SerializableItem> inventory;


    public SerializableInventory(int container, List<SerializableItem> inv)
    {
        this.containerID = container;
        this.inventory = inv;
    }

    public List<SerializableItem> getItems() { return this.inventory; }

}

[System.Serializable]
public class SerializableItem
{
    public int index;
    public int itemID;
    public int quantity;

    public SerializableItem(int idx, int id, int qt)
    {
        index = idx;
        itemID = id;
        quantity = qt;
    }

}





