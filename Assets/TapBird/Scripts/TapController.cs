using UnityEngine;

// 플레이어 컨트롤
[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 250;
    public float tiltSmooth = 2;
    public Vector3 startPos;
    public AudioSource tapSound;
    public AudioSource scoreSound;
    public AudioSource dieSound;

    Rigidbody2D rigidBody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;
    TrailRenderer trail;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 180, 70);
        forwardRotation = Quaternion.Euler(0, 180, -45);
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    void Update()
    {
        if (game.GameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            rigidBody.velocity = Vector2.zero;
            transform.rotation = forwardRotation;
            rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
            tapSound.Play();
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnTriggerEnter2D :: " + col.gameObject.tag);

        if (col.gameObject.tag == "ScoreZone")
        {
            OnPlayerScored();
            scoreSound.Play();
        }
        if (col.gameObject.tag == "DeadZone")
        {
            rigidBody.simulated = false;
            OnPlayerDied();
            dieSound.Play();
        }
    }

}
