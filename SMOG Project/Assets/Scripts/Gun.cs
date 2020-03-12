using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    /*
     * This is a master class for guns that all guns should inherit from
     * It contains a placeholder function for Ammo which is readonly outside of inheriting classes
     * as well as a function for firing that is required to overwrite
     */

    public int Ammo { get; protected set; }
    public abstract void Fire();
}
