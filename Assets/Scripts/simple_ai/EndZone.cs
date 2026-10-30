using UnityEngine;

public class EndZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // ONLY trigger if the object touching it is the Player
        if (other.gameObject.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            CollisionWithPlayer player = Object.FindAnyObjectByType<CollisionWithPlayer>();

            if (player != null)
            {
                int currentScore = player.GetCurrentScore();
                int targetScore = player.GetTargetScore();

                Debug.Log("End Zone entered by Player | Score: " + currentScore + "/" + targetScore);

                if (player.HasReachedTargetScore())
                {
                    Debug.Log("CONGRATULATIONS - WIN CONDITION MET!");
                    print("Congratulations!");

                    if (player.messageText != null)
                    {
                        player.messageText.text = "Congratulations!";
                    }

                    Time.timeScale = 0f; // Freeze game
                }
                else
                {
                    Debug.LogWarning("Reached end zone, but score is too low (" + currentScore + "/" + targetScore + ").");
                }
            }
        }
    }
}