using UnityEngine;
using System.Collections;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    public  float  delay = 0.1f;
    public  string FullText;
    private string currentText = "";

    private void OnEnable()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < FullText.Length + 1; i++)
        {
            currentText = FullText.Substring(0, i);
            this.GetComponent<TextMeshProUGUI>().text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
}
