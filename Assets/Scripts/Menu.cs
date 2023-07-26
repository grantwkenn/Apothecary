using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{

    public abstract void handleInput(direction urdl);

    public abstract void refresh();

}
