using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory_Manager : MonoBehaviour
{
    Quest_Manager qm;
    Scene_Manager sm;
    Player player;


    public Item emptyItem;

    [SerializeField]
    Item_Catalogue itemCatalogue;

    Item_Data[] allItemData;

    //items may not neccessarilly be in index order in the catalogue collection
    Dictionary<int, Item_Data> itemData_by_ID;
    Dictionary<string, Item_Data> itemData_by_Name;

    public RectTransform inv;
    public RectTransform bar;

    Transform barSelector;
    Transform menuSelector;

    [SerializeField]
    Item[] inventory;


    Image[] menuImages;
    Image[] barImages;

    int inventorySize = 44;
    int rowSize = 11;


    int itemCount = 0;
    int emptyCount;
    int barSelection = 0;
    public int menuSelection = 0;

    int menuRow = 0;
    int menuCol = 0;
    int numRows = 4;
    int numCols = 11;

    bool inputBreak = true;

    Vector3[] barPositions;
    Vector3[] menuPositions;


    //public Item_List itemList;

    AudioSource gmAudioSource;

    private void OnEnable()
    {
        qm = GetComponentInParent<Quest_Manager>();
        sm = GetComponentInParent<Scene_Manager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //persistenceData = this.GetComponentInParent<Scene_Manager>().sp;
    }

    // Start is called before the first frame update
    void Start()
    {

        //inventorySize = persistenceData.inventorySize;

        allItemData = itemCatalogue.getCatalogue();
        itemData_by_ID = new Dictionary<int, Item_Data>();
        itemData_by_Name = new Dictionary<string, Item_Data>();

        //populate the mapping of item No to Item_Data
        foreach (Item_Data item_data in allItemData)
        {
            itemData_by_ID.Add(item_data.getItemNo(), item_data);
            itemData_by_Name.Add(item_data.getName(), item_data);
        }


        inventory = new Item[inventorySize];

        emptyItem = new Item(allItemData[0], 1);

        //string[] itemNames = sm.getItemNames();
        //Debug.Log(itemNames.Length);
        Item_Data[] spItemData = sm.getItemData();
        int[] spItemCounts = sm.getItemCounts();
        
        for (int i = 0; i < inventorySize; i++)
        {
            //inventory[i] = emptyItem;
            //Item_Data id = itemData_by_Name[itemNames[i]];
            //TODO get accurate quantities instead of 1
            inventory[i] = new Item(spItemData[i], spItemCounts[i]);
        }



        menuImages = new Image[inventorySize];

        //Find the Row Transform groups
        Transform[] rows = new RectTransform[4];
        for (int i = 0; i < rows.Length; i++)
        {
            string s = "row" + i;
            rows[i] = inv.Find(s);
        }

        //Find each slot Transform in each row
        for (int i = 0; i < rows.Length; i++)
        {
            //Find the row
            //string r = "row" + i;
            //Transform row = inv.Find(r);
            for (int j = 0; j < numCols; j++)
            {
                string s = "Slot" + j;
                Transform slot = rows[i].Find(s);
                menuImages[(i * numCols) + j] = slot.GetComponentInChildren<Image>();
            }
        }


        barImages = new Image[numCols];
        //Find the Inventory Bar Transforms
        for (int i = 0; i < numCols; i++)
        {
            string s = "Slot" + i;
            Transform slot = bar.Find(s);
            barImages[i] = slot.GetComponentInChildren<Image>();
        }
        //placement of Bar Transforms
        barPositions = new Vector3[numCols];
        for (int i = 0; i < numCols; i++)
        {
            barPositions[i] = new Vector3(3 + i * 18, 3, 0);
        }
        //Placement of Menu Slots
        menuPositions = new Vector3[numRows * numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                float x = rows[i].transform.localPosition.x - 2;
                float y = rows[i].transform.localPosition.y - 2;
                menuPositions[(numCols * i) + j] = new Vector3(x + 18 * j, y, 0);

            }
        }

        //Selector Slot
        barSelector = bar.Find("Selection");
        menuSelector = inv.Find("Selection");

        gmAudioSource = GetComponentInParent<AudioSource>();

        emptyCount = inventorySize - itemCount;

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            int itemNo = inventory[i].getData().getItemNo();


            if (allItemData[itemNo].getSprite() != null)
            {
                menuImages[i].enabled = true;
                menuImages[i].sprite = allItemData[inventory[i].getData().getItemNo()].getSprite();
            }
            else
                menuImages[i].enabled = false;
        }


        for (int i = 0; i < numCols; i++)
        {
            if (allItemData[inventory[i].getData().getItemNo()].getSprite() != null)
            {
                barImages[i].enabled = true;
                barImages[i].sprite = allItemData[inventory[i].getData().getItemNo()].getSprite();
            }
            else
                barImages[i].enabled = false;
        }

        barSelector.transform.localPosition = barPositions[barSelection];
        menuSelector.transform.localPosition = menuPositions[menuSelection];

    }

    public bool inventoryFull()
    {
        return itemCount == inventorySize;
    }

    private void addItem(Item item)
    {

        //redundent
        if (inventoryFull()) return;

        if (inventory[barSelection].getName() == emptyItem.getName())
        {
            inventory[barSelection] = item;
        }

        else
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventory[i].getName() == emptyItem.getName())
                {
                    inventory[i] = item;
                    break;
                }
            }
        }

        itemCount++;
        emptyCount = inventorySize - itemCount;

        playSound();

        //notify the Quest Manager
        //qm.itemPickedUp(item.itemNo);
        qm.itemAdded(item.getData().getItemNo(), 1);

    }

    public Item[] getSelectedRow()
    {
        Item[] firstRow = new Item[11];
        Array.Copy(inventory, 0, firstRow, 0, 11);

        return firstRow;

    }


    public Item getEmptyItem()
    {
        return emptyItem;
    }

    public void toggleSelection(int val)
    {
        barSelection += val;
        if (barSelection >= rowSize)
            barSelection = 0;
        else if (barSelection < 0)
            barSelection = rowSize - 1;
    }

    public void inputUpdate(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            inputBreak = true;
            return;
        }

        if (!inputBreak) return;

        inputBreak = false;

        if (input.x < 0) // Left
        {
            if (menuCol > 0)
                menuCol--;
        }
        else if (input.x > 0) // LEFT
        {
            if (menuCol < numCols - 1)
                menuCol++;
        }

        else if (input.y < 0) // DOWN
        {
            if (menuRow < numRows - 1)
                menuRow++;
        }
        else if (input.y > 0) // UP
        {
            if (menuRow > 0)
                menuRow--;
        }

        menuSelection = (menuRow * numCols) + menuCol;
    }

    public int getSelectionNumber()
    {
        return barSelection;
    }


    public void discardSelection()
    {
        qm.itemRemoved(inventory[barSelection].getItemNo(), inventory[barSelection].getQuantity());
        inventory[barSelection] = emptyItem;
        itemCount--;

    }


    void playSound()
    {
        //TODO add sound back in
        //gmAudioSource.Play();
    }

    public bool enoughSpace(int number)
    {
        return emptyCount >= number;
    }

    //will need to fix for stackables...
    public bool offerItems(List<Item> items)
    {
        if (!enoughSpace(items.Count)) return false;

        foreach (Item i in items)
        {
            addItem(i);
        }


        return true;
    }

    public bool offerItem(Item item)
    {
        if (!enoughSpace(1)) return false;
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
        string itemName = inventory[barSelection].getData().getName();

        if (itemName == "Sword") player.executePlayerFn(0);
        if (itemName == "Shovel") player.executePlayerFn(1);

        if (itemName == "Red Potion")
        {
            //TODO add subclass to Item_Data to Consumable, extra fields for healing value
            // Hard Coded healing HP = 3 for now
            player.heal(3);
            consumeSelection();
        }
    }

    void consumeSelection()
    {
        removeItemHelper(barSelection, 1);
    }



    public void removeItem(int itemID, int quantity)
    {
        //check each item and remove until quantity has been discarded.
        //if insufficient quantity, item will be converted to empty

        int quantityToRemove = quantity;

        //search every item for match, then subtract amount
        for (int i = 0; i < inventory.Length; i++)
        {
            Item item = inventory[i];
            if (item.getData().getItemNo() != itemID) continue;

            //quantity of this item
            int itemQuantity = item.getQuantity();

            int remainder = quantityToRemove - itemQuantity;

            if (quantityToRemove < itemQuantity)
                removeItemHelper(i, quantityToRemove);               
            else
                removeItemHelper(i, itemQuantity);
            

            quantityToRemove -= itemQuantity;
            if (quantityToRemove <= 0) break;
        }

    }

    void removeItemHelper(int index, int quantity)
    {        
        int remainder = inventory[index].getQuantity() - quantity;

        if (remainder < 0) Debug.Log("ERROR: Can't remove more than exists");

        //delete item
        if (remainder == 0)
        {
            inventory[index] = emptyItem;
        }
        else if(remainder > 0) //some remains
        {
            inventory[index].subtractQuantity(quantity);
        }
        qm.itemRemoved(inventory[index].getItemNo(), quantity);
    }

    public Item[] getItems()
    {
        return inventory;
    }

    public void setItems(Item[] items)
    {
        for(int i=0; i<items.Length; i++)
        {
            inventory[i] = new Item(items[i].getData(), items[i].getQuantity());
        }    
    }



    //count of an ItemID on hand
    public int countItem(int itemID)
    {
        int count = 0;
        foreach(Item item in inventory)
        {
            if (item == null || item.getData().getItemNo() != itemID) continue;

            count += item.getQuantity();

        }

        return count;
    }



}





