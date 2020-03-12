using UnityEngine;

public class Player : MonoBehaviour
{
    /*
     * Placeholder class for player
     * Has a property for holding a gun and a placeholder implementation for firing the gun
     * Also has a placeholder function for taking damage
     * along with a placeholder method of firing the equipped gun
     * It also has a field for debug testing of guns, see below
     */

    //when testing guns, set this field to the gun you want the player to have equipped
    [SerializeField]
    public Gun DebugEquipGunOnStart;

    //this is the gun that will be shot
    public Gun gun { get; set; }

    private void Awake()
    {
        gun = DebugEquipGunOnStart;
    }

    //int damage is the damage the player should be taking from the attack
    public void TakeDamage(int damage)
    {
        Debug.Log($"Player taking {damage} damage");
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(gun != null)
            {
                Debug.Log("pew pew, player shot gun");
                gun.Fire();
            }
            else
            {
                Debug.LogError("No gun equipped on the player script");
            }
        }
    }
}
