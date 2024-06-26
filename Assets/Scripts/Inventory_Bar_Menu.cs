using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Bar_Menu : Menu
{
    Inventory_Manager im;
    Tile_Manager tm;
    Menu_Manager mm;
    GameObject GM;
    
    byte numRows = 4;
    byte barCount = 11;

    float localSlotHeight;

    Transform barSelector;
    Image[] barImages;
    Vector3[] barPositions;
    Image[] numberImages;

    Item[] inventory;

    byte selection = 0;

    int[] bouncingSlots;
    Transform[] slots;

    public float bouncingVelocity = -10f;
    public float bouncingAcceleration = 1f;

    public float[] bouncingPositions;


    private void OnEnable()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager");


        im = GM.GetComponent<Inventory_Manager>();
        tm = GM.GetComponent<Tile_Manager>();
        mm = GM.GetComponent<Menu_Manager>();

        numberImages = new Image[barCount * 3];
        bouncingSlots = new int[barCount];

        slots = new Transform[barCount];

        barImages = new Image[barCount];
        //Find the Inventory Bar Transforms
        barPositions = new Vector3[barCount];
        for (int i = 0; i < barCount; i++)
        {
            string s = "Slot" + i;
            Transform slot = this.transform.Find(s);
            slots[i] = slot;
            bouncingSlots[i] = -1;
            barImages[i] = slot.Find("Image").GetComponent<Image>();
            numberImages[(i * 3)] = slot.Find("hundreds").gameObject.GetComponent<Image>();
            numberImages[(i * 3)+1] = slot.Find("tens").gameObject.GetComponent<Image>();
            numberImages[(i * 3)+2] = slot.Find("ones").gameObject.GetComponent<Image>();

            barPositions[i] = new Vector3((3 + i * 22), 3, 0);

        }

        localSlotHeight = slots[0].localPosition.y;

        barSelector = this.transform.Find("Selector");

        

        //bouncingPositions = new Vector3[12];
        
    }


    // Start is called before the first frame update
    void Start()
    {
        selection = GM.GetComponent<Scene_Manager>().getDataPersistence().getInvSelection();

        updateSelection();

        inventory = im.getItems();
        refresh();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        for (int i = 0; i < bouncingSlots.Length; i++)
        {
            if(bouncingSlots[i] != -1)
            {
                slots[i].localPosition = new Vector3(slots[i].localPosition.x, localSlotHeight - bouncingPositions[bouncingSlots[i]], slots[i].localPosition.z);
                bouncingSlots[i]++;
                if (bouncingSlots[i] == bouncingPositions.Length)
                {
                    bouncingSlots[i] = -1;
                }
                    
            }
        }
    }



    public override void refresh()
    {
        //TODO this code is very sketchy should be optimized and doesn't need to be called every frame!
        // only need to refresh a slot when we consume, or pickup, or otherwise inventory goes in or out, 
        // these events are controlled by Inv Manager
        for (int i = 0; i < barCount; i++)
        {
            if (!inventory[i].isEmpty())
            {
                barImages[i].enabled = true;
                barImages[i].sprite = inventory[i].getSprite();

                int quant = inventory[i].getQuantity();
                byte hundreds = (byte)(quant / 100);
                byte ones = (byte)(quant % 10);
                byte tens = (byte)((quant % 100) / 10);
                if (inventory[i].getQuantity() > 1)
                {
                    numberImages[i * 3].enabled = true;
                    numberImages[i * 3].sprite = mm.getDigitSprite(hundreds);
                    numberImages[(i * 3) + 1].enabled = true;
                    numberImages[(i * 3) + 1].sprite = mm.getDigitSprite(tens);
                    numberImages[(i * 3) + 2].enabled = true;
                    numberImages[(i * 3) + 2].sprite = mm.getDigitSprite(ones);

                    if (hundreds == 0)
                    {
                        numberImages[i * 3].sprite = null;
                        numberImages[i * 3].enabled = false;

                        if (tens == 0)
                        {
                            numberImages[(i * 3) + 1].sprite = null;
                            numberImages[(i * 3) + 1].enabled = false;
                        }
                    }
                }

                else
                {
                    numberImages[i * 3].sprite = null;
                    numberImages[i * 3].enabled = false;
                    numberImages[(i * 3) + 1].sprite = null;
                    numberImages[(i * 3) + 1].enabled = false;
                    numberImages[(i * 3) + 2].sprite = null;
                    numberImages[(i * 3) + 2].enabled = false;
                }
            }
            else
            {
                barImages[i].enabled = false;
                numberImages[i * 3].enabled = false;
                numberImages[(i * 3) + 1].enabled = false;
                numberImages[(i * 3) + 2].enabled = false;
            }

        }
    }

    public void buttonPress(int index)
    {
        selection = (byte)index;
        updateSelection();

    }
    
    public override void handleInput(direction input)
    {
        if(input == direction.right)
        {
            selection += 1;
            if (selection == barCount)
                selection = 0;
        }
        else if (input == direction.left)
        {
            if (selection == 0)
                selection = (byte)(barCount - 1);
            else selection -= 1;
        }

        updateSelection();

    }

    void updateSelection()
    {
        barSelector.transform.localPosition = barPositions[selection];
        im.setBarSelection(selection);
        im.evaluateSelector();
    }

    public void bounceItem(int index)
    {
        //reset before allowing another bounce!
        bouncingSlots[index] = 0;
    }

}
