using UnityEngine;

public class BoardChecker : MonoBehaviour
{
    [SerializeField] private Board2 board;
    [SerializeField] private float timeToCheck = 0.5f;

    private float timeSinceGemPlaced = 0f;
    private bool shouldCheck = false;


    private void OnEnable()
    {
        Gem2.GemPlaced += OnGemPlaced;
    }

    private void OnDisable()
    {
        Gem2.GemPlaced -= OnGemPlaced;
    }

    private void OnGemPlaced(object sender, GemPlacedEventArgs ev)
    {
        timeSinceGemPlaced = 0f;
        shouldCheck = true;
    }

    private void CheckBoard()
    {
        Debug.Log("*** Check board now! ***");
    }

    void Update()
    {
        timeSinceGemPlaced += Time.deltaTime;

        if (shouldCheck && timeSinceGemPlaced > timeToCheck)
        {
            shouldCheck = false;
            CheckBoard();
        }
    }


}
