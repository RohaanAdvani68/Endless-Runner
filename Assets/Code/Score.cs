using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Scoreboard
/// </summary>
public class Score : MonoBehaviour {
    /// <summary>
    /// Called when the score increases
    /// </summary>
    public event Action ScoreUp = delegate {};

    /// <summary>
    /// Current score
    /// </summary>
    private int score;
    /// <summary>
    /// UnityEngine.UI.Text object to update
    /// </summary>
    private Text myText;

    /// <summary>
    /// Initialize: find components and subscribe to events
    /// </summary>
	internal void Start () {
        var runner = FindObjectOfType<Runner>();
        runner.LandedOnPlatform += IncrementScore;
        runner.FellIntoTheVoid += GameOverMessage;

        score = 0;
        myText = gameObject.GetComponent<Text>();
        UpdateText();
	}

    /// <summary>
    /// The score has increased.
    /// </summary>
    void IncrementScore(){
        score += 100;
        if (score % 500 == 0) {
            ScoreUp();
        }
        UpdateText();
    }

    /// <summary>
    /// Display the Game Over message
    /// </summary>
    void GameOverMessage(){
        myText.text = "Game Over!";
    }

    /// <summary>
    /// Update the Text object with the new score.
    /// </summary>
    void UpdateText(){
        myText.text = string.Format("Score: {0}", score);
    }
}
