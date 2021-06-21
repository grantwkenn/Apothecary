using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public float thrust;
    public float knockTime;


    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if(other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody2D enemy = other.GetComponent<Rigidbody2D>();
            if(enemy != null)
            {
                enemy.bodyType = RigidbodyType2D.Dynamic;
                Vector2 difference = enemy.transform.position - transform.position;
                difference = difference.normalized * thrust;
                enemy.AddForce(difference, ForceMode2D.Impulse);
                //enemy.GetComponentInParent<WoodCrab>().knockBack = true;
                StartCoroutine(KnockCo(enemy));
            }
        }

    }

    private IEnumerator KnockCo(Rigidbody2D other)
    {
        if (other != null)
        {
            yield return new WaitForSeconds(knockTime);
            other.velocity = Vector2.zero;
            //other.GetComponentInParent<Enemy>().knockBack = false;

            Debug.Log("done");
        }    
    }
}
