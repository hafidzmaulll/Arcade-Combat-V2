using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using Firebase.Extensions;

public class FirebaseController : MonoBehaviour
{

    public GameObject loginPanel, signupPanel, profilePanel, forgetPasswordPanel, notificationPanel, menupanel, controlpanel, settingpanel, creditpanel;

    public InputField loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, signupUsername, forgetPassEmail;

    public Text notif_Title_Text, notif_Message_Text, profileUserName_Text, profileUserEmail_Text;

    public Toggle rememberMe;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;

    void InitializeFirebase()
    {
        Debug.Log("Initializing Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (auth == null)
        {
            Debug.LogError("FirebaseAuth not initialized!");
        }
        else
        {
            Debug.Log("FirebaseAuth initialized successfully");
        }
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.

            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void BackToMainMenu()
    {
        // Hide the profile panel
        profilePanel.SetActive(false);

        // Show the main menu panel
        menupanel.SetActive(true);
        controlpanel.SetActive(false);
        settingpanel.SetActive(false);
        creditpanel.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        // Reset the input fields for login
        loginEmail.text = "";
        loginPassword.text = "";

        menupanel.SetActive(false);  // Hide the menu panel
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenSignUpPanel()
    {
        // Reset the input fields for sign-up
        signupEmail.text = "";
        signupPassword.text = "";
        signupCPassword.text = "";
        signupUsername.text = ""; 

        menupanel.SetActive(false);  // Hide the menu panel
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenProfilePanel()
    {
        menupanel.SetActive(false);  // Hide the menu panel
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenForgetPassPanel()
    {
        // Reset the input fields for forget password
        forgetPassEmail.text = "";
        
        menupanel.SetActive(false);  // Hide the menu panel
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginEmail.text) && string.IsNullOrEmpty(loginPassword.text))
        {
            ShowNotificationMessage("Error", "Fields Empty! Please Input Details in All Fields!");
            return;
        }

        // Do Login
        SigninUser(loginEmail.text, loginPassword.text);
    }

    public void SignupUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) && string.IsNullOrEmpty(signupPassword.text) && string.IsNullOrEmpty(signupCPassword.text) && string.IsNullOrEmpty(signupUsername.text))
        {
            ShowNotificationMessage("Error", "Fields Empty! Please Input Details in All Fields!");
            return;
        }

        // Do SignUp
        CreateUser(signupEmail.text, signupPassword.text, signupUsername.text);
    }

    public void ForgetPass()
    {
        if (string.IsNullOrEmpty(forgetPassEmail.text))
        {
            ShowNotificationMessage("Error", "Email field cannot be empty!");
            return;
        }

        forgetPasswordSubmit(forgetPassEmail.text);
    }

    private void ShowNotificationMessage(string title, string message)
    {
        notif_Title_Text.text = "" + title;
        notif_Message_Text.text = "" + message;

        notificationPanel.SetActive(true);
    }

    public void CloseNotif_Panel()
    {
        notif_Title_Text.text = "";
        notif_Message_Text.text = "";

        notificationPanel.SetActive(false);
    }

    public void LogOut()
    {
        auth.SignOut();
        profileUserName_Text.text = "";
        profileUserEmail_Text.text = "";
        OpenLoginPanel();
        menupanel.SetActive(false);  // Hide the menu panel when logging out
    }

    void CreateUser(string email, string password, string Username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            Debug.Log("CreateUserWithEmailAndPasswordAsync called");

            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                        {
                            var errorCode = (AuthError)firebaseEx.ErrorCode;
                            ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                        }
                }

                return;
            }

            // Debug log after successful task
            Debug.Log("CreateUserWithEmailAndPasswordAsync completed successfully");

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.Log("AuthResult obtained");

            Firebase.Auth.FirebaseUser newUser = result.User; // Get the user from AuthResult
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            UpdateUserProfile(Username);
        });
    }

    public void SigninUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }

                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Firebase.Auth.FirebaseUser newUser = result.User; // Get the user from AuthResult
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            if (newUser.DisplayName == null || newUser.DisplayName == "")
            {
                Debug.LogWarning("Username is not set!");
            }

            profileUserName_Text.text = "" + newUser.DisplayName;
            profileUserEmail_Text.text = "" + newUser.Email;

            OpenProfilePanel();
        });
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                isSignIn = false;
                isSigned = false;
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }

    void UpdateUserProfile(string Username)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = Username,
                PhotoUrl = new System.Uri("https://placehold.co/600x400/png"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");

                ShowNotificationMessage("Alert", "Account Successfully Created!");

                // Redirect to login panel after profile update
                Debug.Log("Redirecting to login panel");
                OpenLoginPanel();
            });
        }
    }

    bool isSigned = false;

    void Update()
    {
        if (isSignIn)
        {
            if (!isSigned)
            {
                isSigned = true;
                profileUserName_Text.text = "" + user.DisplayName;
                profileUserEmail_Text.text = "" + user.Email;
                OpenProfilePanel();
            }
        }
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Account Not Exist!";
                break;
            case AuthError.MissingPassword:
                message = "Missing Password!";
                break;
            case AuthError.WeakPassword:
                message = "Password Weak!";
                break;
            case AuthError.WrongPassword:
                message = "Wrong Password!";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Your Email Already in Use!";
                break;
            case AuthError.InvalidEmail:
                message = "Your Email Is Invalid!";
                break;
            case AuthError.MissingEmail:
                message = "Your Email Is Missing!";
                break;
            default:
                message = "Invalid Error!";
                break;
        }
        return message;
    }

    void forgetPasswordSubmit(string forgetPasswordEmail)
    {
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task =>
        {
            if(task.IsCanceled)
            {
                Debug.LogError("SendPasswrdResetEmailAsync was canceled");
            }

            if (task.IsFaulted)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }
            }
            ShowNotificationMessage("Alert", "Successfully Send Email For Reset Password!");
        });
    }
}