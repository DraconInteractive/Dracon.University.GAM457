using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController ui;
    public Button toggleButton;
    public Text debugText;

    Enemy target;

    private void Awake()
    {
        ui = this;
    }
    // Use this for initialization
    void Start () {
        toggleButton.onClick.AddListener(() => ToggleDebugPanel());
        StartCoroutine(UpdateUI());
	}
	
	// Update is called once per frame
	void ToggleDebugPanel () {
        debugText.transform.parent.gameObject.SetActive(!debugText.transform.parent.gameObject.activeSelf);
	}

    public void AssignDebugTarget (Enemy e)
    {
        target = e;
        UpdateUI();
    }

    IEnumerator UpdateUI ()
    {
        while (true)
        {
            if (target != null)
            {
                string s = "";
                foreach (KeyValuePair<Character, float> pair in target.detectionDictionary)
                {
                    s += pair.Key.name + ": " + pair.Value.ToString() + " ";
                }
                debugText.text = s;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }
}
