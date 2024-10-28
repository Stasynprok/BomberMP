using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public delegate void DestroyBomb(Vector2Int cord);
    public static event DestroyBomb OnDestroyBomb;

    public static void OnDestroyBombInvoke(Vector2Int cord) {
        OnDestroyBomb?.Invoke(cord);
    }

    public delegate void DestroyCharacters();
    public static event DestroyCharacters OnDestroyEnemy;
    public static event DestroyCharacters OnDestroyBomberman;
    public static event DestroyCharacters OnDestroyTile;

    public static void OnDestroyEnemyInvoke() {
        OnDestroyEnemy?.Invoke();
    }

    public static void OnDestroyBombermanInvoke() {
        OnDestroyBomberman?.Invoke();
    }

    public static void OnDestroyTileInvoke() {
        OnDestroyTile?.Invoke();
    }

    public delegate void CollectedCollectible();
    public static event CollectedCollectible OnPowerUpCollected;
    public static event CollectedCollectible OnGateEnter;

    public static void OnPowerUpCollectedInvoke() {
        OnPowerUpCollected?.Invoke();
    }

    public static void OnGateEnterInvoke() {
        OnGateEnter?.Invoke();
    }

    public delegate void InstantiateBombRequest();
    public static event InstantiateBombRequest OnRequest;

    public static void OnRequestInvoke() {
        OnRequest?.Invoke();
    }
}
