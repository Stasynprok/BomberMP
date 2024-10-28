using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagement : MonoBehaviour
{
    public AudioClip collectible;
    public AudioClip bomb;
    public AudioClip hurt;
    public AudioClip footSteps;
    private AudioSource AS;

    public static SoundManagement sm;

    private void Awake()
    {
        sm = this;
        AS = GetComponent<AudioSource>();
    }

    public void PlayCollectible() {
        if (AS != null && collectible != null) {
            AS.PlayOneShot(collectible);
        }
    }

    public void PlayBomb() {
        if (AS != null && bomb != null) {
            AS.PlayOneShot(bomb);
        }
    }

    public void PlayHurt() {
        if (AS != null && hurt != null) {
            AS.PlayOneShot(hurt);
        }
    }

    public void PlayFootStep() {
        if (AS != null && footSteps != null) {
            AS.PlayOneShot(footSteps);
        }
    }
}
