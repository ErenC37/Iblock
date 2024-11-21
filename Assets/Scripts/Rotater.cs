using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rotater : MonoBehaviour
{

    public void Dondur()
    {
        transform.Rotate(0, 0, -90);
        AudioManager.instance.Play("Button");
    }

}
