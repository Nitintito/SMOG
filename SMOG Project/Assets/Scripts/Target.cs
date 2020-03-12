using System.Collections;
using UnityEngine;

public class Target : Enemy
{
    [SerializeField] Material takeDamageMaterial;
    Material defaultMat;
    MeshRenderer mr;

    //DEBUG: seconds to stay swapped to another material when damaged, only used for debug visual feedback when taking damage
    [SerializeField] float BlinkSeconds;

    private void Awake()
    {
        //setup for blinking
        mr = GetComponent<MeshRenderer>();
        defaultMat = mr.material;
    }


    public override void TakeDamage(int damage)
    {
        Debug.Log($"Ouch! enemy took {damage} damage");
        StartCoroutine(ColorBlink());
    }

    IEnumerator ColorBlink()
    {
        mr.material = takeDamageMaterial;
        yield return new WaitForSeconds(BlinkSeconds);
        mr.material = defaultMat;
    }
}
