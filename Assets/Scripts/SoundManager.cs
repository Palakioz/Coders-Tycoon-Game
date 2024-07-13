using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public GameObject newSprite;
    public GameObject oldSprite;
    public GameObject SoundObject;

    public void toggleSoundOn()
    {
        newSprite.SetActive(true);
        oldSprite.SetActive(false);
        SoundObject.SetActive(true);
    }

    public void toggleSoundOff()
    {
        newSprite.SetActive(false);
        oldSprite.SetActive(true);
        SoundObject.SetActive(false);
    }


}
