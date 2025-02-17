using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroBehaviour : MonoBehaviour
{
    private string text;
    private string textWrote = "";
    private Stack<char> chars = new Stack<char>();
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private TextMeshProUGUI uiTextBuggedY, uiTextBuggedO;
    [SerializeField] private Animator anim;
    [SerializeField] private float timeBetweenTwoLetters;
    [SerializeField] private float timeBetweenTwoLines;
    [SerializeField] private float timeShow;
    private int letterIndex;
    public int sinnerNumber;

    private void Start()
    {
        GameData data = SaveSystem.Load();
        data.sinnerNB++;
        SaveSystem.Save(data);
        sinnerNumber = data.sinnerNB;
        text = "Sinner n°" + sinnerNumber + "\nSurvive as long as possible\nCollect khorteus and send it to the company\nRemember, your sacrifice is a good thing for this world";
        anim.SetTrigger("Show");
        StartCoroutine(AddLettre(text[0]));
    }

    private IEnumerator AddLettre(char letter)
    {
        if (textWrote.Length == 0)
        {
            chars.Push('<');
            chars.Push(letter);
            chars.Push('>');
            yield return new WaitForSeconds(timeBetweenTwoLetters);
        }
        else
        {

            if(letter == '\n')
            {
                chars.Push('\n');
                chars.Push('<');
                chars.Push('>');
                yield return new WaitForSeconds(timeBetweenTwoLines);
            }
            else
            {
                chars.Pop();
                chars.Push(letter);
                chars.Push('>');
                yield return new WaitForSeconds(timeBetweenTwoLetters);
            }
        }
        textWrote = "";
        List<char> reversedList = new();
        foreach (char letterInText in chars)
        {
            reversedList.Add(letterInText);
        }
        reversedList.Reverse();
        foreach(char newLetter in reversedList)
        {
            textWrote = textWrote + newLetter;
        }
        uiText.text = textWrote;
        uiTextBuggedY.text = uiText.text;
        uiTextBuggedO.text = uiText.text;
        letterIndex++;
        if (letterIndex < text.Length)
        {
            StartCoroutine(AddLettre(text[letterIndex]));
        }
    }
}
