using UnityEngine;

public class ScoreAward : MonoBehaviour
{
    [SerializeField] private int scoreAmount = 100;

    public int ScoreAmount => scoreAmount;
}