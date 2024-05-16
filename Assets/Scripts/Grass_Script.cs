using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass_Script : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D entity)
    {
        bool isLeft = entity.transform.position.x < this.transform.position.x;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<Tile_Manager>()
            .animateGrass(this.GetComponent<Animator>(), isLeft);
    }


    private void OnTriggerExit2D(Collider2D entity)
    {
        bool isLeft = entity.transform.position.x < this.transform.position.x;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<Tile_Manager>()
            .animateGrass(this.GetComponent<Animator>(), isLeft);
    }


}