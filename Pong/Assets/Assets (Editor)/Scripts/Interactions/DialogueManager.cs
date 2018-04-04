﻿using UnityEngine;
using System;
using System.Xml;

public class DialogueManager : MonoBehaviour {

	public TextAsset XML;
	private Dialogue dialog;
	private Dialogue currentdialog;

	public bool talking;
	private bool mouseover;

	private string playerObject = "PlayerMan";
	public PlayerMovementController control;
	public PlayerInteractionController interact;
	//public NPCScript owner;
	private Inventory inventory;

	//public GameObject itemToGive; // this will be ripped out and made a part of prisoners to whom a dialogue is attached, fine for now

	public int width = 20;
	public int height = 20;

	private GUIStyle custombutton, extraButton;
	public int fontSize = 15;

	public string name;

	private float displayTime = 0.0f;
	private string display = "";

	void Start () {
		GameObject player = GameObject.Find (playerObject);
		inventory = player.GetComponent<Inventory> ();

		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(XML.text);
		dialog = CreateTree (xmlDoc.FirstChild);
		Cursor.lockState = CursorLockMode.Locked;

		width = width * Screen.width / 100;
		height = height * Screen.height / 100;
	}

	public void Reset()
	{
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(XML.text);
		dialog = CreateTree(xmlDoc.FirstChild);
		Cursor.lockState = CursorLockMode.Locked;

	}

	Dialogue CreateTree(XmlNode xml) {
		Dialogue d = new Dialogue ();
		XmlAttributeCollection attr = xml.Attributes;
		d.Text = attr["text"].Value;
		d.Option = attr["option"].Value;
		d.Req = attr ["require"] != null ? attr ["require"].Value : "";
		d.Take = attr["take"] != null ? attr["take"].Value : "";
		d.Rep = attr ["rep"] != null ? int.Parse(attr ["rep"].Value) : 0;
		d.RepMin = attr ["repMin"] != null ? int.Parse(attr ["repMin"].Value) : -1000;
		d.RepMax = attr ["repMax"] != null ? int.Parse(attr ["repMax"].Value) : 1000;
		if (attr ["sendX"] != null) {
			d.sendToX = int.Parse (attr ["sendX"].Value);
			d.sendToY = int.Parse (attr ["sendY"].Value);
			d.sendToZ = int.Parse (attr ["sendZ"].Value);
		} else {
			d.sendToX = 0;
			d.sendToY = 0;
			d.sendToZ = 0;
		}

		if (attr ["item"] != null)
		{
			//Should be int to line up with ItemAttributeInformation options. This is for inventory icons mostly
			//this is the name of the item basically (or the identifier for key-door connection)             
			d.GiveItem = new Pickup(attr["id"].Value, (ItemAttributeInformation.Type)int.Parse(attr["item"].Value), interact.GetComponent<ItemIconHolder>());
		}
		else d.GiveItem = null;		

		if (xml.HasChildNodes)
		{
			for (int i = 0; i < xml.ChildNodes.Count; i++)
			{
				d.AddChild (CreateTree (xml.ChildNodes [i]));
			}
		}
		return d;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Escape))
		{
			if (talking) EndDialogue ();
		}
		if (displayTime > 0) {
			displayTime -= 0.1f;
		}
	}

	public void LookingAt()
	{
		if (!talking)
		{
			mouseover = true;
		}
	}

	public void NoLongerLookingAt()
	{
		mouseover = false;
	}

	void OnGUI() {
		GUI.skin.box.wordWrap = true;
		GUI.skin.button.wordWrap = true;

		if (talking)
		{
			if(custombutton == null) custombutton = new GUIStyle("button") {fontSize = fontSize};
		    if (extraButton == null)
		    {
		        extraButton = new GUIStyle("button")
		        {
		            fontSize = fontSize,
		            onHover = new GUIStyleState(),
		            hover = new GUIStyleState()
		        };
		    }
            GUI.Box (new Rect (20, 50, width, height), name+": "+currentdialog.Text, extraButton);
			if (currentdialog.Children().Count == 0)
			{
				if (GUI.Button(new Rect(20, height + 60, width, height), "End", custombutton))
				{
					EndDialogue();
				}
			}
			int count = 0;
			for (int i = 0; i < currentdialog.Children().Count; i++)
			{
				Dialogue child = currentdialog.Children () [i];
				if (InventoryCheck(child.Req) && ReputationCheck(child.RepMin,child.RepMax))
				{
					if (GUI.Button(new Rect(20, 60 + height * (count + 1), width, height), child.Option,custombutton))
					{
						if (child.GiveItem != null)
						{
							inventory.AddItem(child.GiveItem);
							setDisplayText ("Item added to inventory");
							child.GiveItem = null;
						}
						if (child.Take.Length > 0)
						{
							inventory.RemoveThis(child.Take);
						}
						interact.Reputation [name] = interact.Reputation [name] + child.Rep;
						if (child.Rep > 0) {
							setDisplayText ("Reputation increased");
						}
						if (child.sendToX!=0 && child.sendToY!=0 && child.sendToZ!=0) {
							EndDialogue ();
							control.SetPosition (new Vector3 (child.sendToX, child.sendToY, child.sendToZ));
							break;
						}
						ContinueDialogue(i);
						break;
					}
					count++;
				}
			}
		}

		if (displayTime > 0) {
			GUI.Box (new Rect (Screen.width/2 - width/2, Screen.height/2 - height/2, width, height), display, extraButton);
			print (display);
		}
	}  

	public void StartDialogue()
	{
		if (talking) return;
		mouseover = false;
		talking = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		currentdialog = dialog;     
	}

	void ContinueDialogue(int i)
	{
		currentdialog = currentdialog.Children () [i];
	}

	void EndDialogue() {
		talking = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		control.LeaveConversation();
	}

	bool InventoryCheck(string id)
	{
		return id.Length == 0 || inventory.HaveItem(id);
	}

	bool ReputationCheck(int r1, int r2) {
		return interact.Reputation [name] > r1 && interact.Reputation[name] < r2;
	}

	void setDisplayText(string str) {
		display = str;
		displayTime = 2.0f;
	}
}
