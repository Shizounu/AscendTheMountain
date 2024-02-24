using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudioScript : MonoBehaviour
{
    public AudioClip RandomClip;

    public void UseAudioClip()
    {
        AudioManager.Instance.PlayAudioClip(RandomClip);
    }
}
