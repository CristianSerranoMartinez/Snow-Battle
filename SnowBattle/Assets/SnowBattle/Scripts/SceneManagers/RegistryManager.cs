using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Unity.Editor;
using System;
using Firebase.Database;

public class RegistryManager : MonoBehaviour {

    [SerializeField]
    private InputField inputFieldPassword;
    [SerializeField]
    private InputField inputFieldEmail;
    [SerializeField]
    private InputField inputFieldNickName;
    [SerializeField]
    private InputField inputFieldPasswordAgain;
    [SerializeField]
    private Text textWarning;
    [SerializeField]
    private GameObject panelLoading;
    [SerializeField]
    private GameObject imageLoading;
    /*[SerializeField]
    private GameObject logInCanvas;
    [SerializeField]
    private GameObject selfCanvas;*/

    private bool ok;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                VariablesRealTime.auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    /*private void OnEnable()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                VariablesRealTime.auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }*/

    private void Update()
    {
        if (ok)
        {
            imageLoading.transform.Rotate(0, 0, 5);
        }
    }


    public void SignUp()
    {
        if (TestEmail.IsEmail(inputFieldEmail.text) && inputFieldEmail.text != "" && inputFieldPassword.text != "" && inputFieldEmail.text != " " && inputFieldEmail.text != " " && inputFieldNickName.text != "" && inputFieldNickName.text != " " && inputFieldPasswordAgain.text != "" && inputFieldPasswordAgain.text != " " && inputFieldPassword.text == inputFieldPasswordAgain.text)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                textWarning.text = "Error. Check internet connection!";
            }
            else
            {
                panelLoading.SetActive(true); ok = true;
                VariablesRealTime.auth.CreateUserWithEmailAndPasswordAsync(inputFieldEmail.text, inputFieldPassword.text).ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        panelLoading.SetActive(false); ok = false;
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                        AggregateException ex = task.Exception as AggregateException;
                        if (ex != null)
                        {
                            Firebase.FirebaseException fbEx = null;
                            foreach (Exception e in ex.InnerExceptions)
                            {
                                fbEx = e as Firebase.FirebaseException;
                                if (fbEx != null)
                                    break;
                            }

                            if (fbEx != null)
                            {
                                textWarning.color = Color.red;
                                textWarning.text = fbEx.Message;
                            }
                        }
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        panelLoading.SetActive(false); ok = false;
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        AggregateException ex = task.Exception as AggregateException;
                        if (ex != null)
                        {
                            Firebase.FirebaseException fbEx = null;
                            foreach (Exception e in ex.InnerExceptions)
                            {
                                fbEx = e as Firebase.FirebaseException;
                                if (fbEx != null)
                                    break;
                            }

                            if (fbEx != null)
                            {
                                textWarning.color = Color.red;
                                textWarning.text = fbEx.Message;
                            }
                        }
                        return;
                    }
                    if (task.IsCompleted)
                    {
                        textWarning.color = Color.green;

                        textWarning.text = "Count Created";

                        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://snowballs-98497.firebaseio.com/");

                        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

                        User user = new User
                        (
                            inputFieldNickName.text,

                            inputFieldEmail.text,
                            
                            inputFieldPassword.text,
                            
                            task.Result.UserId,
                            
                            1,
                            
                            0,
                            
                            0,
                            
                            100f
                        );

                        VariablesRealTime.eMail = inputFieldEmail.text;

                        VariablesRealTime.password = inputFieldPassword.text;

                        VariablesRealTime.newUser = task.Result;

                        string json = JsonUtility.ToJson(user);

                        reference.Child("users").Child(task.Result.UserId).SetRawJsonValueAsync(json).ContinueWith((task3) =>
                        {
                            if (task3.IsFaulted)
                            {
                                textWarning.text = task3.Exception.Message;

                                panelLoading.SetActive(false); ok = false;

                                return;
                            }
                            if (task3.IsCompleted)
                            {
                                SceneManager.LoadScene("Main");
                            }
                        });
                    }
                });
            }
        }
        else
            textWarning.text = "Please Check Your Email Or Password Or Your User Name";
        {
            if (inputFieldPasswordAgain.text != inputFieldPassword.text)
            {
                textWarning.color = Color.red;
                textWarning.text = "Your Password Does Not Match";
            }
        }
    }

    public void ButtonBack()
    {
        SceneManager.LoadScene("LogIn");
        /*logInCanvas.SetActive(true);
        selfCanvas.SetActive(false);*/
    }
}
