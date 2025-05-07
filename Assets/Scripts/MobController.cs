using System.Collections;
using UnityEngine;

public class MobController : MonoBehaviour {
    
    private TileManager tileManager;
    private MobManager mobManager;
    private TutorialManager tutorialManager;
    private Light mobLight;

    public Vector2Int position;
    public Vector2Int forward;
    public int visionRange;
    public bool inGrass;

    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float rotationTime = 0.625f;

    private const string PLAYER_TAG = "Player";

    private void Awake() {
        tileManager = TileManager.tileManager;
        mobManager = MobManager.mobManager;
        tutorialManager = TutorialManager.tutorialManager;
        mobLight = GetComponentInChildren<Light>();

        tileManager.OnTileManagerInitialized += Initialize;
        mobManager.OnNextTurn += Forward;
        tutorialManager.OnTutorialEnd += OnTurnEnd;
    }

    private void Start() {
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
        Debug.Log("MobController Initialization Complete");
    }
    
    public void Forward() {
        Vector2Int nextStep = position + forward;
        if (tileManager.gridLayout.TryGetValue(nextStep, out GameObject tile)) {
            //move to tile position
            position = nextStep;
            StartCoroutine(MoveToAnimation(tile));
        }
        else {
            //turn around
            forward *= -1;
            StartCoroutine(TurnAroundAnimation(180));
        }
    }

    private IEnumerator MoveToAnimation(GameObject tile) {
        float mindistance = 0.01f;
        Vector3 targetPosition = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
        while (Vector3.Distance(transform.position, targetPosition) > mindistance) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        OnTurnEnd();
    }

    private IEnumerator TurnAroundAnimation(float angle) {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);

        float elapsedTime = 0.01f;
        while (elapsedTime < rotationTime) {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;

            if (Quaternion.Angle(transform.rotation, targetRotation) <= 0.1) {
                break;
            }
            yield return null;
        }
        transform.rotation = targetRotation;
        OnTurnEnd();
    }

    private void OnTurnEnd() {
        UpdateInGrass();
        if (visionRange == 1) {
            tileManager.isPlayerSeen(position);
        }
        tileManager.gridLayout[position].GetComponent<Tile>().SetExclamationTile();
    }
    
    private void UpdateInGrass() {
        inGrass = tileManager.gridLayout[position].GetComponent<Tile>().isBrush;
        if (inGrass) {visionRange = 0;}
        else {visionRange = 1;}
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag(PLAYER_TAG)) {
            Debug.Log("Player killed the villager");
            mobManager.DestroyMob(this);
            StopAllCoroutines();

            mobManager.OnNextTurn -= Forward;
        }
    }
}
