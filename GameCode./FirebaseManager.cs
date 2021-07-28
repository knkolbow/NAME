using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    // Firebase Data Fields
    public DependencyStatus dependencyStatus;
    public static FirebaseAuth auth;
    public static FirebaseUser user;
    public DatabaseReference DBreference;

    // Login Data Fields
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;
    public TMP_Text loginOutputText;

    // Register Data Fields
    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;
    public TMP_InputField registerVerifyPassword;
    public TMP_Text registerOutputText;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        //Initialize Firebase
        StartCoroutine(CheckAndFixDependancies());
    }

    private IEnumerator CheckAndFixDependancies()
    {
        var checkAndFixDependanciesTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(predicate: () => checkAndFixDependanciesTask.IsCompleted);
        dependencyStatus = checkAndFixDependanciesTask.Result;

        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(CheckAutoLogin());       // Checks if player is already logged in
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        DBreference = FirebaseDatabase.DefaultInstance.RootReference; // Connects to database
    }

    // Checks if user is already logged in
    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();
        if(user != null)
        {
            var reloadUserTask = user.ReloadAsync();
            yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);
            AutoLogin();
        }   
    }

    private void AutoLogin()
    {
        if (user != null)
        {
            //If user is logged in go straight to main menu.
            GameManager.instance.ChangeScene(1);
        }    
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed Out " + user.Email);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.Email);
            }
        }
    }
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public void ResetLoginFields()
    {
        loginEmail.text = "";
        loginPassword.text = "";
        loginOutputText.text = "";
    }

    public void ResetRegisterFields()
    {
        registerEmail.text = "";
        registerPassword.text = "";
        registerVerifyPassword.text = "";
        registerOutputText.text = "";
    }

    // Logs user in
    public void LoginButton()
    {
        StartCoroutine(LoginLogic(loginEmail.text, loginPassword.text));

    }
    
    // Registers user
    public void RegisterButton()
    {
        StartCoroutine(Register(registerEmail.text, registerPassword.text));
    }

    // Takes you to the register screen
    public void goToRegisterButton()
    {
        ResetLoginFields();
        GameManager.instance.ChangeScene(5);
    }

    // Takes you to the login screen
    public void BackToLogin()
    {
        ResetRegisterFields();
        GameManager.instance.ChangeScene(0);
    }

    private IEnumerator LoginLogic(string email, string password)
    {
        //Call the Firebase auth signin function passing the email and password
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);
       
        if (loginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {loginTask.Exception}");
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError error = (AuthError)firebaseEx.ErrorCode;

            string output = "Login Failed, Please Try Again";

            switch (error)
            {
                case AuthError.MissingEmail:
                    output = "Please Enter Your Email";
                    break;
                case AuthError.MissingPassword:
                    output = "Please Enter Your Password";
                    break;
                case AuthError.InvalidEmail:
                    output = "InvalidEmail";
                    break;
                case AuthError.WrongPassword:
                    output = "Incorrect Password";
                    break;
                case AuthError.UserNotFound:
                    output = "Account Does Not Exist";
                    break;
            }
            loginOutputText.text = output;
        }
        else
        {
            // User is logged in
            loginOutputText.text = "Signed In";
            user = loginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);

            yield return new WaitForSeconds(1);
            GameManager.instance.ChangeScene(1);

        }
    }

    private IEnumerator Register(string _email, string _password)
    {
        if (_email == "")
        {
            //If the email field is blank show a warning
            registerOutputText.text = "Missing Email";
        }
        else if (registerPassword.text != registerVerifyPassword.text)
        {
            //If the password does not match show a warning
            registerOutputText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                //Reset user input
                ResetRegisterFields();

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                registerOutputText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                user = RegisterTask.Result;

                if (user != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _email};
                    string userId = user.UserId;
                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = user.UpdateUserProfileAsync(profile);

                    //Set default user values
                    DBreference.Child("users").Child(userId);
                    DBreference.Child("users").Child(userId).Child("level").SetValueAsync(1);
                    DBreference.Child("users").Child(userId).Child("kills").SetValueAsync(0);
                    DBreference.Child("users").Child(userId).Child("deaths").SetValueAsync(0);
                    DBreference.Child("users").Child(userId).Child("username").SetValueAsync(_email);

                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);


                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        registerOutputText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Return to login screen
                        registerOutputText.text = "Successful!";
                        GameManager.instance.ChangeScene(1);

                    }
                }
            }
        }
    }
}
