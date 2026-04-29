using UnityEngine;

public class AirHockeyGameManager : MonoBehaviour
{
    public static AirHockeyGameManager Instance;

    public int leftScore = 0;
    public int rightScore = 0;

    public float puckMinSpeed = 6f;
    public float puckMaxSpeed = 12f;

    public bool matchOver = false;
    public string winnerMessage = "";

    private GameObject puck;
    private Rigidbody puckRb;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterPuck(GameObject puckObject)
    {
        puck = puckObject;
        puckRb = puck.GetComponent<Rigidbody>();
    }

    public void ScoreGoal(bool leftGoal)
    {
        if (matchOver) return;

        if (leftGoal)
        {
            rightScore++;
            Debug.Log("Right Team Scores! Score: Left " + leftScore + " - Right " + rightScore);
        }
        else
        {
            leftScore++;
            Debug.Log("Left Team Scores! Score: Left " + leftScore + " - Right " + rightScore);
        }

        if (leftScore >= 7)
        {
            EndMatch("LEFT TEAM WINS!");
            return;
        }

        if (rightScore >= 7)
        {
            EndMatch("RIGHT TEAM WINS!");
            return;
        }

        ResetPuck(leftGoal ? -1 : 1);
    }

    void EndMatch(string message)
    {
        matchOver = true;
        winnerMessage = message;

        if (puckRb != null)
        {
            puckRb.linearVelocity = Vector3.zero;
            puckRb.angularVelocity = Vector3.zero;
        }

        Debug.Log(message);
    }

    public void ResetFullMatch()
    {
        leftScore = 0;
        rightScore = 0;
        matchOver = false;
        winnerMessage = "";

        ResetPuck(Random.value > 0.5f ? 1 : -1);
    }

    public void ResetPuck(int serveDirection)
    {
        if (puck == null || puckRb == null) return;

        puck.transform.position = new Vector3(0f, 0.35f, 0f);
        puckRb.linearVelocity = Vector3.zero;
        puckRb.angularVelocity = Vector3.zero;

        ServePuck(serveDirection);
    }

    public void ServePuck(int direction)
    {
        if (puckRb == null || matchOver) return;

        float zDirection = Random.Range(-0.45f, 0.45f);
        Vector3 forceDirection = new Vector3(direction, 0f, zDirection).normalized;

        puckRb.AddForce(forceDirection * 10f, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (puckRb == null || matchOver) return;

        Vector3 velocity = puckRb.linearVelocity;
        velocity.y = 0f;

        if (velocity.magnitude < 0.1f) return;

        if (velocity.magnitude < puckMinSpeed)
        {
            puckRb.linearVelocity = velocity.normalized * puckMinSpeed;
        }
        else if (velocity.magnitude > puckMaxSpeed)
        {
            puckRb.linearVelocity = velocity.normalized * puckMaxSpeed;
        }
    }
}