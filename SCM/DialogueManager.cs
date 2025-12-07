using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For scene management

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(3, 5)] public string dialogue;
        public string backgroundName; // Background to change
        public string characterName;  // Character to show/change expression
        public string expression;     // Character's expression

        public string objectName; // Object to move/teleport
        public Vector3 movePosition; // Target position
        public bool isTeleport; // If true, instantly teleport
        public float moveSpeed; // Movement speed (default 8)
    }

    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public SpriteRenderer backgroundRenderer;
    public List<CharacterObject> characterObjects;
    public Image fadePanel;  // UI Fade Panel
    public float textSpeed = 0.05f;
    public string nextSceneName; // Set this in the inspector for the next scene

    private bool isTyping = false;
    private bool skipTyping = false;
    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();

    public DialogueLine[] dialogueLines;

    void Start()
    {
        fadePanel.color = new Color(0, 0, 0, 1); // Start with a black screen
        StartCoroutine(FadeFromBlack());

        StartDialogue(dialogueLines);
    }

    public void StartDialogue(DialogueLine[] dialogueLines)
    {
        dialogueQueue.Clear();
        foreach (DialogueLine line in dialogueLines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextLine();
        }
    }

    public void DisplayNextLine()
    {
        if (isTyping)
        {
            skipTyping = true;
            return;
        }

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueQueue.Dequeue();
        speakerNameText.text = currentLine.speakerName;

        if (!string.IsNullOrEmpty(currentLine.backgroundName))
        {
            StartCoroutine(FadeBackgroundTransition(currentLine.backgroundName, currentLine.dialogue));
        }
        else
        {
            StartCoroutine(TypeText(currentLine.dialogue));
        }

        if (!string.IsNullOrEmpty(currentLine.characterName))
        {
            ShowCharacter(currentLine.characterName, currentLine.expression);
        }

        if (!string.IsNullOrEmpty(currentLine.objectName))
        {
            MoveOrTeleportObject(currentLine.objectName, currentLine.movePosition, currentLine.isTeleport, currentLine.moveSpeed);
        }
    }

    private IEnumerator TypeText(string dialogue)
    {
        isTyping = true;
        skipTyping = false;
        dialogueText.text = "";

        foreach (char letter in dialogue.ToCharArray())
        {
            if (skipTyping)
            {
                dialogueText.text = dialogue;
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        StartCoroutine(TransitionToNextScene()); // Start scene transition
    }

    private IEnumerator FadeBackgroundTransition(string backgroundName, string dialogue)
    {
        yield return StartCoroutine(FadeToBlack());

        Sprite newBackground = Resources.Load<Sprite>("Backgrounds/" + backgroundName);
        if (newBackground != null)
        {
            backgroundRenderer.sprite = newBackground;
        }
        else
        {
            Debug.LogWarning("Background not found: " + backgroundName);
        }

        yield return StartCoroutine(FadeFromBlack());

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(TypeText(dialogue));
    }

    private IEnumerator FadeToBlack()
    {
        for (float t = 0; t <= 1; t += Time.deltaTime * 2)
        {
            fadePanel.color = new Color(0, 0, 0, t);
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeFromBlack()
    {
        for (float t = 1; t >= 0; t -= Time.deltaTime * 2)
        {
            fadePanel.color = new Color(0, 0, 0, t);
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 0);
    }

    void ShowCharacter(string characterName, string expression)
    {
        foreach (CharacterObject character in characterObjects)
        {
            if (character.characterName == characterName)
            {
                character.gameObject.SetActive(true);
                character.ChangeExpression(expression);
                return;
            }
        }
    }

    void MoveOrTeleportObject(string objectName, Vector3 targetPosition, bool isTeleport, float speed)
    {
        GameObject obj = GameObject.Find(objectName);

        if (obj == null)
        {
            Debug.LogWarning("Object not found: " + objectName);
            return;
        }

        if (isTeleport)
        {
            obj.transform.position = targetPosition;
        }
        else
        {
            StartCoroutine(MoveObjectSmooth(obj, targetPosition, speed));
        }
    }

    IEnumerator MoveObjectSmooth(GameObject obj, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator TransitionToNextScene()
    {
        yield return StartCoroutine(FadeToBlack()); // Fade to black before scene change
        SceneManager.LoadScene(nextSceneName); // Load next scene
    }
}
