using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    private int x;
    private int z;
    private bool isMoving;
    int[,] move;
    private Vector3 rotationAngles;
    System.Random rnd;

    public GameObject footStep;
    private FootStepPool _poolFoots;

    [Server]
    private void Awake()
    {
        x = z = 1;
        isMoving = false;
        move = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        rnd = new System.Random();
        _poolFoots = GetComponent<FootStepPool>();
    }

    [Server]
    private void Start()
    {
        Vector2Int pos = Grid.instance.GetCoordinate();
        //Debug.Log($"pos is {pos}");
        x = pos.x;
        z = pos.y;
    }
    [Server]
    private void OnEnable()
    {
        GameEvents.OnDestroyBomb += SelfDestroyHandler;
    }
    [Server]
    private void OnDisable()
    {
        GameEvents.OnDestroyBomb -= SelfDestroyHandler;
        GameEvents.OnDestroyEnemyInvoke();
        SoundManagement.sm.PlayHurt();
    }
    [Server]
    private void SelfDestroyHandler(Vector2Int cord) {
        if (!Grid.instance.SafeFromBomb(cord, new Vector2Int(x, z))) {
            //Debug.Log("Enemy Destroyed");
            Destroy(gameObject);
        }
    }
    [Server]
    private void FixedUpdate()
    {
        int i = rnd.Next(0, 4);
        Vector2Int next = new (move[i, 0], move[i, 1]);
        OnMovementPerformed(next);
    }
    [Server]
    private void OnMovementPerformed(Vector2Int temp) {
        Vector2Int newPos = new(x + temp.x, z + temp.y);
        if (!isMoving && Grid.instance.CanMove(newPos)) {
            Vector3 dest = Grid.instance.MatToWorldPos(newPos.x, newPos.y);
            float y_rotation = 0;
            if (temp.y == 1) {
                rotationAngles.y = y_rotation = 90;
            } else if (temp.y == -1) {
                rotationAngles.y = y_rotation = -90;
            } else if (temp.x == 1) {
                rotationAngles.y = y_rotation = 180;
            } else {
                rotationAngles.y = y_rotation = 0;
            }

            InstantiateFootSteps(y_rotation);
            StartCoroutine(Move(dest));
            x = newPos.x;
            z = newPos.y;
        }
    }
    [Server]
    private IEnumerator Move(Vector3 dest)
    {
        isMoving = true;
        //Vector3 end = transform.position + dir, start = transform.position;
        Vector3 end = dest, start = transform.position;
        end.y = 0.95f;
        float timeElapsed = 0;
        float timeToMove = 0.8f;


        while (timeElapsed < timeToMove)
        {
            transform.position = Vector3.Lerp(start, end, timeElapsed / timeToMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationAngles), timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;
    }
    [Server]
    private void InstantiateFootSteps(float y) {
        var obj = _poolFoots.GetObjectFromPool();
        obj.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        obj.transform.rotation = Quaternion.Euler(90, y, 0);

        FootStepScript step = obj.GetComponent<FootStepScript>();

        step.Initialize(_poolFoots);
    }
}
