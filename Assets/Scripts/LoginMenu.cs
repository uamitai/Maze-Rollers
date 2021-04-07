using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;

public class LoginMenu : MonoBehaviour {

    public static LoginMenu inst;
    private bool fullScreen = true;

    private const string errorMsg = "Error: Can't communicate with server";

    //All public variables bellow are assigned in the Inspector

    //These are the GameObjects which are parents of groups of UI elements. The objects are enabled and disabled to show and hide the UI elements.
    public GameObject loginParent;
    public GameObject registerParent;
    public GameObject loadingParent;

    //These are all the InputFields which we need in order to get the entered usernames, passwords, etc
    public InputField Login_UsernameField;
    public InputField Login_PasswordField;
    public InputField Register_UsernameField;
    public InputField Register_PasswordField;
    public InputField Register_ConfirmPasswordField;

    //These are the UI Texts which display errors
    public Text Login_ErrorText;
    public Text Register_ErrorText;

    //These store the username and password of the player when they have logged in
    public string playerUsername = "";
    public string playerPassword = "";

    //Called at the very start of the game
    void Awake()
    {
        ResetAllUIElements();
        Screen.fullScreen = true;
        inst = this;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            fullScreen = !fullScreen;
            Screen.fullScreen = fullScreen;
        }
    }

    //Called by Button Pressed Methods to Reset UI Fields
    public void ResetAllUIElements ()
    {
        //This resets all of the UI elements. It clears all the strings in the input fields and any errors being displayed
        Login_UsernameField.text = "";
        Login_PasswordField.text = "";
        Register_UsernameField.text = "";
        Register_PasswordField.text = "";
        Register_ConfirmPasswordField.text = "";
        Login_ErrorText.text = "";
        Register_ErrorText.text = "";
    }

    //UI Button Pressed Methods
    public void Login_LoginButtonPressed ()
    {
        //Called when player presses button to Login
        if (!Client.isConnected)
        {
            SetLoginErrorText(errorMsg);
            Client.Connect();
            return;
        }

        //Get the username and password the player entered
        playerUsername = Login_UsernameField.text;
        playerPassword = Login_PasswordField.text;

        //Check the lengths of the username and password. (If they are wrong, we might as well show an error now instead of waiting for the request to the server)
        if (playerUsername.Length > 3)
        {
            if (playerPassword.Length > 5)
            {
                if (!playerUsername.Contains("'"))
                {
                    if(!playerPassword.Contains("'"))
                    {
                        //Username and password seem reasonable. Change UI to 'Loading...'. Start the Coroutine which tries to log the player in.
                        loginParent.gameObject.SetActive(false);
                        loadingParent.gameObject.SetActive(true);
                        ClientSend.LoginUser(playerUsername, Encrypt(playerPassword));
                    }
                    else
                    {
                        //password cannot contain sql injection characters
                        Login_ErrorText.text = "Error: Password invalid";
                    }
                }
                else
                {
                    //username cannot contain sql injection characters
                    Login_ErrorText.text = "Error: Username invalid";
                }
            }
            else
            {
                //Password too short so it must be wrong
                Login_ErrorText.text = "Error: Password too short";
            }
        } else
        {
            //Username too short so it must be wrong
            Login_ErrorText.text = "Error: Username too short";
        }
    }

    public void Login_RegisterButtonPressed ()
    {
        //Called when the player hits register on the Login UI, so switches to the Register UI
        ResetAllUIElements();
        loginParent.gameObject.SetActive(false);
        registerParent.gameObject.SetActive(true);
    }

    public void Register_RegisterButtonPressed ()
    {
        //Called when the player presses the button to register
        if (!Client.isConnected)
        {
            SetRegisterErrorText(errorMsg);
            Client.Connect();
            return;
        }

        //Get the username and password and repeated password the player entered
        playerUsername = Register_UsernameField.text;
        playerPassword = Register_PasswordField.text;
        string confirmedPassword = Register_ConfirmPasswordField.text;

        //Make sure username and password are long enough
        if (playerUsername.Length > 3)
        {
            if (playerPassword.Length > 5)
            {
                //Check the two passwords entered match
                if (playerPassword == confirmedPassword)
                {
                    if (!playerUsername.Contains("'"))
                    {
                        if (!playerPassword.Contains("'"))
                        {
                            //Username and passwords seem reasonable. Switch to 'Loading...' and start the coroutine to try and register an account on the server
                            registerParent.gameObject.SetActive(false);
                            loadingParent.gameObject.SetActive(true);
                            ClientSend.RegisterUser(playerUsername, Encrypt(playerPassword));
                        }
                        else
                        {
                            //password cannot contain sql injection characters
                            Login_ErrorText.text = "Error: Password invalid";
                        }
                    }
                    else
                    {
                        //username cannot contain sql injection characters
                        Login_ErrorText.text = "Error: Username invalid";
                    }
                }
                else
                {
                    //Passwords don't match, show error
                    Register_ErrorText.text = "Error: Passwords don't match";
                }
            }
            else
            {
                //Password too short so show error
                Register_ErrorText.text = "Error: Password too short";
            }
        }
        else
        {
            //Username too short so show error
            Register_ErrorText.text = "Error: Username too short";
        }
    }

    public void Register_BackButtonPressed ()
    {
        //Called when the player presses the 'Back' button on the register UI. Switches back to the Login UI
        ResetAllUIElements();
        loginParent.gameObject.SetActive(true);
        registerParent.gameObject.SetActive(false);
    }

    public void SetLoginErrorText(string text)
    {
        loginParent.gameObject.SetActive(true);
        loadingParent.gameObject.SetActive(false);
        Login_ErrorText.text = text;
    }

    public void SetRegisterErrorText(string text)
    {
        registerParent.gameObject.SetActive(true);
        loadingParent.gameObject.SetActive(false);
        Register_ErrorText.text = text;
    }

    static string Encrypt(string password)
    {
        //simple encrypting function
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        UTF8Encoding utf8 = new UTF8Encoding();
        byte[] data = md5.ComputeHash(utf8.GetBytes(password));
        md5.Dispose();
        return Convert.ToBase64String(data);
    }
}
