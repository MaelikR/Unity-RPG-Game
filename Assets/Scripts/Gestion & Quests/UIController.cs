using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public Text questMessageText;
    public Text questCompletionMessageText;
    public Text rewardMessageText;
    public Text endGameMessageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowQuestMessage(string message)
    {
        questMessageText.text = message;
    }

    public void ShowQuestCompletionMessage(string message)
    {
        questCompletionMessageText.text = message;
    }

    public void ShowRewardMessage(string message)
    {
        rewardMessageText.text = message;
    }

    public void ShowEndGameScreen(string message)
    {
        endGameMessageText.text = message;
    }
}
