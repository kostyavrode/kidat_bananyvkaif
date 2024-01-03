using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public bool isCanRotate=true;
    public bool isCanInteract=true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Target" && isCanInteract)
        {
            isCanInteract = false;
            GameManager.instance.AddPrize(other.GetComponent<TargetPart>().targetPrize);
        }
    }
    private void Update()
    {
        if (isCanRotate)
            transform.Rotate(new Vector3(0, 1, 0));
    }
}
