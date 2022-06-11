using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Manager : MonoBehaviour
{
    public Text textObj;
    public GameObject dialogueContainer;

    //public GameObject allDialogue;
    Inventory_Manager invMan;
    Dialogue_Manager dialogueManager;


    Transform arrow;
    Vector3 arrowRefPos;
    float arrowRefY;

    byte[] arrowAnimationHeights;
    bool animateArrow = false;
    byte animationTimer = 0;
    byte frameTime = 4;
    byte arrowAnimationIndex = 0;

    Image arrowImage;


    // Start is called before the first frame update
    void Start()
    {
        invMan = GetComponentInParent<Inventory_Manager>();

        dialogueManager = GetComponentInParent<Dialogue_Manager>();

        dialogueContainer.SetActive(false);

        arrow = dialogueContainer.transform.Find("Arrow");

        arrowRefPos = arrow.transform.localPosition;
        
        arrowAnimationHeights = new byte[8] { 0,1,2,4,2,1,0,0 };

        arrowImage = arrow.GetComponent<Image>();
        
    }


    private void FixedUpdate()
    {
        if (animateArrow)
        {
            if (!arrowImage.IsActive()) arrowImage.enabled = true;
            if (animationTimer == frameTime)
            {
                arrowAnimationIndex++;
                arrow.transform.localPosition = new Vector3(arrowRefPos.x, arrowRefPos.y - arrowAnimationHeights[arrowAnimationIndex], arrowRefPos.z);
                animationTimer = 0;

                if (arrowAnimationIndex == arrowAnimationHeights.Length - 1)
                    arrowAnimationIndex = 0;
            }
            else
                animationTimer++;

        }
        else if (arrowImage.IsActive()) arrowImage.enabled = false;

    }


    public void activateTextBox()
    {
        if (dialogueContainer.activeSelf) return;
        
        animationTimer = 0;
        arrowAnimationIndex = 0;

        dialogueContainer.SetActive(true);
    }

    public void deactivateTextBox()
    {
        animateArrow = false;
        animationTimer = 0;
        arrowAnimationIndex = 0;
        dialogueContainer.SetActive(false);

    }

    public void setArrow(bool value)
    {
        animateArrow = value;
    }


    public void setText(string text)
    {
        textObj.text = text;
    }


}
