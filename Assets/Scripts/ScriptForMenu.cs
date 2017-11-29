﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptForMenu : MonoBehaviour {

	public Canvas quitMenu;
	public Button startText;
	public Button exitText;

	void Start () {
		quitMenu = quitMenu.GetComponent<Canvas> ();
		startText = startText.GetComponent<Button> ();
		exitText = exitText.GetComponent<Button> ();
		quitMenu.enabled = false;

	}

	public void ExitPress(){
		quitMenu.enabled = true;
		startText.enabled = false;
		exitText.enabled = false;
	}
	public void NoPress(){

		quitMenu.enabled = false;
		startText.enabled = true;
		exitText.enabled = true;
	}

	public void StartLevel(){
		Application.LoadLevel (0);
	}
	public void ExitGame(){
		Application.Quit ();
	}

}
