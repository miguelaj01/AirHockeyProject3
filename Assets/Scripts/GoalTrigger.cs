using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public bool isLeftGoal;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Puck")
        {
            AirHockeyGameManager.Instance.ScoreGoal(isLeftGoal);
        }
    }
}
