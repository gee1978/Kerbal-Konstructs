﻿using KerbalKonstructs.StaticObjects;
using KerbalKonstructs.API;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Upgradeables;
using UpgradeLevel = Upgradeables.UpgradeableObject.UpgradeLevel;

// R and T LOG
// 14112014 ASH

namespace KerbalKonstructs.LaunchSites
{
	public class LaunchSiteManager
	{
		private static List<LaunchSite> launchSites = new List<LaunchSite>();
		public static Texture defaultLaunchSiteLogo = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/DefaultSiteLogo", false);

		public static LaunchSite runway = new LaunchSite("Runway", "Squad", SiteType.SPH, GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/KSCRunway", false), null, "The KSC runway is a concrete runway measuring about 2.5km long and 70m wide, on a magnetic heading of 90/270. It is not uncommon to see burning chunks of metal sliding across the surface.", "Runway", 0, 0, "Open", SpaceCenter.Instance.gameObject/*, new PSystemSetup.SpaceCenterFacility()*/);
		public static LaunchSite launchpad = new LaunchSite("LaunchPad", "Squad", SiteType.VAB, GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/KSCLaunchpad", false), null, "The KSC launchpad is a platform used to fire screaming Kerbals into the kosmos. There was a tower here at one point but for some reason nobody seems to know where it went...", "RocketPad", 0, 0, "Open", SpaceCenter.Instance.gameObject/*, new PSystemSetup.SpaceCenterFacility()*/);

		static LaunchSiteManager()
		{
			// TODO Expose KSC runway and launchpad descriptions to KerbalKonstructs.cfg so players can change them
			// ASH 28102014 - Added category
			// ASH 12112014 - Added career open close costs and state
			launchSites.Add(runway);
			launchSites.Add(launchpad);
		}

		// KerbTown legacy code - TODO Review and update?
		public static void createLaunchSite(StaticObject obj)
		{
			if (obj.settings.ContainsKey("LaunchSiteName") && obj.gameObject.transform.Find((string) obj.getSetting("LaunchPadTransform")) != null)
			{
				// Debug.Log("KK: Creating launch site " + obj.getSetting("LaunchSiteName"));
				obj.gameObject.transform.name = (string) obj.getSetting("LaunchSiteName");
				obj.gameObject.name = (string) obj.getSetting("LaunchSiteName");
				// var x = obj.gameObject.AddComponent(typeof(UpgradeableFacility)) as UpgradeableFacility;
				// x.SetLevel(0);
				// x.setNormLevel(0);

				foreach (FieldInfo fi in PSystemSetup.Instance.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
				{
					if (fi.FieldType.Name == "SpaceCenterFacility[]")
					{
						PSystemSetup.SpaceCenterFacility[] facilities = (PSystemSetup.SpaceCenterFacility[])fi.GetValue(PSystemSetup.Instance);
						if (PSystemSetup.Instance.GetSpaceCenterFacility((string) obj.getSetting("LaunchSiteName")) == null)
						{
							PSystemSetup.SpaceCenterFacility newFacility = new PSystemSetup.SpaceCenterFacility();
							newFacility.name = "FacilityName";
							newFacility.facilityName = (string) obj.getSetting("LaunchSiteName");
							newFacility.facilityPQS = ((CelestialBody) obj.getSetting("CelestialBody")).pqsController;
							newFacility.facilityTransformName = obj.gameObject.name;
							newFacility.pqsName = ((CelestialBody) obj.getSetting("CelestialBody")).pqsController.name;
							PSystemSetup.SpaceCenterFacility.SpawnPoint spawnPoint = new PSystemSetup.SpaceCenterFacility.SpawnPoint();
							spawnPoint.name = (string) obj.getSetting("LaunchSiteName");
							spawnPoint.spawnTransformURL = (string) obj.getSetting("LaunchPadTransform");
							newFacility.spawnPoints = new PSystemSetup.SpaceCenterFacility.SpawnPoint[1];
							newFacility.spawnPoints[0] = spawnPoint;
							PSystemSetup.SpaceCenterFacility[] newFacilities = new PSystemSetup.SpaceCenterFacility[facilities.Length + 1];
							for (int i = 0; i < facilities.Length; ++i)
							{
								newFacilities[i] = facilities[i];
							}
							newFacilities[newFacilities.Length - 1] = newFacility;
							fi.SetValue(PSystemSetup.Instance, newFacilities);
							facilities = newFacilities;
							
							Texture logo = null;
							Texture icon = null;

							if (obj.settings.ContainsKey("LaunchSiteLogo"))
								logo = GameDatabase.Instance.GetTexture(obj.model.path + "/" + obj.getSetting("LaunchSiteLogo"), false);

							if (logo == null)
								logo = defaultLaunchSiteLogo;

							if(obj.settings.ContainsKey("LaunchSiteIcon"))
								icon = GameDatabase.Instance.GetTexture(obj.model.path + "/" + obj.getSetting("LaunchSiteIcon"), false);
							
							// TODO This is still hard-code and needs to use the API properly
							// ASH 12112014 - Added career open close costs
							launchSites.Add(new LaunchSite((string)obj.getSetting("LaunchSiteName"), (obj.settings.ContainsKey("LaunchSiteAuthor")) ? (string)obj.getSetting("LaunchSiteAuthor") : (string)obj.model.getSetting("author"), (SiteType)obj.getSetting("LaunchSiteType"), logo, icon, (string)obj.getSetting("LaunchSiteDescription"), (string)obj.getSetting("Category"), (float)obj.getSetting("OpenCost"), (float)obj.getSetting("CloseValue"), "Closed", /*(Vector3)obj.getSetting("RadialPosition"), */obj.gameObject, newFacility));
							// Debug.Log("KK: Created launch site \"" + newFacility.name + "\" with transform " + obj.getSetting("LaunchSiteName") + "/" + obj.getSetting("LaunchPadTransform"));
						}
						else
						{
							Debug.Log("KK: Launch site " + obj.getSetting("LaunchSiteName") + " already exists.");
						}
					}
				}

				MethodInfo updateSitesMI = PSystemSetup.Instance.GetType().GetMethod("SetupFacilities", BindingFlags.NonPublic | BindingFlags.Instance);
				if (updateSitesMI == null)
					Debug.Log("KK: Failed to find SetupFacilities().");
				else
					updateSitesMI.Invoke(PSystemSetup.Instance, null);

				if (obj.gameObject != null)
					CustomSpaceCenter.CreateFromLaunchsite((string)obj.getSetting("LaunchSiteName"), obj.gameObject);
			}
			else
			{
				Debug.Log("KK: Launch pad transform \"" + obj.getSetting("LaunchPadTransform") + "\" missing for " + obj.getSetting("LaunchSiteName"));
			}
		}

		// ASH 28102014 Added handling for Category filter
		public static List<LaunchSite> getLaunchSites(String usedFilter = "ALL")
		{
			List<LaunchSite> sites = new List<LaunchSite>();
			foreach (LaunchSite site in launchSites)
			{
				if (usedFilter.Equals("ALL"))
				{
					sites.Add(site);
				}
				else
				{
					if (site.category.Equals(usedFilter))
					{
						sites.Add(site);
					}
				}
			}
			return sites;
		}

		// ASH 28102014 Added handling for Category filter
		public static List<LaunchSite> getLaunchSites(SiteType type, Boolean allowAny = true, String appliedFilter = "ALL")
		{
			List<LaunchSite> sites = new List<LaunchSite>();
			foreach (LaunchSite site in launchSites)
			{
				if (site.type.Equals(type) || (site.type.Equals(SiteType.Any) && allowAny))
				{
					if (appliedFilter.Equals("ALL"))
					{
						sites.Add(site);
					}
					else
					{
						if (site.category.Equals(appliedFilter))
						{
							sites.Add(site);
						}
					}
				}
			}
			return sites;
		}

		// Open and close a launchsite
		public static void setSiteOpenCloseState(string sSiteName, string sState)
		{
			List<LaunchSite> sites = LaunchSiteManager.getLaunchSites();
			foreach (LaunchSite site in sites)
			{
				if (site.name == sSiteName)
				{
					site.openclosestate = sState;
					PersistenceFile<LaunchSite>.SaveList(sites, "LAUNCHSITES", "KK");
					return;
				}
			}
		}

		// Find out if a launchsite is open or closed
		public static void getSiteOpenCloseState(string sSiteName, out string sOpenCloseState, out float fOpenCost)
		{
			List<LaunchSite> sites = LaunchSiteManager.getLaunchSites();
			foreach (LaunchSite site in sites)
			{
				if (site.name == sSiteName)
				{
					sOpenCloseState = site.openclosestate;
					fOpenCost = site.opencost;
					return;
				}
			}

			sOpenCloseState = "Open";
			fOpenCost = 0;
		}

		public static void setAllLaunchsitesClosed()
		{
			List<LaunchSite> sites = LaunchSiteManager.getLaunchSites();
			foreach (LaunchSite site in sites)
			{
				site.openclosestate = "Closed";
			}
		}

		public static float RangeNearestOpenBase = 0f;
		public static string NearestOpenBase = "";

		public static Boolean isCareerGame()
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

		public static float getDistanceToBase(Vector3 position, LaunchSite lTarget)
		{
			float flRange = 0f;

			List<LaunchSite> sites = LaunchSiteManager.getLaunchSites();
			foreach (LaunchSite site in sites)
			{
				if (site == lTarget)
				{
					var radialposition = site.GameObject.transform.position;
					var dist = Vector3.Distance(position, radialposition);
					flRange = dist;
				}
			}

			return flRange;
		}

		// Get the nearest open base and range to it
		public static void getNearestOpenBase(Vector3 position, out string sBase, out float flRange, out LaunchSite lNearest)
		{
			SpaceCenter KSC = SpaceCenter.Instance;
			var smallestDist = Vector3.Distance(KSC.gameObject.transform.position, position);
			string sNearestBase = "";
			string sOpenCloseState = "";
			LaunchSite lNearestBase = null;

			List<LaunchSite> basesites = LaunchSiteManager.getLaunchSites();

			foreach (LaunchSite site in basesites)
			{
				sOpenCloseState = site.openclosestate;

				if (!isCareerGame())
					sOpenCloseState = "Open";

				if (sOpenCloseState == "Open")
				{
					var radialposition = site.GameObject.transform.position;
					var dist = Vector3.Distance(position, radialposition);

					if (site.name == "Runway")
					{
						if (lNearestBase == null)
							lNearestBase = site;
					}
					else if (site.name != "LaunchPad")
					{
						if ((float)dist < (float)smallestDist)
						{
							{
								sNearestBase = site.name;
								lNearestBase = site;
								smallestDist = dist;
							}
						}
					}
				}
			}

			if (sNearestBase == "")
			{
				sNearestBase = "KSC";
			}

			RangeNearestOpenBase = (float)smallestDist;

			// Air traffic control messaging
			if (KerbalKonstructs.instance.enableATC)
			{
				if (sNearestBase != NearestOpenBase)
				{
					if (RangeNearestOpenBase < 25000)
					{
						NearestOpenBase = sNearestBase;
						MessageSystemButton.MessageButtonColor color = MessageSystemButton.MessageButtonColor.BLUE;
						MessageSystem.Message m = new MessageSystem.Message("KK ATC", "You have entered the airspace of " + sNearestBase + " ATC. Please keep this channel open and obey all signal lights. Thank you. " + sNearestBase + " Air Traffic Control out.", color, MessageSystemButton.ButtonIcons.MESSAGE);
						MessageSystem.Instance.AddMessage(m);
					}
					else
						if (NearestOpenBase != "")
						{
							// you have left ...
							MessageSystemButton.MessageButtonColor color = MessageSystemButton.MessageButtonColor.GREEN;
							MessageSystem.Message m = new MessageSystem.Message("KK ATC", "You are now leaving the airspace of " + sNearestBase + ". Safe journey. " + sNearestBase + " Air Traffic Control out.", color, MessageSystemButton.ButtonIcons.MESSAGE);
							MessageSystem.Instance.AddMessage(m);
							NearestOpenBase = "";
						}
				}
			}

			sBase = sNearestBase;
			flRange = RangeNearestOpenBase;
			lNearest = lNearestBase;
		}

		public static float RangeNearestBase = 0f;
		public static string NearestBase = "";

		// Get nearest base, either open or closed, and the range to it
		public static void getNearestBase(Vector3 position, out string sBase, out float flRange, out LaunchSite lSite)
		{
			SpaceCenter KSC = SpaceCenter.Instance;
			var smallestDist = Vector3.Distance(KSC.gameObject.transform.position, position);
			string sNearestBase = "";
			LaunchSite lTargetSite = null;

			List<LaunchSite> basesites = LaunchSiteManager.getLaunchSites();

			foreach (LaunchSite site in basesites)
			{
				if (site.GameObject == null) continue;

				var radialposition = site.GameObject.transform.position;
				var dist = Vector3.Distance(position, radialposition);

				if (site.name == "Runway" || site.name == "LaunchPad")
				{ }
				else
				{
					if ((float)dist < (float)smallestDist)
					{
						{
							sNearestBase = site.name;
							smallestDist = dist;
							lTargetSite = site;
						}
					}
				}
			}

			if (sNearestBase == "")
			{
				sNearestBase = "KSC";
			}

			RangeNearestBase = (float)smallestDist;

			sBase = sNearestBase;
			flRange = RangeNearestBase;
			lSite = lTargetSite;
		}

		public static void setLaunchSite(LaunchSite site)
		{
			// Debug.Log("KK: EditorLogic.fetch.launchSiteName set to " + site.name);
			//Trick KSP to think that you launched from Runway or LaunchPad
			//I'm sure Squad will break this in the future
			//This only works because they use multiple variables to store the same value, basically its black magic.
			//--medsouz
			if (site.facility != null)
			{
				if (EditorDriver.editorFacility.Equals(EditorFacility.SPH))
				{
					site.facility.name = "Runway";
				}
				else
				{
					site.facility.name = "LaunchPad";
				}
			}
			EditorLogic.fetch.launchSiteName = site.name;
		}

		public static string getCurrentLaunchSite()
		{
			return EditorLogic.fetch.launchSiteName;
		}

		public static List<LaunchSite> AllLaunchSites { get { return launchSites; } }
	}
}
