using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    /* difficulty gauge on how fast the gamp ramps up to maximum speed
    With linesMax of 200, level n will reach peak difficulty after 200/n times clearing any number of lines */
    public int level; 
    public bool randomStart; //whether the game starts with n randomly selected blank squares (n as level number)

    public TMP_Text displayLevel;
    public TMP_Text displayScore;

    public Board board;

    public void Awake() {
        level = 1;
        randomStart = false;
    }

    public void Update() { //update display of score
        displayLevel.text = "Level: " + level;
        displayScore.text = "Score: " + board.score;
    }

    public void changeDifficulty() {
        level = (int)GameObject.Find("Slider").GetComponent<Slider>().value;
        displayLevel.text = "Level: " + level.ToString();
    }

    public void toggleRandom() {
        randomStart = GameObject.Find("Toggle").GetComponent<Toggle>().isOn;
    }

}
