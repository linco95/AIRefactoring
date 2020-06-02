using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
    public TextMesh numberText;
    public int number;

    public void Start() {
        numberText.text = number.ToString();
    }
}
