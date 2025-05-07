using UnityEngine;

public class Tile : MonoBehaviour {

    private MobManager mobManager;
    public Vector2Int tilePosition;
    public bool isBrush;
    public bool isLuminated;
    public bool isWalkable = false;

    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject exclamationTile;
    [SerializeField] private GameObject selectionTile;

    private LayerMask playerLayer;
    private const string GRASS_TAG = "Grass";
    private const string PLAYER_TAG = "Player";

    private void Awake() {
        mobManager = MobManager.mobManager;
        playerLayer = LayerMask.GetMask(PLAYER_TAG);

        mobManager.OnNextTurn += ResetVisualTile;

        isBrush = isGrassCheck();
    }

    private void Start() {

    }

    private bool isGrassCheck() {
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 2)) {
            if (hit.collider.CompareTag(GRASS_TAG)) {
                grassTile = hit.collider.gameObject;
                return true;
            }
        }
        return false;
    }

    public bool isPlayerCheck() {
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 2, playerLayer)) {
            if (hit.collider.CompareTag(PLAYER_TAG)) {
                return true;
            }
        }
        return false;
    }
    
    private void ResetVisualTile() {
        exclamationTile.SetActive(false);
        selectionTile.SetActive(false);
    }

    public void SetSelectionTile() {selectionTile.SetActive(true);}
    public void SetExclamationTile() {exclamationTile.SetActive(true);}
}