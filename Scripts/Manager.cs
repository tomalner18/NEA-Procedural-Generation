using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

	[SerializeField]
	private GameObject player;
	[SerializeField]
	private GameObject minimapCamera;
	[SerializeField]
	private GameObject menuMinimapCamera;
	private Player pPlayer;
	[SerializeField]
	private GameObject inventoryMenu;
	private MapGen mapGen;
	private GameObject collectable;
	[SerializeField]
	private GameObject minimap;
	[SerializeField]
	private GameObject north;
	[SerializeField]
	private GameObject pauseMarker;
	private MinimapPathManager minimapPathManager;
	[SerializeField]
	private GameObject popup;
	[SerializeField]
	private GameObject popupTitle;
	[SerializeField]
	private GameObject popupText;
	[SerializeField]
	private Button popupButton;
	private Text popupTitleText;
	private GameObject[] enemies;
	private Text popupTextText;

	private bool paused = false;
	void Start () {
		popup.SetActive(false);
		mapGen = FindObjectOfType<MapGen>();
		pPlayer = FindObjectOfType<Player>();
		menuMinimapCamera.GetComponent<Camera>().orthographicSize = mapGen.terrainData.mapDimension.x * 240 / 2;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			menuMinimapCamera.SetActive(!menuMinimapCamera.activeInHierarchy);
			paused = !paused;
			// shows inventory menu
			inventoryMenu.SetActive(!inventoryMenu.activeInHierarchy);
			// disables standard minimap
			minimap.SetActive(!minimap.activeInHierarchy);
			minimapPathManager = GetComponent<MinimapPathManager>();
			if (paused)
			{
				pauseMarker.GetComponent<MarkerBob>().SetPosition(pPlayer.transform.position, mapGen.wholeNoiseMap.GetLength(0));
				enemies = GameObject.FindGameObjectsWithTag("Enemy");
				// pauses game time
				Time.timeScale = 0;
				// updates pause minimap
				minimapPathManager.UpdatePath(player, pPlayer.collectables["Pathfinder"].getHas(), pPlayer.collectables["Enemy Finder"].getHas(), enemies);

			}
			else
			{
				// resumes game on every pause button press
				Time.timeScale = 1;
			}
		}
		if(pPlayer.collectables["Compass"].getHas())
		{
			// displays compass on minimap if player has the collectable
			north.SetActive(true);
		}
		else
		{
			north.SetActive(false);
		}
		if (pPlayer.collectables["Enemy Finder"].getHas())
		{
			//bitwise shift
			menuMinimapCamera.GetComponent<Camera>().cullingMask |= (1 << 14);
			minimapCamera.GetComponent<Camera>().cullingMask |= (1 << 14);
		}
		else
		{
			menuMinimapCamera.GetComponent<Camera>().cullingMask &= ~(1 << 14);
			minimapCamera.GetComponent<Camera>().cullingMask &= ~(1 << 14);
		}
		if (pPlayer.collectables["Player Marker"].getHas())
		{
			pauseMarker.SetActive(true);
		}
		else
		{
			pauseMarker.SetActive(false);
		}
	}
	public void DisplayPopup(Item item)
	{
		// displays collectable information when found
		popupTitleText = popupTitle.GetComponent<Text>();
		popupTextText = popupText.GetComponent<Text>();
		popupTitleText.text = "You found the " + item.getDisplayName() + "!";
		popupTextText.text = item.getDescription();
		popup.SetActive(true);
		popupButton.Select();
		Time.timeScale = 0;
	}
	public void HidePopup()
	{
		popup.SetActive(false);
		Time.timeScale = 1;
	}
}

