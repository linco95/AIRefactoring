using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(xorcNet))]
public class ANNGUI : MonoBehaviour {

    private List<KeyValuePair<List<double>, List<double>>> values;
    public Text resultText;
    public GameObject circleBar;
    public GameObject crossBar;
    public GameObject filledImage;
    public GameObject emptyImage;
    //public float offset = 100.0f;
    private xorcNet xcn;
    private int currentIndex = 0;
    List<GameObject> cells = new List<GameObject>();

    private void Awake() {
        xcn = GetComponent<xorcNet>();
    }

    // Use this for initialization
    void Start() {
        if (xcn == null || resultText == null) { return; }
         values = xcn.results;
        if (values.Count == 0) return;
        loadTest();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            currentIndex = (int)Mathf.Max(--currentIndex, 0.0f);
            loadTest();
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            currentIndex = (int)Mathf.Min(++currentIndex, values.Count - 1);
            loadTest();
        } else if (Input.GetKeyUp(KeyCode.Space)) {
            SceneManager.LoadScene(1);
        }
    }

    private  void loadTest() {
        cells.ForEach(go => Destroy(go));
        int index = currentIndex;
        var Values = this.values;
        var vals = Values[index].Key;
        var tars = Values[index].Value;

        int width = (int)Mathf.Sqrt(vals.Count);
        for (int i = 0; i < vals.Count; i++) {
            cells.Add(Instantiate(vals[i] == 1 ? filledImage : emptyImage, Camera.main.WorldToScreenPoint(new Vector3(i % width - 1.5f, -i / width + 4, 0)), Quaternion.identity, gameObject.transform));
        }
        string outputstr = "";
        outputstr += "\ncross: " + tars[0] + "\ncircle: " + tars[1] + "\n";
        circleBar.transform.localScale = new Vector3(1, (float)tars[0], 1);
        crossBar.transform.localScale = new Vector3(1, (float)tars[1], 1);
        outputstr += (tars[0] > 0.5f ? "It is a circle!!!" : "") + '\n';
        outputstr += (vals[1] > 0.5f ? "It is a cross!!!" : "") + '\n';
        outputstr += (tars[0]> tars[1] ? "More circle than cross." : "More cross than circle") + '\n';
        resultText.text = outputstr;
        resultText.text = "";
    }
}
