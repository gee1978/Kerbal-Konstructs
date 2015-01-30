﻿using KerbalKonstructs.LaunchSites;
using System;
using System.Collections.Generic;
using KerbalKonstructs.API;
using UnityEngine;

namespace KerbalKonstructs.UI
{
	public class MapIconManager
	{
		public Texture VABIcon = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/VABMapIcon", false);
		public Texture SPHIcon = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/SPHMapIcon", false);
		public Texture ANYIcon = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/ANYMapIcon", false);
		private Boolean displayingTooltip = false;
		Rect mapManagerRect = new Rect(200, 150, 210, 600);
		static LaunchSite selectedSite = null;

		public static LaunchSite getSelectedTarget()
		{
			LaunchSite thisSite = selectedSite;
			return thisSite;
		}

		public void drawManager()
		{
			mapManagerRect = GUI.Window(0xB00B2E7, mapManagerRect, drawMapManagerWindow, "Base Boss : Map Manager");
		}

		bool loadedPersistence = false;

		public Boolean isCareerGame()
		{
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
			{
				// disableCareerStrategyLayer is configurable in KerbalKonstructs.cfg
				if (!KerbalKonstructs.instance.disableCareerStrategyLayer)
				{
					return true;
				}
				else
					return false;
			}
			else
				return false;
		}

		public float iFundsOpen = 0;
		public float iFundsClose = 0;
		public Boolean isOpen = false;
		public Vector2 sitesScrollPosition;
		public Vector2 descriptionScrollPosition;

		void drawMapManagerWindow(int windowID)
		{
			if (!loadedPersistence && isCareerGame())
			{
				PersistenceFile<LaunchSite>.LoadList(LaunchSiteManager.AllLaunchSites, "LAUNCHSITES", "KK");
				loadedPersistence = true;
			}
			GUI.enabled = (isCareerGame());
			if (!isCareerGame())
			{
				KerbalKonstructs.instance.mapShowOpen = GUILayout.Toggle(true, "Show open bases");
				KerbalKonstructs.instance.mapShowClosed = GUILayout.Toggle(true, "Show closed bases");
			}
			else
			{
				KerbalKonstructs.instance.mapShowOpen = GUILayout.Toggle(KerbalKonstructs.instance.mapShowOpen, "Show open bases");
				KerbalKonstructs.instance.mapShowClosed = GUILayout.Toggle(KerbalKonstructs.instance.mapShowClosed, "Show closed bases");
			}
			GUI.enabled = true;
			GUILayout.Space(5);
			KerbalKonstructs.instance.mapShowRocketbases = GUILayout.Toggle(KerbalKonstructs.instance.mapShowRocketbases, "Show rocketpads");
			KerbalKonstructs.instance.mapShowHelipads = GUILayout.Toggle(KerbalKonstructs.instance.mapShowHelipads, "Show helipads");
			KerbalKonstructs.instance.mapShowRunways = GUILayout.Toggle(KerbalKonstructs.instance.mapShowRunways, "Show runways");
			KerbalKonstructs.instance.mapShowOther = GUILayout.Toggle(KerbalKonstructs.instance.mapShowOther, "Show other launchsites");
			GUILayout.Space(3);

			if (selectedSite != null)
			{
				GUILayout.Box("Selected Base");
				GUILayout.Label(selectedSite.name);

				GUILayout.Box(selectedSite.logo, GUILayout.Width(196), GUILayout.Height(187));

				descriptionScrollPosition = GUILayout.BeginScrollView(descriptionScrollPosition);
					GUI.enabled = false;
					GUILayout.TextArea(selectedSite.description);
					GUI.enabled = true;
				GUILayout.EndScrollView();

				// Career mode - get cost to open and value of opening from launchsite (defined in the cfg)
				iFundsOpen = selectedSite.opencost;
				iFundsClose = selectedSite.closevalue;
				
				bool isAlwaysOpen = false;
				bool cannotBeClosed = false;

				// Career mode
				// If a launchsite is 0 to open it is always open
				if (iFundsOpen == 0)
					isAlwaysOpen = true;

				// If it is 0 to close you cannot close it
				if (iFundsClose == 0)
					cannotBeClosed = true;

				if (isCareerGame())
				{
					isOpen = (selectedSite.openclosestate == "Open");

					GUI.enabled = !isOpen;
					List<LaunchSite> sites = LaunchSiteManager.getLaunchSites();

					if (!isAlwaysOpen)
					{
						if (GUILayout.Button("Open Base for " + iFundsOpen + " Funds"))
						{
							double currentfunds = Funding.Instance.Funds;

							if (iFundsOpen > currentfunds)
							{
								ScreenMessages.PostScreenMessage("Insufficient funds to open this base!", 10, ScreenMessageStyle.LOWER_CENTER);
							}
							else
							{
								// Open the site - save to instance
								selectedSite.openclosestate = "Open";

								// Charge some funds
								Funding.Instance.AddFunds(-iFundsOpen, TransactionReasons.Cheating);

								// Save new state to persistence
								PersistenceFile<LaunchSite>.SaveList(sites, "LAUNCHSITES", "KK");
							}
						}
					}
					GUI.enabled = true;

					GUI.enabled = isOpen;
					if (!cannotBeClosed)
					{
						if (GUILayout.Button("Close Base for " + iFundsClose + " Funds"))
						{
							// Close the site - save to instance
							// Pay back some funds
							Funding.Instance.AddFunds(iFundsClose, TransactionReasons.Cheating);
							selectedSite.openclosestate = "Closed";

							// Save new state to persistence
							PersistenceFile<LaunchSite>.SaveList(sites, "LAUNCHSITES", "KK");
						}
					}
					GUI.enabled = true;
				}
			}
			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		public void drawIcons()
		{
			displayingTooltip = false;
			MapObject target = PlanetariumCamera.fetch.target;
			if (target.type == MapObject.MapObjectType.CELESTIALBODY)
			{
				List<LaunchSite> sites = LaunchSiteManager.getLaunchSites();
				foreach (LaunchSite site in sites)
				{
					PSystemSetup.SpaceCenterFacility facility = PSystemSetup.Instance.GetSpaceCenterFacility(site.name);
					if (facility != null)
					{
						PSystemSetup.SpaceCenterFacility.SpawnPoint sp = facility.GetSpawnPoint(site.name);
						if (sp != null)
						{
							if (facility.facilityPQS == target.celestialBody.pqsController)
							{
								if (!isOccluded(sp.GetSpawnPointTransform().position, target.celestialBody))
								{
									Vector3 pos = MapView.MapCamera.camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(sp.GetSpawnPointTransform().position));
									Rect screenRect = new Rect((pos.x - 8), (Screen.height - pos.y) - 8, 16, 16);

									bool display = false;
									string openclosed = site.openclosestate;
									string category = site.category;

									if (KerbalKonstructs.instance.mapShowHelipads && category == "Helipad")
										display = true;
									if (KerbalKonstructs.instance.mapShowOther && category == "Other")
										display = true;
									if (KerbalKonstructs.instance.mapShowRocketbases && category == "RocketPad")
										display = true;
									if (KerbalKonstructs.instance.mapShowRunways && category == "Runway")
										display = true;

									if (display && isCareerGame())
									{
										if (!KerbalKonstructs.instance.mapShowOpen && openclosed == "Open")
											display = false;
										if (!KerbalKonstructs.instance.mapShowClosed && openclosed == "Closed")
											display = false;
									}

									if (display)
									{
										if (site.icon != null)
										{
											Graphics.DrawTexture(screenRect, site.icon);
										}
										else
										{
											switch (site.type)
											{
												case SiteType.VAB:
													Graphics.DrawTexture(screenRect, VABIcon);
													break;
												case SiteType.SPH:
													Graphics.DrawTexture(screenRect, SPHIcon);
													break;
												default:
													Graphics.DrawTexture(screenRect, ANYIcon);
													break;
											}
										}
										//"Borrowed" from FinePrint
										//https://github.com/Arsonide/FinePrint/blob/master/Source/WaypointManager.cs#L53
										if (screenRect.Contains(Event.current.mousePosition) && !displayingTooltip)
										{
											//Only display one tooltip at a time
											displayingTooltip = true;
											GUI.Label(new Rect((float)(pos.x) + 16, (float)(Screen.height - pos.y) - 8, 200, 20), site.name);

											if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
											{
												ScreenMessages.PostScreenMessage("Selected base is " + site.name + ".", 5f, ScreenMessageStyle.LOWER_CENTER);
												selectedSite = site;
												EditorGUI.setTargetSite(site);

												if (HighLogic.LoadedSceneIsFlight)
												{

												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		//"Borrowed" from FinePrint
		//https://github.com/Arsonide/FinePrint/blob/master/Source/WaypointManager.cs#L53
		private bool isOccluded(Vector3d loc, CelestialBody body)
		{
			Vector3d camPos = ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position);

			if (Vector3d.Angle(camPos - loc, body.position - loc) > 90)
				return false;

			return true;
		}
	}
}
