using UnityEngine;

public class DebugGun : Gun
{
    /*
     * This is a test gun that simply deals damage to all enemies in the scene.
     */

    Enemy[] enemies;
    int damage = 10;

    private void Awake()
    {
        //finds all enemies in the scene and stores them in an array
        enemies = FindObjectsOfType<Enemy>();
    }

    public override void Fire()
    {
        //damages each enemy in the array(and scene)
        foreach(Enemy enemy in enemies)
        {
            enemy.TakeDamage(damage);
        }
    }
}
