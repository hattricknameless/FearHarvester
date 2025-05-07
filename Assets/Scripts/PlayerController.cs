using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public static PlayerController player;
    private TileManager tileManager;
    private MobManager mobManager;
    private TutorialManager tutorialManager;
    private AudioSource audioSource;
    public Vector2Int position;
    public Vector2Int forward;
    public bool inGrass;
    public bool controllable;
    [SerializeField] private List<GameObject> moveableTileObjects = new();
    [SerializeField] private float moveSpeed = 1;
    private LayerMask tileLayer;

    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private AudioClip walkAudio;

    private void Awake() {
        player = this;

        tileManager = TileManager.tileManager;
        mobManager = MobManager.mobManager;
        tutorialManager = TutorialManager.tutorialManager;
        audioSource = GetComponent<AudioSource>();
        tileLayer = LayerMask.GetMask("Tile");

        tileManager.OnTileManagerInitialized += Initialize;
        tileManager.OnGameEnd += GameEnd;
        tutorialManager.OnTutorialEnd += OnTutorialEnd;

        controllable = false;
    }
    
    private void Start() {
        
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && controllable) {
            audioSource.PlayOneShot(clickAudio);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer)) {
                GameObject destinationTile = hit.collider.gameObject;
                // Debug.Log($"Hit at {destinationTile.GetComponent<Tile>().tilePosition}");
                if (moveableTileObjects.Contains(destinationTile)) {
                    controllable = false;
                    mobManager.NextTurn();
                    StartCoroutine(MoveToAnimation(destinationTile));
                }
            }
        }
    }

    public void Initialize() {
        forward = new Vector2Int((int)transform.forward.normalized.x, (int)transform.forward.normalized.z);
        Vector3 worldPosition = gameObject.transform.position;
        position = (Vector2Int)tileManager.tilemap.WorldToCell(worldPosition);
        if (tileManager.gridLayout.TryGetValue(position, out GameObject tile)) {
            transform.position = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
        }
        else {
            Debug.Log("Can't find grid");
        }
        Debug.Log("PlayerController Initialization Complete");
    }

    private void OnTutorialEnd() {
        controllable = true;
        OnTurnEnd();
    }

    private void OnTurnEnd() {
        controllable = true;
        UpdateActiveTiles();
        UpdateInGrass();
    }

    private void UpdateActiveTiles() {
        moveableTileObjects = new();
        List<Vector2Int> tileList = new List<Vector2Int>{position + Vector2Int.up, position + Vector2Int.down, position + Vector2Int.left, position + Vector2Int.right};
        foreach (Vector2Int pos in tileList) {
            if (tileManager.gridLayout.TryGetValue(pos, out GameObject tile)) {
                moveableTileObjects.Add(tile);
                tile.GetComponent<Tile>().SetSelectionTile();
            }
        }
    }

    private void UpdateInGrass() {
        inGrass = tileManager.gridLayout[position].GetComponent<Tile>().isBrush;
    }

    private IEnumerator MoveToAnimation(GameObject destinationTile) {
        // Debug.Log($"Move to {destinationTile.GetComponent<Tile>().tilePosition}");
        audioSource.PlayOneShot(walkAudio);
        float mindistance = 0.01f;
        Vector3 targetPosition = new Vector3(destinationTile.transform.position.x, transform.position.y, destinationTile.transform.position.z);
        TurnAngle(destinationTile.GetComponent<Tile>());
        while (Vector3.Distance(transform.position, targetPosition) > mindistance) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        position = destinationTile.GetComponent<Tile>().tilePosition;
        OnTurnEnd();
    }

    private void TurnAngle(Tile destiationTile) {
        Dictionary<Vector2Int, float> directionAngles = new Dictionary<Vector2Int, float> {
            { Vector2Int.up, 0f },
            { Vector2Int.right, 90f },
            { Vector2Int.down, 180f },
            { Vector2Int.left, 270f }
        };
        Vector2Int destination = destiationTile.tilePosition - position;
        if (directionAngles.TryGetValue(destination, out float angle)) {
            gameObject.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
        else {
            Debug.Log("Angle turn doesn't work");
        }
    }

    private void GameEnd(bool result) {
        controllable = false;
    }
}
