using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.UI;
using Google;
using System.Net.Http;

public class GoogleLogin : MonoBehaviour
{

    public string GoogleWebAPI= "913914480574-akki3qdde95qo31tthg6j4ohs3mk7btv.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public Text UsernameTxt, UserEmailTxt;
    public Image UserProfilePic;
    public string imageUrl;

    public GameObject LoginScreen,ProfileScreen;
    
    void Awake(){
        configuration = new GoogleSignInConfiguration
        {
            WebclientId = GoogleWebAPI,
            RequestIdToken = True
        };
    }

    void Start(){
        InitFirebase();
    }
    void InitFirebase(){
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    void GoogleSignInClick(){
        GoogleSignIn.Configuration = configuration;
         GoogleSignIn.Configuration.UserGameSignIn=false;
          GoogleSignIn.Configuration.RequestIdToken=true;
           GoogleSignIn.Configuration.RequestEmail=true;

           GoogleSignin.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);

    }

    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task){
        
        if (task.IsFaulted){
            Debug.LogError("Fault");
        }
        else if (task.IsCanceled){
            Debug.LogError("Canceled");
        }
        else{
            Firebase.Auth.Credential credential = Firabase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken,null);
            Auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task=>
            {
                if (task.IsCanceled){
                    Debug.LogError("SignInWithCredentialAsync was canceled");
                }
                if (task.IsFaulted){
                    Debug.LogError("SignInwithCredentialAsync encountered an error"+task.Exeption);
                    return;
                }

                    user = auth.CurrentUser;

                
                UsernameTxt.txt = user.DisplayName;
                UserEmailTxt.txt= user.Email;

                LoginScreen.SetActive(false);
                ProfileScreen.SetActive(true);

                StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.Tostring())));
            });
        }
    }

    private string CheckImageUrl(string url){
        if (!string.IsNullOrEmpty(url)){
            return url;
        }
        return imageUrl;
    }
    IEnumerator LoadImage(string imageUri){

        WWW wwww = new WWW (imageUri);
        yield return www;

        UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0,0, www.texture.width,www.texture.height),new Vector2(0,0));
    }

}
