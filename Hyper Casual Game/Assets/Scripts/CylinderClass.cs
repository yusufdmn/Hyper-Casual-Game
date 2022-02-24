using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderClass : MonoBehaviour
{

    private bool _isFilled;
    private float _volume;

    public void InclemetCylinderVolume(float addingSize)
    {

        _volume += addingSize;
        if(_volume > 1)
        {            
            //Add new Cylinder 
            float leftVolume = _volume - 1;
            int cylinderCount = CharacterControllerScript.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);

            CharacterControllerScript.Current.CreateCylinder(leftVolume);
        }
        else if(_volume < 0)
        {
            //destroy cylinder
            CharacterControllerScript.Current.DestroyCylinder(this);
        }
        else
        {
            //increase current cylinder's volume

            int cylinderCount = CharacterControllerScript.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f * _volume, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _volume, transform.localScale.y, 0.5f * _volume);

        }

    }

}
