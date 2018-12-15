using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Firebase;
using UnityEngine.Audio;

public class LoginManager : MonoBehaviour {

    [SerializeField]
    private InputField inputFieldPassword;
    [SerializeField]
    private InputField inputFieldEmail;
    [SerializeField]
    private Text textWarning;
    [SerializeField]
    private GameObject panelLoading;
    [SerializeField]
    private GameObject imageLoading;
    [SerializeField]
    AudioMixer audioMixer;
    /*[SerializeField]
    private GameObject menuCanvas;
    [SerializeField]
    private GameObject registryCanvas;
    [SerializeField]
    private GameObject selfCanvas;*/

    private bool ok;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;


        ConfigurationData cd = SaveSystemFile.LoadConfiguration();

        if (cd != null)
        {
            audioMixer.SetFloat("Music", cd.musicVolume);

            audioMixer.SetFloat("Effects", cd.effectsVolume);
        }
    }

   /* private void OnEnable()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;


        ConfigurationData cd = SaveSystemFile.LoadConfiguration();

        if (cd != null)
        {
            audioMixer.SetFloat("Music", cd.musicVolume);

            audioMixer.SetFloat("Effects", cd.effectsVolume);
        }
    }*/

    private void Update()
    {
        if (ok)
        {
            imageLoading.transform.Rotate(0, 0, 5);
        }
    }
    public void ButtonLogin()
    {
        if (TestEmail.IsEmail(inputFieldEmail.text) && inputFieldEmail.text != "" && inputFieldPassword.text != "" && inputFieldEmail.text != " " && inputFieldPassword.text != " ")
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                textWarning.text = "Error. Check internet connection!";
            }
            else
            {
                panelLoading.SetActive(true); ok = true;
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    if (task.Result == DependencyStatus.Available)
                    {
                        InitializeFirebase();
                    }
                    else
                    {
                        Debug.LogError(
                          "Could not resolve all Firebase dependencies: " + task.Result);
                    }
                });
            }
        }
        else
        {
            textWarning.color = Color.red;
            textWarning.text = "Please Check Your Email Or Password";
        }
    }

    protected virtual void InitializeFirebase()
    {

        VariablesRealTime.auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        VariablesRealTime.auth.SignInWithEmailAndPasswordAsync(inputFieldEmail.text, inputFieldPassword.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                AggregateException ex = task.Exception as AggregateException;
                if (ex != null)
                {
                    panelLoading.SetActive(false); ok = false;
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
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception.Message);
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
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            VariablesRealTime.eMail = inputFieldEmail.text; 
            VariablesRealTime.password = inputFieldPassword.text;
            VariablesRealTime.newUser = task.Result;

            Debug.Log(VariablesRealTime.newUser);
            Screen.orientation = ScreenOrientation.Landscape;
            SceneManager.LoadScene("Main");
            /*menuCanvas.SetActive(true);
            selfCanvas.SetActive(false);*/
        });
    }

    public void ButtonResgistry()
    {
        SceneManager.LoadScene("Registry");
        //registryCanvas.SetActive(true);
        //selfCanvas.SetActive(false);
    }
}
