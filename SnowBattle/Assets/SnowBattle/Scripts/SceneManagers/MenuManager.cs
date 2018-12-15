using UnityEngine.SceneManagement;
using UnityEngine;
using Firebase.Database;
using Firebase;
using System.Collections;
using UnityEngine.UI;
using Firebase.Unity.Editor;

public class MenuManager : MonoBehaviour {
    [SerializeField]
    Text level;
    [SerializeField]
    Text lose;
    [SerializeField]
    Text won;
    [SerializeField]
    Text nickName;
    [SerializeField]
    Text moneyText;
    [SerializeField]
    private Text textWarning;
    [SerializeField]
    private GameObject selfCanvas;
    [SerializeField]
    private GameObject findGameCanvas;
    [SerializeField]
    private GameObject selectCharacterCanvas;
    [SerializeField]
    private GameObject selectMapCanvas;
    [SerializeField]
    private GameObject optionsCanvas;

    /*void Awake()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            level.text = "Error. Check internet connection!";
        }
        else
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                }
            });
        }
    }*/

    private void OnEnable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            level.text = "Error. Check internet connection!";
        }
        else
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                }
            });
        }
    }

    // Initialize the Firebase database:
    protected virtual void InitializeFirebase()
    {
        VariablesRealTime.auth.SignInWithEmailAndPasswordAsync(VariablesRealTime.eMail, VariablesRealTime.password).ContinueWith(task1 => {

            if (task1.IsCanceled)
            {
                textWarning.text = "SignInWithEmailAndPasswordAsync was canceled.";
                return;
            }
            if (task1.IsFaulted)
            {
                textWarning.text = "SignInWithEmailAndPasswordAsync encountered an error: " + task1.Exception;
                return;
            }

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://snowballs-98497.firebaseio.com/");

            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            FirebaseDatabase.DefaultInstance.GetReference("users/" + VariablesRealTime.newUser.UserId).GetValueAsync().ContinueWith(task2 => {

                if (task2.IsFaulted)
                {
                    textWarning.text = task2.Exception.ToString();
                }
                else if (task2.IsCompleted)
                {
                    DataSnapshot snapshot = task2.Result;
                    IDictionary dictUser = (IDictionary)snapshot.Value;
                    nickName.text = "" + dictUser["username"];
                    VariablesRealTime.nickName = nickName.text;
                   /* level.text = "Level " + dictUser["level"];
                    lose.text = "Losed\n" + dictUser["lose"];
                    won.text = "Won\n" + dictUser["won"];
                    moneyText.text = "$" + dictUser["money"];*/
                }
            });
        });
    }

    public void ButtonLogOut()
    {
        VariablesRealTime.auth.SignOut();

        SceneManager.LoadScene("LogIn");
    }

    void OnApplicationQuit()
    {
        VariablesRealTime.auth.SignOut();
    }

    public void ButtonFindGame()
    {
        findGameCanvas.SetActive(true);
        selfCanvas.SetActive(false);
        selectCharacterCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
       // SceneManager.LoadScene("FindGame");
    }
    public void ButtonCreateGame()
    {
        selectCharacterCanvas.SetActive(true);
        findGameCanvas.SetActive(false);
        selfCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        //SceneManager.LoadScene("SelectCharacter");
    }
    public void ButtonOptions()
    {
        // SceneManager.LoadScene("Options");
        optionsCanvas.SetActive(true);
        findGameCanvas.SetActive(false);
        selfCanvas.SetActive(false);
        selectCharacterCanvas.SetActive(false);
    }
}
