using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase; 
using Firebase.Auth; 
public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;

    //Login variables
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;
    public Text emailLoginWarningText;
    public Text passWordLoginWarningText;
    public GameObject LoginPanelUI; 

    //SIgnip variables
    [Header("Signup")]
    public InputField emailSignupField;
    public InputField passwordSignupField;
    public InputField rePasswordSignupField;
    public Text emailSignUpWarningText;
    public Text passWordSignUpWarningText;
    public GameObject SignUpPanelUI; 

    [Header("ResetPawssword")]
    public InputField resetEmailField;
    public Text resetEmailWarningText;
    public GameObject ResetPwPanelUI; 

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the Signup button
    public void SignupButton()
    {
        //Call the Signup coroutine passing the email, password, and username
        StartCoroutine(Signup(emailSignupField.text, passwordSignupField.text, usernameSignupField.text));
    }

    public void SignupPanel() 
    {
        SignUpPanelUI.SetActive(true);
        LoginPanelUI.SetActive(false);
    }

    public void LoginPanel() 
    {
        SignUpPanelUI.SetActive(false);
        LoginPanelUI.SetActive(true);
    }

    public void ResetPwPanel() 
    {

    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to Signup task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            string message2 = "";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message2 = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message2 = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            emailLoginWarningText.text = message;
            passWordLoginWarningText.text = message2;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            emailLoginWarningText.text = "";
            //confirmLoginText.text = "Logged In";
        }
    }

    private IEnumerator Signup(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            emailSignUpWarningText.text = "Missing Username";
        }
        else if(passwordSignupField.text != passwordSignupVerifyField.text)
        {
            //If the password does not match show a warning
            passWordSignUpWarningText.text = "Password Does Not Match!";
        }
        else 
        {
            //Call the Firebase auth signin function passing the email and password
            var SignupTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => SignupTask.IsCompleted);

            if (SignupTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to Signup task with {SignupTask.Exception}");
                FirebaseException firebaseEx = SignupTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Signup Failed!";
                string message2 = "";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message2 = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message2 = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                emailSignUpWarningText.text = message;
                passWordSignUpWarningText.text = message2; 
            }
            else
            {
                //User has now been created
                //Now get the result
                User = SignupTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _username};

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to Signup task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningSignupText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        LoginPanel(); 
                        warningSignupText.text = "";
                    }
                }
            }
        }
    }
}
