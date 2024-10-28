using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;


public class Grid: NetworkBehaviour
{
    /// <summary>
    /// Tile Info
    /// 1: hard tile, 2: soft tile
    /// </summary>
    [SyncVar]
    private float originX, originY, originZ;
    public GameObject hardTile;
    public GameObject softTile;
    public GameObject tileContainer;
    public GameObject bomberman;
    public GameObject enemy;
    public GameObject bomb;
    public GameObject door;

    //public Text score, timer;
    public TextMeshProUGUI score, timer;
    public GameObject menu;

    private HashSet<string> bs;
    private Queue<int> X, Y;
    [SyncVar]
    private int factor;
    [SyncVar]
    private int enemyDestroyed, numberOfTotalEnemies;
    [SyncVar]
    private GameObject instantiatedGate;
    [SyncVar]
    private float _timer, _score;
    [SyncVar]
    private bool toggleMenu, gameOver;
    
    public int[,] mat;
    [SyncVar]
    private int numberOfSoftTile;
    System.Random rnd;

    public static Grid instance;

    private void Start()
    {
        mat = new int[13, 13];
        X = new Queue<int>();
        Y = new Queue<int>();

        for (int j = 0; j < 13; j++)
        {
            mat[0, j] = mat[12, j] = 1;
            mat[j, 0] = mat[j, 12] = 1;
        }

        for (int i = 2; i < 12; i += 2)
        {
            for (int j = 2; j < 12; j += 2)
            {
                mat[i, j] = 1;
            }
        }

        OnAwake();
        if (instance == null)
        { // Ёкземпл€р менеджера был найден
            instance = this; // «адаем ссылку на экземпл€р объекта
            
        }
        else if (instance == this)
        { // Ёкземпл€р объекта уже существует на сцене
            Destroy(gameObject); // ”дал€ем объект
        }
    }

    [Server]
    private void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
        rnd = new System.Random();
        originX = -7.5f;
        originY = 0.5f;
        originZ = 7.5f;
        factor = 1;
        enemyDestroyed = 0;
        toggleMenu = false;
        gameOver = false;

        timer.text = "Timer: 0";
        score.text = "Score: 0";
        _timer = _score = 0;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (mat[i, j] == 1)
                {
                    //GameObject obj = NetworkManager.singleton.spawnPrefabs[5]; //hardTile
                    //InstantiateTile(i, j, obj);
                    GameObject pref = GetObjectFromNetworkManager(hardTile);
                    GameObject obj = InstantiateTile(i, j, pref, 0.95f);
                    NetworkServer.Spawn(obj);
                    //InstantiateTile(i, j, hardTile);
                }
            }
        }

        // Instantiating soft tiles here
        numberOfSoftTile = rnd.Next(25, 36);
        while (numberOfSoftTile > 0)
        {
            if (InstantiateSoftTile())
            {
                numberOfSoftTile--;
            }
        }

        // Instantiating bomberman here
        //InstantiateTile(1, 1, bomberman, 0);

        // Instantiating enemies here
        int numberOfEnemy = rnd.Next(5, 10);
        //int numberOfEnemy = rnd.Next(1, 2);
        numberOfTotalEnemies = numberOfEnemy;
        while (numberOfEnemy > 0)
        {
            if (InstantiateEnemy())
            {
                numberOfEnemy--;
            }
        }

        // Instantiating collectible here
        int numberOfCollectible = rnd.Next(2, 6);
        bs = new HashSet<string>();
        while (numberOfCollectible > 0)
        {
            if (InstantiateCollectible())
            {
                numberOfCollectible--;
            }
        }

        // Instantiating door here
        bool doorInstantiated = false;
        while (!doorInstantiated)
        {
            if (InstantiateDoor())
            {
                doorInstantiated = true;
            }
        }
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        timer.text = "Timer: " + Mathf.RoundToInt(_timer);
        score.text = "Score: " + Mathf.RoundToInt(_score);
    }

    private void Update()
    {
        menu.SetActive(toggleMenu || gameOver);
    }

    private GameObject InstantiateTile (int x, int z, GameObject tile, float _y = 0.5f) {
        Vector3 pos = MatToWorldPos(x, z);
        pos.y = _y;

        //var obj = Instantiate();

        var _tile = Instantiate(tile, pos, tile.transform.rotation);
        _tile.transform.parent = tileContainer.transform;
        return _tile;
    }
    public GameObject InitBomberman (int x, int z, GameObject tile, float _y = 0.5f) {
        Vector3 pos = MatToWorldPos(x, z);
        pos.y = _y;
        //var _tile = Instantiate(tile, pos, tile.transform.rotation);
        tile.transform.position = pos;
        tile.transform.parent = tileContainer.transform;
        return tile;
    }

    private bool InstantiateDoor() {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);

        if (mat[x, z] != 2 || bs.Contains(x + "," + z)) {
            return false;
        }

        //GameObject obj = NetworkManager.singleton.spawnPrefabs[2]; //door
        //instantiatedGate = InstantiateTile(x, z, obj, 0);

        GameObject pref = GetObjectFromNetworkManager(door);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);
        //instantiatedGate = InstantiateTile(x, z, door, 0);
        return true;
    }

    private bool InstantiateCollectible () {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);
        if (mat[x, z] != 2) {
            return false;
        }

        //GameObject obj = NetworkManager.singleton.spawnPrefabs[0]; //bomb
        //InstantiateTile(x, z, obj);
        GameObject pref = GetObjectFromNetworkManager(bomb);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);
        //InstantiateTile(x, z, bomb);
        bs.Add(x + "," + z);
        return true;
    }

    private bool InstantiateEnemy () {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);
        if (mat[x, z] == 1 || mat[x, z] == 2 || (x >= 1 && x <= 5 && z >= 1 && z <= 5)) {
            return false;
        }

        X.Enqueue(x); Y.Enqueue(z);
        GameObject pref = GetObjectFromNetworkManager(enemy); //enemy
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);
        //InstantiateTile(x, z, enemy, 0.95f);
        return true;
    }

    private bool InstantiateSoftTile () {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);
        if (mat[x, z] == 1 || mat[x, z] == 2 || (x >= 1 && x <= 2 && z >= 1 && z <= 2)) {
            return false;
        }

        //GameObject obj = NetworkManager.singleton.spawnPrefabs[8]; //softTile
        //InstantiateTile(x, z, obj);

        GameObject pref = GetObjectFromNetworkManager(softTile);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);

        //InstantiateTile(x, z, softTile);
        mat[x, z] = 2;
        return true;
    }

    public Vector3 MatToWorldPos(int x, int z) {
        float _x = z;
        float _z = -x;

        return new Vector3(_x + originX, originY, _z + originZ);
    }

    public Vector2Int PosToMat(Vector3 pos) {
        Vector2Int p = new (0, 0);

        float x = pos.x - originX;
        float z = pos.z - originZ;
        p.x = -(int)z;
        p.y = (int)x;
        return p;
    }

    public bool CanMove(Vector2Int pos) {
        return pos.x >= 0 && pos.x < 13 && pos.y >= 0 && pos.y < 13 && mat[pos.x, pos.y] != 1 && mat[pos.x, pos.y] != 2;
    }

    public Vector2Int GetCoordinate() {
        Vector2Int cord = Vector2Int.zero;
        if (X.Count > 0 && Y.Count > 0) {
            cord.x = X.Dequeue();
            cord.y = Y.Dequeue();
        }

        //Debug.Log($"cord is {cord}");

        return cord;
    }

    public bool SafeFromBomb(Vector2Int bomb, Vector2Int obj) {
        Vector2Int up = new(-1, 0);
        Vector2Int down = new(1, 0);
        Vector2Int left = new(0, -1);
        Vector2Int right = new(0, 1);

        Vector2Int pos = bomb;
        for (int i = 0; i < factor; i++) {
            pos += up;
            if (pos == obj)
            {
                return false;
            } else if (pos.x < 1 || mat[pos.x, pos.y] == 1) {
                break;
            }
        }

        pos = bomb;
        for (int i = 0; i < factor; i++) {
            pos += down;
            if (pos == obj) {
                return false;
            } else if (pos.x > 11 || mat[pos.x, pos.y] == 1) {
                break;
            }
        }

        pos = bomb;
        for (int i = 0; i < factor; i++) {
            pos += left;
            if (pos == obj) {
                return false;
            } else if (pos.y < 1 || mat[pos.x, pos.y] == 1) {
                break;
            }
        }

        pos = bomb;
        for (int i = 0; i < factor; i++) {
            pos += right;
            if (pos == obj) {
                return false;
            } else if (pos.y > 11 || mat[pos.x, pos.y] == 1) {
                break;
            }
        }

        return obj != bomb;
    }

    private void OnEnable()
    {
        GameEvents.OnGateEnter += GateHandler;
        GameEvents.OnDestroyEnemy += EnemyHandler;
        GameEvents.OnDestroyTile += TileHandler;
        GameEvents.OnPowerUpCollected += CollectionHandler;
        GameEvents.OnDestroyBomberman += GateHandler;
    }

    private void OnDisable()
    {
        GameEvents.OnGateEnter -= GateHandler;
        GameEvents.OnDestroyEnemy -= EnemyHandler;
        GameEvents.OnDestroyTile -= TileHandler;
        GameEvents.OnPowerUpCollected -= CollectionHandler;
        GameEvents.OnDestroyBomberman -= GateHandler;
    }

    private void GateHandler() {
        gameOver = true;
    }

    private void EnemyHandler() {
        enemyDestroyed++;
        _score += 200;
        if (enemyDestroyed == numberOfTotalEnemies)
        {
            instantiatedGate.GetComponent<BoxCollider>().enabled = true;
        }
    }

    private void TileHandler() {
        _score += 50;
    }

    private void CollectionHandler() {
        _score += 100;
    }

    public void ToggleMenuHandler() {
        toggleMenu = !toggleMenu;
    }

    public void RestartGameHandler() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuHandler() {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void InstantiateBombRequestSend() {
        if (gameOver) {
            return;
        }

        GameEvents.OnRequestInvoke();
    }

    private GameObject GetObjectFromNetworkManager(GameObject gameObject)
    {
        for (int i = 0; i < NetworkManager.singleton.spawnPrefabs.Count; i++)
        {
            if (NetworkManager.singleton.spawnPrefabs[i] == gameObject)
            {
                return NetworkManager.singleton.spawnPrefabs[i];
            }
        }
        return null;
    }
}
