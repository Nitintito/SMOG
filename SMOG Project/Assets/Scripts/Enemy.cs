using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    /*
     * This is a master class for enemies that all enemies should inherit from
     * It contains a placeholder function for taking damage that must be overrided 
     */

    //int damage is the damage the enemy should take
    public abstract void TakeDamage(int damage);
}
