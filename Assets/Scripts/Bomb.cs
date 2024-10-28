using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float timeToTrigger, timeToExplode;
    private float time;
    public Animator anim;
    private bool triggered;
    private Vector2Int cord;

    private void Awake()
    {
        timeToTrigger = 1.3f;
        timeToExplode = 2.5f;
        triggered = false;
    }

    private void Start()
    {
        cord = Grid.instance.PosToMat(transform.position);
    }

    private void FixedUpdate()
    {
        if (time < timeToTrigger)
        {
            time += Time.deltaTime;
        }
        else if (!triggered)
        {
            anim.SetTrigger("attack01");
            triggered = true;
        } else if (time < timeToExplode) {
            time += Time.deltaTime;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        SoundManagement.sm.PlayBomb();
    }

    private void OnDestroy()
    {
        GameEvents.OnDestroyBombInvoke(cord);
    }
}
