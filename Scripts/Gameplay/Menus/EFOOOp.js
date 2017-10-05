#pragma strict

function Start () {
	
}

function Update () {
	    //if running on Android, check for Menu/Home and exit
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
            {
                Application.Quit();
                return;
            }
        }
}