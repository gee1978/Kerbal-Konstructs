using KerbalKonstructs.LaunchSites;
using KerbalKonstructs.StaticObjects;
using KerbalKonstructs.API;
using System;
using System.Collections.Generic;
using LibNoise.Unity.Operator;
using UnityEngine;
using System.Linq;
using System.IO;
using Upgradeables;
using UpgradeLevel = Upgradeables.UpgradeableObject.UpgradeLevel;

namespace KerbalKonstructs.UI
{
	class EditorGUI
	{
		#region Variable Declarations

			public StaticObject selectedObject;
			static LaunchSite lTargetSite = null;

			#region Texture Definitions
			// Texture definitions
			public Texture tBilleted = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/billeted", false);
			public Texture tCopyPos = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/copypos", false);
			public Texture tPastePos = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/pastepos", false);
			public Texture tIconClosed = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/siteclosed", false);
			public Texture tIconOpen = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/siteopen", false);
			public Texture tLeftOn = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/lefton", false);
			public Texture tLeftOff = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/leftoff", false);
			public Texture tRightOn = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/righton", false);
			public Texture tRightOff = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/rightoff", false);
			public Texture tTextureLeft = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/leftoff", false);
			public Texture tTextureRight = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/rightoff", false);
			public Texture tTextureMiddle = GameDatabase.Instance.GetTexture("medsouz/KerbalKonstructs/Assets/siteclosed", false);
			#endregion

			#region Switches
			// Switches
			public Boolean enableColliders = false;
			public Boolean editingSite = false;
			public Boolean foundingBase = false;
			public Boolean creatingInstance = false;
			public Boolean showLocal = false;
			public Boolean managingFacility = false;
			public Boolean onNGS = false;
			#endregion

			#region GUI Windows
			// GUI Windows
			Rect toolRect = new Rect(150, 25, 310, 440);
			Rect editorRect = new Rect(10, 25, 520, 520);
			Rect siteEditorRect = new Rect(400, 50, 340, 480);
			Rect managerRect = new Rect(10, 25, 400, 405);
			Rect facilityRect = new Rect(150, 75, 400, 620);
			Rect NGSRect = new Rect(250, 50, 350, 150);
			Rect KSCmanagerRect = new Rect(150, 50, 400, 400);
			#endregion

			#region GUI elements
			// GUI elements
			Vector2 scrollPos;
			Vector2 scrollPos2;
			Vector2 scrollPos3;
			Vector2 descScroll;
			GUIStyle listStyle = new GUIStyle();
			GUIStyle navStyle = new GUIStyle();
			SiteType siteType;
			GUIContent[] siteTypeOptions = {
											new GUIContent("VAB"),
											new GUIContent("SPH"),
											new GUIContent("ANY")
										};
			ComboBox siteTypeMenu;
			#endregion

			#region Holders
			// Holders
			String xPos, yPos, zPos, altitude, rotation, customgroup = "";
			String visrange = "";
			String increment = "1";
			String siteName, siteTrans, siteDesc, siteAuthor, siteCategory;
			float flOpenCost, flCloseValue;		
			float fOldRange = 0f;
			// public string sKISAFunds = "0";
			// public string sKISARep = "0";
			#endregion
		
		#endregion
		
		public EditorGUI()
		{
			listStyle.normal.textColor = Color.white;
			listStyle.onHover.background =
			listStyle.hover.background = new Texture2D(2, 2);
			listStyle.padding.left =
			listStyle.padding.right =
			listStyle.padding.top =
			listStyle.padding.bottom = 4;

			navStyle.padding.left = 0;
			navStyle.padding.right = 0;
			navStyle.padding.top = 1;
			navStyle.padding.bottom = 3;
			
			siteTypeMenu = new ComboBox(siteTypeOptions[0], siteTypeOptions, "button", "box", null, listStyle);
		}

		#region draw Methods
		public void drawKSCManager()
		{
			KSCmanagerRect = GUI.Window(0xC00B1E2, KSCmanagerRect, drawKSCmanagerWindow, "Base Boss : KSC Manager");
		}

		public void drawManager(StaticObject obj)
		{
			if (obj != null)
			{
				if (selectedObject != obj)
					updateSelection(obj);

				if (managingFacility)
					facilityRect = GUI.Window(0xB00B1E1, facilityRect, drawFacilityManagerWindow, "Base Boss : Facility Manager");
			}
			
			managerRect = GUI.Window(0xB00B1E2, managerRect, drawBaseManagerWindow, "Base Boss");
		}

		public void drawNGS()
		{
			if (KerbalKonstructs.instance.showNGS)
			{
				NGSRect = GUI.Window(0xB00B1E9, NGSRect, drawNGSWindow, "", navStyle);
			}
		}

		public void drawEditor(StaticObject obj)
		{
			if (obj != null)
			{
				if (selectedObject != obj)
					updateSelection(obj);

				toolRect = GUI.Window(0xB00B1E3, toolRect, drawToolWindow, "KK Instance Editor");

				if (editingSite)
				{
						siteEditorRect = GUI.Window(0xB00B1E4, siteEditorRect, drawSiteEditorWindow, "KK Launchsite Editor");
				}
			}

			editorRect = GUI.Window(0xB00B1E5, editorRect, drawEditorWindow, "Kerbal Konstructs Statics Editor");
		}

		void drawKSCmanagerWindow(int WindowID)
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			foreach (UpgradeableFacility facility in GameObject.FindObjectsOfType<UpgradeableFacility>())
			{
				GUILayout.Button(facility.name + " | Lvl: " + facility.FacilityLevel + "/" + facility.MaxLevel);
			}
			GUILayout.EndScrollView();
			/* GUILayout.Box("Kerbin International Space Agency");
			GUILayout.BeginHorizontal();
			GUILayout.Label("KISA Confidence: ");
			GUI.enabled = false;
			GUILayout.TextField("E");
			GUI.enabled = true;
			sKISARep = GUILayout.TextField(sKISARep);
			GUILayout.Button("Boost with Rep");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("KISA Funding: ");
			GUI.enabled = false;
			GUILayout.TextField("0");
			GUI.enabled = true;
			GUILayout.Label("Monthly Costs: ");
			GUI.enabled = false;
			GUILayout.TextField("0");
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Funds: ");
			sKISAFunds = GUILayout.TextField(sKISAFunds);
			GUILayout.Button("Withdraw");
			GUILayout.Button("Deposit");
			GUILayout.EndHorizontal(); */
			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}
		#endregion

		#region Base Boss
		// BASE BOSS
		void drawBaseManagerWindow(int windowID)
		{
			string Base;
			float Range;
			LaunchSite lNearest;
			LaunchSite lBase;

			GUILayout.BeginArea(new Rect(10, 30, 380, 380));
				GUILayout.Space(3);
				GUILayout.Box("Settings");

				GUILayout.BeginHorizontal();
					KerbalKonstructs.instance.enableATC = GUILayout.Toggle(KerbalKonstructs.instance.enableATC, "Enable ATC", GUILayout.Width(175));
					KerbalKonstructs.instance.enableNGS = GUILayout.Toggle(KerbalKonstructs.instance.enableNGS, "Enable NGS", GUILayout.Width(175));
					KerbalKonstructs.instance.showNGS = (KerbalKonstructs.instance.enableNGS);
				GUILayout.EndHorizontal();

				GUILayout.Box("Base");

				if (isCareerGame())
				{
					GUILayout.BeginHorizontal();
						GUILayout.Label("Nearest Open Base: ", GUILayout.Width(100));
						LaunchSiteManager.getNearestOpenBase(FlightGlobals.ActiveVessel.GetTransform().position, out Base, out Range, out lNearest);
						GUILayout.Label(Base + " at ", GUILayout.Width(130));
						GUI.enabled = false;
						GUILayout.TextField(" " + Range + " ", GUILayout.Width(75));
						GUI.enabled = true;
						GUILayout.Label("m");
						if (KerbalKonstructs.instance.enableNGS)
						{
							if (GUILayout.Button("NGS",GUILayout.Height(21)))
							{
								setTargetSite(lNearest);
							}
						}
					GUILayout.EndHorizontal();

					GUILayout.Space(2);
				}

				GUILayout.BeginHorizontal();
					GUILayout.Label("Nearest Base: ", GUILayout.Width(100));
					LaunchSiteManager.getNearestBase(FlightGlobals.ActiveVessel.GetTransform().position, out Base, out Range, out lBase);
					GUILayout.Label(Base + " at ", GUILayout.Width(130));
					GUI.enabled = false;
					GUILayout.TextField(" " + Range + " ", GUILayout.Width(75));
					GUI.enabled = true;
					GUILayout.Label("m");
					if (KerbalKonstructs.instance.enableNGS)
					{
						if (GUILayout.Button("NGS", GUILayout.Height(21)))
						{
							setTargetSite(lBase);
						}
					}
				GUILayout.EndHorizontal();

				if (isCareerGame())
				{
					bool bLanded = (FlightGlobals.ActiveVessel.Landed);

					if (Range < 2000)
					{
						string sClosed;
						float fOpenCost;
						LaunchSiteManager.getSiteOpenCloseState(Base, out sClosed, out fOpenCost);
						fOpenCost = fOpenCost / 2f;

						if (bLanded && sClosed == "Closed")
						{
							if (GUILayout.Button("Open Base for " + fOpenCost + " Funds"))
							{
								double currentfunds = Funding.Instance.Funds;

								if (fOpenCost > currentfunds)
								{
									ScreenMessages.PostScreenMessage("Insufficient funds to open this site!", 10, 0);
								}
								else
								{
									// Charge some funds
									Funding.Instance.AddFunds(-fOpenCost, TransactionReasons.Cheating);

									// Open the site - save to instance
									LaunchSiteManager.setSiteOpenCloseState(Base, "Open");
								}
							}
						}

						if (bLanded && sClosed == "Open")
						{
							GUI.enabled = false;
							GUILayout.Button("Base is Open");
							GUI.enabled = true;
						}

						GUILayout.Space(2);
					}

					if (Range > 100000)
					{
						if (bLanded)
						{
							if (GUILayout.Button("Found a New Base"))
							{
								foundingBase = true;
							}
						}
					}
				}

				if (FlightGlobals.ActiveVessel.Landed)
				{
					GUILayout.Box("Facilities");

					scrollPos = GUILayout.BeginScrollView(scrollPos);
					foreach (StaticObject obj in KerbalKonstructs.instance.getStaticDB().getAllStatics())
					{
						bool isLocal = true;
						if (obj.pqsCity.sphere == FlightGlobals.currentMainBody.pqsController)
						{
							var dist = Vector3.Distance(FlightGlobals.ActiveVessel.GetTransform().position, obj.gameObject.transform.position);
							isLocal = dist < 2000f;
						}
						else
							isLocal = false;

						if (isLocal)
						{
							if (GUILayout.Button((string)obj.model.getSetting("title")))
							{
								KerbalKonstructs.instance.selectObject(obj, false);
								loadStaticPersistence(obj);
								managingFacility = true;
							}
						}
					}
					GUILayout.EndScrollView();

					GUILayout.Space(5);
				}
			GUILayout.EndArea();

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}
		#endregion

			#region NGS
		// NGS Handling
		public float fRangeToTarget = 0f;
		public bool bClosing = false;
		public int iCorrection = 3;

		public string sTargetSiteName = "NO TARGET";

		private float angle1, angle2;
		private float angle3, angle4;

		private Vector3 vCrft;
		private Vector3 vSPos;
		private Vector3 vHead;
		private Double disLat;
		private Double disLon;
		private Double disBaseLat;
		private Double disBaseLon;

		private double dshipheading;
		private double dreqheading;

		private static double GetLongitude(Vector3d radialPosition)
		{
			Vector3d norm = radialPosition.normalized;
			double longitude = Math.Atan2(norm.z, norm.x);
			return (!double.IsNaN(longitude) ? longitude : 0.0);
		}

		private static double GetLatitude(Vector3d radialPosition)
		{
			double latitude = Math.Asin(radialPosition.normalized.y);
			return (!double.IsNaN(latitude) ? latitude : 0.0);
		}

		public static void setTargetSite(LaunchSite lsTarget)
		{
			lTargetSite = lsTarget;
		}

		void prepNGS()
		{
			if (lTargetSite != null)
			{
				sTargetSiteName = lTargetSite.name;
				fRangeToTarget = LaunchSiteManager.getDistanceToBase(FlightGlobals.ActiveVessel.GetTransform().position, lTargetSite);
				if (fRangeToTarget > fOldRange) bClosing = false;
				if (fRangeToTarget < fOldRange) bClosing = true;

				var basepos = KerbalKonstructs.instance.getCurrentBody().transform.InverseTransformPoint(lTargetSite.GameObject.transform.position);
				var dBaseLat = GetLatitude(basepos);
				var dBaseLon = GetLongitude(basepos);
				disBaseLat = dBaseLat * 180 / Math.PI;
				disBaseLon = dBaseLon * 180 / Math.PI;

				fOldRange = fRangeToTarget;

				if (bClosing)
				{
					tTextureMiddle = tIconOpen;
				}
				else
				{
					tTextureMiddle = tIconClosed;
				}

				Vector3 vcraftpos = FlightGlobals.ActiveVessel.GetTransform().position;
				vCrft = vcraftpos;
				Vector3 vsitepos = lTargetSite.GameObject.transform.position;
				vSPos = vsitepos;
				Vector3 vHeading = FlightGlobals.ActiveVessel.transform.up;
				vHead = vHeading;

				disLat = FlightGlobals.ActiveVessel.latitude;
				var dLat = disLat / 180 * Math.PI;
				disLon = FlightGlobals.ActiveVessel.longitude;
				var dLon = disLon / 180 * Math.PI;

				var y = Math.Sin(dBaseLon - dLon) * Math.Cos(dBaseLat);
				var x = (Math.Cos(dLat) * Math.Sin(dBaseLat)) - (Math.Sin(dLat) * Math.Cos(dBaseLat) * Math.Cos(dBaseLon - dLon));
				var requiredHeading = Math.Atan2(y, x) * 180 / Math.PI;
				dreqheading = (requiredHeading + 360) % 360;

				var diff = (360 + 180 + requiredHeading - FlightGlobals.ship_heading) % 360 - 180;
				dshipheading = (FlightGlobals.ship_heading + 360) % 360;

				if (diff > 5)
					iCorrection = 2;
				else if (diff < -5)
					iCorrection = 1;
				else
					iCorrection = 0;

				if (bClosing)
				{
					tTextureLeft = tLeftOff;
					tTextureRight = tRightOff;
				}
				else
				{
					tTextureLeft = tLeftOn;
					tTextureRight = tRightOn;
				}

				if (iCorrection == 1)
				{
					tTextureLeft = tLeftOn;
					tTextureRight = tRightOff;
				}
				if (iCorrection == 2)
				{
					tTextureLeft = tLeftOff;
					tTextureRight = tRightOn;
				}
			}
			else
			{
				tTextureMiddle = tIconClosed;
				tTextureLeft = tLeftOff;
				tTextureRight = tRightOff;
			}
		}

		// NGS
		void drawNGSWindow(int windowID)
		{
			GUILayout.BeginHorizontal();
				GUILayout.Box(sTargetSiteName, GUILayout.Height(20));
				if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(15)))
				{
					KerbalKonstructs.instance.enableNGS = false;
					KerbalKonstructs.instance.showNGS = false;
				}
			GUILayout.EndHorizontal();
			
			GUILayout.Box(fRangeToTarget + " m", GUILayout.Height(20));

			GUILayout.BeginHorizontal();
				GUILayout.Box(tTextureLeft, GUILayout.Height(25), GUILayout.Width(155));
				GUILayout.Box(tTextureMiddle, GUILayout.Height(25), GUILayout.Width(25));
				GUILayout.Box(tTextureRight, GUILayout.Height(25), GUILayout.Width(155));			
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Box("CRFT", GUILayout.Width(60), GUILayout.Height(20));
				GUILayout.Box("Lat.", GUILayout.Height(20));
				GUILayout.Box(disLat.ToString("#0"), GUILayout.Width(35), GUILayout.Height(20));
				GUILayout.Box("Lon.", GUILayout.Height(20));
				GUILayout.Box(disLon.ToString("#0"), GUILayout.Width(35), GUILayout.Height(20));
				GUILayout.Box("Head", GUILayout.Height(20));
				GUILayout.Box(dshipheading.ToString("#0"), GUILayout.Width(35), GUILayout.Height(20));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Box("BASE", GUILayout.Width(60), GUILayout.Height(20));
				GUILayout.Box("Lat.", GUILayout.Height(20));
				GUILayout.Box(disBaseLat.ToString("#0"), GUILayout.Width(35), GUILayout.Height(20));
				GUILayout.Box("Lon.", GUILayout.Height(20));
				GUILayout.Box(disBaseLon.ToString("#0"), GUILayout.Width(35), GUILayout.Height(20));
				GUILayout.Box("Head", GUILayout.Height(20));
				GUILayout.Box(dreqheading.ToString("#0"), GUILayout.Width(35), GUILayout.Height(20));
			GUILayout.EndHorizontal();

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));

			prepNGS();
		}
		#endregion

			#region Facility Manager
		// BASE BOSS FACILITY MANAGER
		void drawFacilityManagerWindow(int windowID)
		{
			string sFacilityName = (string)selectedObject.model.getSetting("title");
			string sFacilityRole = (string)selectedObject.model.getSetting("FacilityRole");

			float fStaffMax = (float)selectedObject.model.getSetting("StaffMax");
			float fStaffCurrent = (float)selectedObject.getSetting("StaffCurrent");

			float fScienceOMax = (float)selectedObject.model.getSetting("ScienceOMax");
			float fScienceOCurrent = (float)selectedObject.getSetting("ScienceOCurrent");

			float fFundsOMax = (float)selectedObject.model.getSetting("FundsOMax");
			float fFundsOCurrent = (float)selectedObject.getSetting("FundsOCurrent");

			fLqFMax = (float)selectedObject.model.getSetting("LqFMax");
			fLqFCurrent = (float)selectedObject.getSetting("LqFCurrent");
			fOxFMax = (float)selectedObject.model.getSetting("OxFMax");
			fOxFCurrent = (float)selectedObject.getSetting("OxFCurrent");
			fMoFMax = (float)selectedObject.model.getSetting("MoFMax");
			fMoFCurrent = (float)selectedObject.getSetting("MoFCurrent");

			float fPurchaseRate = fTransferRate * 100f;

			GUILayout.Box(sFacilityName);

			if (!FlightGlobals.ActiveVessel.Landed)
			{
				GUILayout.Box("VESSEL MUST BE LANDED TO USE THIS FACILITY!");
				LockFuelTank();
			}

			var vDist = Vector3.Distance(selectedObject.gameObject.transform.position, FlightGlobals.ActiveVessel.transform.position);

			if ((double)vDist < KerbalKonstructs.instance.facilityUseRange)
			{}
			else
			{
				GUILayout.Box("VESSEL MUST BE IN RANGE TO USE THIS FACILITY!");
				LockFuelTank();
			}

			if (fLqFMax > 0)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label("Max LiquidFuel");
					GUI.enabled = false;
					GUILayout.TextField(string.Format("{0}", fLqFMax));
					GUI.enabled = true;
					GUILayout.Label("Current LiquidFuel");
					GUI.enabled = false;
					GUILayout.TextField(fLqFCurrent.ToString("#0.00"));
					GUI.enabled = true;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					if (GUILayout.Button("Order LiquidFuel"))
					{
						LockFuelTank();
						saveStaticPersistence(selectedObject);
						bOrderedLqF = true;
					}
					GUI.enabled = !bLqFIn;
					if (GUILayout.Button("Transfer In"))
					{
						bLqFIn = true;
						bLqFOut = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = !bLqFOut;
					if (GUILayout.Button("Transfer Out"))
					{
						bLqFOut = true;
						bLqFIn = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = bLqFIn || bLqFOut;
					if (GUILayout.Button("Stop"))
					{
						bLqFIn = false;
						bLqFOut = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = true;
				GUILayout.EndHorizontal();
			}

			if (bOrderedLqF)
			{
				GUILayout.BeginHorizontal();
					if (GUILayout.RepeatButton("-"))
					{
						fLqFAmount = (float.Parse(fLqFAmount)  -fPurchaseRate).ToString();
						if ((float.Parse(fLqFAmount)) < 0f) fLqFAmount = "0.00";
					}
					GUILayout.TextField(fLqFAmount);
					if (GUILayout.RepeatButton("+"))
					{
						fLqFAmount = (float.Parse(fLqFAmount) +fPurchaseRate).ToString();
						if ((float.Parse(fLqFAmount)) > (fLqFMax - fLqFCurrent)) fLqFAmount = (fLqFMax - fLqFCurrent).ToString();
					}

					if (GUILayout.Button("Max"))
					{
						fLqFAmount = (fLqFMax - fLqFCurrent).ToString();
						if ((float.Parse(fLqFAmount)) < 0f) fLqFAmount = "0.00";
						saveStaticPersistence(selectedObject);
					}

					float flqFPrice = 0.5f;

					float fLqFCost = (float.Parse(fLqFAmount)) * flqFPrice;
					GUILayout.Label("Cost: " + fLqFCost + " \\F");
					if (GUILayout.Button("Buy"))
					{
						if ((float)selectedObject.getSetting("LqFCurrent") + (float.Parse(fLqFAmount)) > fLqFMax)
						{
							ScreenMessages.PostScreenMessage("Insufficient fuel capacity!", 10, 0);
							fLqFAmount = "0.00";
						}
						else
						{
							double currentfunds = Funding.Instance.Funds;

							if (fLqFCost > currentfunds)
							{
								ScreenMessages.PostScreenMessage("Insufficient funds!", 10, 0);
							}
							else
							{
								Funding.Instance.AddFunds(-fLqFCost, TransactionReasons.Cheating);
								selectedObject.setSetting("LqFCurrent", (float)selectedObject.getSetting("LqFCurrent") + (float.Parse(fLqFAmount)));
							}
						}

						saveStaticPersistence(selectedObject);
					}
					if (GUILayout.Button("Done"))
					{
						saveStaticPersistence(selectedObject);
						bOrderedLqF = false;
					}
				GUILayout.EndHorizontal();
			}

			if (fOxFMax > 0)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label("Max Oxidizer");
					GUI.enabled = false;
					GUILayout.TextField(string.Format("{0}", fOxFMax));
					GUI.enabled = true;
					GUILayout.Label("Current Oxidizer");
					GUI.enabled = false;
					GUILayout.TextField(fOxFCurrent.ToString("#0.00"));
					GUI.enabled = true;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					if (GUILayout.Button("Order Oxidizer"))
					{
						LockFuelTank();
						saveStaticPersistence(selectedObject);
						bOrderedOxF = true;
					}
					GUI.enabled = !bOxFIn;
					if (GUILayout.Button("Transfer In"))
					{
						bOxFIn = true;
						bOxFOut = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = !bOxFOut;
					if (GUILayout.Button("Transfer Out"))
					{
						bOxFOut = true;
						bOxFIn = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = bOxFIn || bOxFOut;
					if (GUILayout.Button("Stop"))
					{
						bOxFIn = false;
						bOxFOut = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = true;
				GUILayout.EndHorizontal();
			}

			if (bOrderedOxF)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.RepeatButton("-"))
				{
					fOxFAmount = (float.Parse(fOxFAmount) -fPurchaseRate).ToString();
					if ((float.Parse(fOxFAmount)) < 0f) fOxFAmount = "0.00";
				}
				GUILayout.TextField(fOxFAmount);
				if (GUILayout.RepeatButton("+"))
				{
					fOxFAmount = (float.Parse(fOxFAmount) + fPurchaseRate).ToString();
					if ((float.Parse(fOxFAmount)) > (fOxFMax - fOxFCurrent)) fOxFAmount = (fOxFMax - fOxFCurrent).ToString();
				}

				if (GUILayout.Button("Max"))
				{
					fOxFAmount = (fOxFMax - fOxFCurrent).ToString();
					if ((float.Parse(fOxFAmount)) < 0f) fOxFAmount = "0.00";
					saveStaticPersistence(selectedObject);
				}

				float fOxFPrice = 1.5f;

				float fOxFCost = (float.Parse(fOxFAmount)) * fOxFPrice;
				GUILayout.Label("Cost: " + fOxFCost + " \\F");
				if (GUILayout.Button("Buy"))
				{
					if ((float)selectedObject.getSetting("OxFCurrent") + (float.Parse(fOxFAmount)) > fOxFMax)
					{
						ScreenMessages.PostScreenMessage("Insufficient fuel capacity!", 10, 0);
						fOxFAmount = "0.00";
					}
					else
					{
						double currentfunds = Funding.Instance.Funds;

						if (fOxFCost > currentfunds)
						{
							ScreenMessages.PostScreenMessage("Insufficient funds!", 10, 0);
						}
						else
						{
							Funding.Instance.AddFunds(-fOxFCost, TransactionReasons.Cheating);
							selectedObject.setSetting("OxFCurrent", (float)selectedObject.getSetting("OxFCurrent") + (float.Parse(fOxFAmount)));
						}
					}

					saveStaticPersistence(selectedObject);
				}
				if (GUILayout.Button("Done"))
				{
					saveStaticPersistence(selectedObject);
					bOrderedOxF = false;
				}
				GUILayout.EndHorizontal();
			}

			if (fMoFMax > 0)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label("Max MonoProp.");
					GUI.enabled = false;
					GUILayout.TextField(string.Format("{0}", fMoFMax));
					GUI.enabled = true;
					GUILayout.Label("Current MonoProp.");
					GUI.enabled = false;
					GUILayout.TextField(fMoFCurrent.ToString("#0.00"));
					GUI.enabled = true;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					if (GUILayout.Button("Order MonoProp."))
					{
						LockFuelTank();
						saveStaticPersistence(selectedObject);
						bOrderedMoF = true;
					}
					GUI.enabled = !bMoFIn;
					if (GUILayout.Button("Transfer In"))
					{
						bMoFIn = true;
						bMoFOut = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = !bMoFOut;
					if (GUILayout.Button("Transfer Out"))
					{
						bMoFOut = true;
						bMoFIn = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = bMoFIn || bMoFOut;
					if (GUILayout.Button("Stop"))
					{
						bMoFIn = false;
						bMoFOut = false;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = true;
				GUILayout.EndHorizontal();
			}

			if (bOrderedMoF)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.RepeatButton("-"))
				{
					fMoFAmount = (float.Parse(fMoFAmount) -fPurchaseRate).ToString();
					if ((float.Parse(fMoFAmount)) < 0f) fMoFAmount = "0.00";
				}
				GUILayout.TextField(fMoFAmount);
				if (GUILayout.RepeatButton("+"))
				{
					fMoFAmount = (float.Parse(fMoFAmount) +fPurchaseRate).ToString();
					if ((float.Parse(fMoFAmount)) > (fMoFMax - fMoFCurrent)) fMoFAmount = (fMoFMax - fMoFCurrent).ToString();
				}

				if (GUILayout.Button("Max"))
				{
					fMoFAmount = (fMoFMax - fMoFCurrent).ToString();
					if ((float.Parse(fMoFAmount)) < 0f) fMoFAmount = "0.00";
					saveStaticPersistence(selectedObject);
				}

				float fMoFPrice = 1.2f;

				float fMoFCost = (float.Parse(fMoFAmount)) * fMoFPrice;
				GUILayout.Label("Cost: " + fMoFCost + " \\F");
				if (GUILayout.Button("Buy"))
				{
					if ((float)selectedObject.getSetting("MoFCurrent") + (float.Parse(fMoFAmount)) > fMoFMax)
					{
						ScreenMessages.PostScreenMessage("Insufficient fuel capacity!", 10, 0);
						fMoFAmount = "0.00";
					}
					else
					{
						double currentfunds = Funding.Instance.Funds;

						if (fMoFCost > currentfunds)
						{
							ScreenMessages.PostScreenMessage("Insufficient funds!", 10, 0);
						}
						else
						{
							Funding.Instance.AddFunds(-fMoFCost, TransactionReasons.Cheating);
							selectedObject.setSetting("MoFCurrent", (float)selectedObject.getSetting("MoFCurrent") + (float.Parse(fMoFAmount)));
						}
					}

					saveStaticPersistence(selectedObject);
				}
				if (GUILayout.Button("Done"))
				{
					saveStaticPersistence(selectedObject);
					bOrderedMoF = false;
				}
				GUILayout.EndHorizontal();
			}

			if (fOxFMax > 0 || fLqFMax > 0 || fMoFMax > 0)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label("Active Vessel");
					GUILayout.Box("" + FlightGlobals.ActiveVessel.vesselName, GUILayout.Height(25));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label("Transfer Rate");

					GUI.enabled = (fTransferRate != 0.01f);
					if (GUILayout.Button("x1"))
					{
						fTransferRate = 0.01f;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = (fTransferRate != 0.04f);
					if (GUILayout.Button("x4"))
					{
						fTransferRate = 0.04f;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = (fTransferRate != 0.1f);
					if (GUILayout.Button("x10"))
					{
						fTransferRate = 0.1f;
						saveStaticPersistence(selectedObject);
					}
					GUI.enabled = true;
				GUILayout.EndHorizontal();

				if (!FlightGlobals.ActiveVessel.isEVA && FlightGlobals.ActiveVessel.Landed)
				{
					GUILayout.Label("Vessel's Tanks");

					scrollPos3 = GUILayout.BeginScrollView(scrollPos3);
						foreach (Part fTank in FlightGlobals.ActiveVessel.parts)
						{
							foreach (PartResource rResource in fTank.Resources)
							{
								if (rResource.resourceName == "LiquidFuel" || rResource.resourceName == "Oxidizer" || rResource.resourceName == "MonoPropellant")
								{
									if (SelectedTank == fTank && SelectedResource == rResource)
										PartSelected = true;
									else
										PartSelected = false;

									GUILayout.BeginHorizontal();
										GUILayout.Box("" + fTank.gameObject.name);
										GUILayout.Box("" + rResource.resourceName);
									GUILayout.EndHorizontal();
								
									GUILayout.BeginHorizontal();
										GUILayout.Label("Fuel");
										GUILayout.TextField("" + rResource.amount.ToString("#0.00"));

										GUI.enabled = !PartSelected;
										if (GUILayout.Button("Select"))
										{
											SelectedResource = rResource;
											SelectedTank = fTank;
											saveStaticPersistence(selectedObject);
										}

										GUI.enabled = PartSelected;
										if (GUILayout.Button("Deselect"))
										{
											SelectedResource = null;
											SelectedTank = null;
											saveStaticPersistence(selectedObject);
										}
										GUI.enabled = true;
									GUILayout.EndHorizontal();
								}
								else
									continue;
							}
						}
					GUILayout.EndScrollView();

					GUI.enabled = true;

					if (SelectedResource != null && SelectedTank != null)
					{
						// Debug.Log("KK: doFuelIn or doFuelOut");
						if (bMoFOut || bOxFOut || bLqFOut)
							doFuelOut();
						if (bMoFIn || bOxFIn || bLqFIn)
							doFuelIn();
					}
				}
			}

			/* if (fScienceOMax > 0)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label("Max Science");
					GUI.enabled = false;
					GUILayout.TextField(string.Format("{0}", fScienceOMax));
					GUI.enabled = true;
					GUILayout.Space(20);
					GUILayout.Label("Current Science");
					GUI.enabled = false;
					GUILayout.TextField(string.Format("{0}", fScienceOCurrent));
					GUI.enabled = true;
				GUILayout.EndHorizontal();

				GUILayout.Button("Claim Science");
			}

			if (fFundsOMax > 0)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Max Funds");
				GUI.enabled = false;
				GUILayout.TextField(string.Format("{0}", fFundsOMax));
				GUI.enabled = true;
				GUILayout.Space(20);
				GUILayout.Label("Current Funds");
				GUI.enabled = false;
				GUILayout.TextField(string.Format("{0}", fFundsOCurrent));
				GUI.enabled = true;
				GUILayout.EndHorizontal();

				GUILayout.Button("Claim Funds");
			}

			if (fStaffMax > 0)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label("Max Staff");
					GUI.enabled = false;
					GUILayout.TextField(string.Format("{0}", fStaffMax));
					GUI.enabled = true;
					GUILayout.Space(20);
					GUILayout.Label("Current Staff");
					GUI.enabled = false;
					GUILayout.TextField(string.Format("{0}", fStaffCurrent));
					GUI.enabled = true;
				GUILayout.EndHorizontal();

				double dHiring = KerbalKonstructs.instance.staffHireCost;
				double dRepMultiplier = KerbalKonstructs.instance.staffRepRequirementMultiplier;

				GUILayout.Label("To hire a kerbal costs " + dHiring + " Funds and requires Rep equal to the current number of staff x " + dRepMultiplier + ". Firing a kerbal costs nothing.");

				GUILayout.BeginHorizontal();
					GUI.enabled = (fStaffCurrent < fStaffMax);
					if (GUILayout.Button("Hire 1"))
					{
						double dFunds = Funding.Instance.Funds;
						double dRep = Reputation.Instance.reputation;

						if (dFunds < dHiring)
						{
							ScreenMessages.PostScreenMessage("Insufficient funds to hire more staff!", 10, 0);
						}
						else
							if (dRep < (fStaffCurrent*dRepMultiplier))
							{
								ScreenMessages.PostScreenMessage("Insufficient rep to hire more staff!", 10, 0);
							}
							else
							{
								selectedObject.setSetting("StaffCurrent", fStaffCurrent + 1);
								Funding.Instance.AddFunds(-dHiring, TransactionReasons.Cheating);
							}
					}
					GUI.enabled = true;
					GUI.enabled = (fStaffCurrent > 0);
					if (GUILayout.Button("Fire 1"))
					{
						selectedObject.setSetting("StaffCurrent", fStaffCurrent - 1);
					}
					GUI.enabled = true;
				GUILayout.EndHorizontal();
			} */

			GUILayout.Space(10);

			if (GUILayout.Button("Close"))
			{
				LockFuelTank();
				managingFacility = false;
				Debug.Log("KK: saveStaticPersistence");
				saveStaticPersistence(selectedObject);
				KerbalKonstructs.instance.deselectObject();
			}

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}
		#endregion

				#region Fuel Tanks
		// Working facilities handling
		Boolean bLqFIn = false;
		Boolean bLqFOut = false;
		Boolean bOxFIn = false;
		Boolean bOxFOut = false;
		Boolean bMoFIn = false;
		Boolean bMoFOut = false;
		Boolean PartSelected = false;

		Boolean bOrderedLqF = false;
		Boolean bOrderedOxF = false;
		Boolean bOrderedMoF = false;

		Vessel CurrentVessel = null;
		public PartResource SelectedResource = null;
		public Part SelectedTank = null;

		float fLqFMax = 0;
		float fLqFCurrent = 0;
		float fOxFMax = 0;
		float fOxFCurrent = 0;
		float fMoFMax = 0;
		float fMoFCurrent = 0;

		float fTransferRate = 0.01f;

		string fOxFAmount = "0.00";
		string fLqFAmount = "0.00";
		string fMoFAmount = "0.00";

		void doFuelOut()
		{
			if (SelectedResource == null) return;
			if (SelectedTank == null) return;

			// Debug.Log("KK: doFuelOut " + SelectedResource.resourceName);

			if (SelectedResource.resourceName == "MonoPropellant" && !bMoFOut) return;
			if (SelectedResource.resourceName == "LiquidFuel" && !bLqFOut) return;
			if (SelectedResource.resourceName == "Oxidizer" && !bOxFOut) return;

			if (SelectedResource.resourceName == "MonoPropellant" && fMoFCurrent <= 0) return;
			if (SelectedResource.resourceName == "LiquidFuel" && fLqFCurrent <= 0) return;
			if (SelectedResource.resourceName == "Oxidizer" && fOxFCurrent <= 0) return;

			if (SelectedResource.amount >= SelectedResource.maxAmount) return;

			float dStaticFuel;

			// Debug.Log("KK: doFuelOut " + SelectedResource.resourceName);

			SelectedResource.amount = SelectedResource.amount + fTransferRate;
			if (SelectedResource.amount > SelectedResource.maxAmount) SelectedResource.amount = SelectedResource.maxAmount;

			if (SelectedResource.resourceName == "MonoPropellant")
			{
				dStaticFuel = ((float)selectedObject.getSetting("MoFCurrent")) - fTransferRate;
				if (dStaticFuel < 0) dStaticFuel = 0;
				selectedObject.setSetting("MoFCurrent", dStaticFuel);
			}
			if (SelectedResource.resourceName == "LiquidFuel")
			{
				dStaticFuel = ((float)selectedObject.getSetting("LqFCurrent")) - fTransferRate;
				if (dStaticFuel < 0) dStaticFuel = 0;
				selectedObject.setSetting("LqFCurrent", dStaticFuel);
			}
			if (SelectedResource.resourceName == "Oxidizer")
			{
				dStaticFuel = ((float)selectedObject.getSetting("OxFCurrent")) - fTransferRate;
				if (dStaticFuel < 0) dStaticFuel = 0;
				selectedObject.setSetting("OxFCurrent", dStaticFuel);
			}
		}

		void doFuelIn()
		{
			if (SelectedResource == null) return;
			if (SelectedTank == null) return;

			// Debug.Log("KK: doFuelIn " + SelectedResource.resourceName);

			if (SelectedResource.resourceName == "MonoPropellant" && !bMoFIn) return;
			if (SelectedResource.resourceName == "LiquidFuel" && !bLqFIn) return;
			if (SelectedResource.resourceName == "Oxidizer" && !bOxFIn) return;

			if (SelectedResource.resourceName == "MonoPropellant" && fMoFCurrent >= fMoFMax) return;
			if (SelectedResource.resourceName == "LiquidFuel" && fLqFCurrent >= fLqFMax) return;
			if (SelectedResource.resourceName == "Oxidizer" && fOxFCurrent >= fOxFMax) return;

			if (SelectedResource.amount <= 0) return;

			// Debug.Log("KK: doFuelIn " + SelectedResource.amount);

			float dStaticFuel;

			SelectedResource.amount = SelectedResource.amount - fTransferRate;
			if (SelectedResource.amount < 0) SelectedResource.amount = 0;

			if (SelectedResource.resourceName == "MonoPropellant")
			{
				dStaticFuel = ((float)selectedObject.getSetting("MoFCurrent")) + fTransferRate;
				if (dStaticFuel > fMoFMax) dStaticFuel = fMoFMax;
				selectedObject.setSetting("MoFCurrent", dStaticFuel);
			}
			if (SelectedResource.resourceName == "LiquidFuel")
			{
				dStaticFuel = ((float)selectedObject.getSetting("LqFCurrent")) + fTransferRate;
				if (dStaticFuel > fLqFMax) dStaticFuel = fLqFMax;
				selectedObject.setSetting("LqFCurrent", dStaticFuel);
			}
			if (SelectedResource.resourceName == "Oxidizer")
			{
				dStaticFuel = ((float)selectedObject.getSetting("OxFCurrent")) + fTransferRate;
				if (dStaticFuel > fOxFMax) dStaticFuel = fOxFMax;
				selectedObject.setSetting("OxFCurrent", dStaticFuel);
			}
		}

		void LockFuelTank()
		{
			SelectedResource = null;
			SelectedTank = null;
			bLqFIn = false;
			bLqFOut = false;
			bOxFIn = false;
			bOxFOut = false;
			bMoFIn = false;
			bMoFOut = false;
		}
		#endregion

		#region Editors

			#region Statics Editor
		// STATICS EDITOR
		void drawEditorWindow(int id)
		{
			GUILayout.BeginArea(new Rect(10, 25, 500, 485));
			GUILayout.BeginHorizontal();
				GUI.enabled = !creatingInstance;
				if (GUILayout.Button("Spawn New", GUILayout.Width(115)))
				{
					creatingInstance = true;
					showLocal = false;
				}
				GUILayout.Space(10);
				GUI.enabled = creatingInstance || showLocal;
				if (GUILayout.Button("All Instances", GUILayout.Width(108)))
				{
					creatingInstance = false;
					showLocal = false;
				}
				GUI.enabled = true;
				GUILayout.Space(2);
				GUI.enabled = creatingInstance || !showLocal;
				if (GUILayout.Button("Local Instances", GUILayout.Width(108)))
				{
					creatingInstance = false;
					showLocal = true;
				}
				GUI.enabled = true;
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Save Objects", GUILayout.Width(115)))
					KerbalKonstructs.instance.saveObjects();
			GUILayout.EndHorizontal();

			scrollPos = GUILayout.BeginScrollView(scrollPos);
				if (creatingInstance)
				{
					foreach (StaticModel model in KerbalKonstructs.instance.getStaticDB().getModels())
					{
						if (GUILayout.Button(model.getSetting("title") + " : " + model.getSetting("mesh")))
						{
							spawnInstance(model);
						}
					}
				}

				if (!creatingInstance)
				{
					foreach (StaticObject obj in KerbalKonstructs.instance.getStaticDB().getAllStatics())
					{
						bool isLocal = true;

						if (showLocal)
						{
							if (obj.pqsCity.sphere == FlightGlobals.currentMainBody.pqsController)
							{
								var dist = Vector3.Distance(FlightGlobals.ActiveVessel.GetTransform().position, obj.gameObject.transform.position);
								isLocal = dist < 10000f;
							}
							else
								isLocal = false;
						}
							
						if (isLocal)
						{
							if (GUILayout.Button("[" + obj.getSetting("Group") + "] " + (obj.settings.ContainsKey("LaunchSiteName") ? obj.getSetting("LaunchSiteName") + " : " + obj.model.getSetting("title") : obj.model.getSetting("title"))))
							{
								enableColliders = true;
								KerbalKonstructs.instance.selectObject(obj, false);
							}
						}
					}
				}
			GUILayout.EndScrollView();
			GUI.enabled = true;
				
			// Set locals to group function
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label("Group:");
				GUILayout.Space(5);
				GUI.enabled = showLocal;
				customgroup = GUILayout.TextField(customgroup, 25, GUILayout.Width(150));
				GUI.enabled = true;
				GUILayout.Space(5);
				GUI.enabled = showLocal;
				if (GUILayout.Button("Set as Group", GUILayout.Width(115)))
				{
					setLocalsGroup(customgroup);
				}
				GUI.enabled = true;
			GUILayout.EndHorizontal();

			GUILayout.EndArea();

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		// Set locals to group function
		void setLocalsGroup(string sGroup)
		{
			if (sGroup == "")
				return;

			foreach (StaticObject obj in KerbalKonstructs.instance.getStaticDB().getAllStatics())
			{
				if (obj.pqsCity.sphere == FlightGlobals.currentMainBody.pqsController)
				{
					var dist = Vector3.Distance(FlightGlobals.ActiveVessel.GetTransform().position, obj.gameObject.transform.position);
					if (dist < 10000f)
					{
						KerbalKonstructs.instance.getStaticDB().changeGroup(obj, sGroup);
					}
				}
			}
		}
		#endregion

			#region Instance Editor
		// Instance Editor handlers
		string savedxpos = "";
		string savedypos = "";
		string savedzpos = "";
		string savedalt = "";
		string savedrot = "";
		bool savedpos = false;
		bool pospasted = false;

		// INSTANCE EDITOR
		void drawToolWindow(int windowID)
		{
			Vector3 position = Vector3.zero;
			float alt = 0;
			float newRot = 0;
			float vis = 0;
			bool shouldUpdateSelection = false;
			bool manuallySet = false;

			GUILayout.BeginArea(new Rect(5, 25, 295, 425));

			GUILayout.Box((string)selectedObject.model.getSetting("title"));
			GUILayout.Label("Hit enter after typing a value to apply.");

				GUILayout.BeginHorizontal();
					GUILayout.Label("Position   ");
					GUILayout.Space(15);
					if (GUILayout.Button(tCopyPos, GUILayout.Width(23), GUILayout.Height(23)))
					{
						savedpos = true;
						savedxpos = xPos;
						savedypos = yPos;
						savedzpos = zPos;
						savedalt = altitude;
						savedrot = rotation;
						// Debug.Log("KK: Instance position copied");
					}
					if (GUILayout.Button(tPastePos, GUILayout.Width(23), GUILayout.Height(23)))
					{
						if (savedpos)
						{
							pospasted = true;
							xPos = savedxpos;
							yPos = savedypos;
							zPos = savedzpos;
							altitude = savedalt;
							rotation = savedrot;
							// Debug.Log("KK: Instance position pasted");
						}
					}
					GUILayout.FlexibleSpace();
					GUILayout.Label("Increment");
					increment = GUILayout.TextField(increment, 5, GUILayout.Width(50));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label("X:");
					GUILayout.FlexibleSpace();
					xPos = GUILayout.TextField(xPos, 25, GUILayout.Width(80));
					GUI.enabled = true;
					if (GUILayout.RepeatButton("<<", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(21)))
					{
						position.x -= float.Parse(increment);
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.RepeatButton(">>", GUILayout.Width(30), GUILayout.Height(21)))
					{
						position.x += float.Parse(increment);
						shouldUpdateSelection = true;
					}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label("Y:");
					GUILayout.FlexibleSpace();
					yPos = GUILayout.TextField(yPos, 25, GUILayout.Width(80));
					GUI.enabled = true;
					if (GUILayout.RepeatButton("<<", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(21)))
					{
						position.y -= float.Parse(increment);
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.RepeatButton(">>", GUILayout.Width(30), GUILayout.Height(21)))
					{
						position.y += float.Parse(increment);
						shouldUpdateSelection = true;
					}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label("Z:");
					GUILayout.FlexibleSpace();
					zPos = GUILayout.TextField(zPos, 25, GUILayout.Width(80));
					GUI.enabled = true;
					if (GUILayout.RepeatButton("<<", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(21)))
					{
						position.z -= float.Parse(increment);
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.RepeatButton(">>", GUILayout.Width(30), GUILayout.Height(21)))
					{
						position.z += float.Parse(increment);
						shouldUpdateSelection = true;
					}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label("Alt.");
					GUILayout.FlexibleSpace();
					altitude = GUILayout.TextField(altitude, 25, GUILayout.Width(80));
					GUI.enabled = true;
					if (GUILayout.RepeatButton("<<", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(21)))
					{
						alt -= float.Parse(increment);
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(21)) || GUILayout.RepeatButton(">>", GUILayout.Width(30), GUILayout.Height(21)))
					{
						alt += float.Parse(increment);
						shouldUpdateSelection = true;
					}
				GUILayout.EndHorizontal();

				var pqsc = ((CelestialBody)selectedObject.getSetting("CelestialBody")).pqsController;
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Snap to Terrain", GUILayout.Width(130), GUILayout.Height(21)))
					{
						alt = 1.0f + ((float)(pqsc.GetSurfaceHeight((Vector3)selectedObject.getSetting("RadialPosition")) - pqsc.radius - (float)selectedObject.getSetting("RadiusOffset")));
						shouldUpdateSelection = true;
					}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label("Rot.");
					GUILayout.FlexibleSpace();
					rotation = GUILayout.TextField(rotation, 4, GUILayout.Width(80));

					if (GUILayout.RepeatButton("<<", GUILayout.Width(30), GUILayout.Height(23)))
					{
						newRot -= 1.0f;
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(23)))
					{
						newRot -= float.Parse(increment) / 10f;
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(23)))
					{
						newRot += float.Parse(increment) / 10f;
						shouldUpdateSelection = true;
					}
					if (GUILayout.RepeatButton(">>", GUILayout.Width(30), GUILayout.Height(23)))
					{
						newRot += 1.0f;
						shouldUpdateSelection = true;
					}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Label("Vis.");
					GUILayout.FlexibleSpace();
					visrange = GUILayout.TextField(visrange, 6, GUILayout.Width(80));
					if (GUILayout.Button("Min", GUILayout.Width(30), GUILayout.Height(23)))
					{
						vis -= 100000f;
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(23)))
					{
						vis -= 2500f;
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(23)))
					{
						vis += 2500f;
						shouldUpdateSelection = true;
					}
					if (GUILayout.Button("Max", GUILayout.Width(30), GUILayout.Height(23)))
					{
						vis += 100000f;
						shouldUpdateSelection = true;
					}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
					enableColliders = GUILayout.Toggle(enableColliders, "Enable Colliders", GUILayout.Width(140));

					Transform[] gameObjectList = selectedObject.gameObject.GetComponentsInChildren<Transform>();
					List<GameObject> colliderList = (from t in gameObjectList where t.gameObject.collider != null select t.gameObject).ToList();

					if (enableColliders)
					{
						foreach (GameObject collider in colliderList)
						{
							collider.collider.enabled = true;
						}
					}
					if (!enableColliders)
					{
						foreach (GameObject collider in colliderList)
						{
							collider.collider.enabled = false;
						}
					}

					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Duplicate", GUILayout.Width(130)))
					{
						KerbalKonstructs.instance.saveObjects();
						StaticModel oModel = selectedObject.model;
						float fOffset = (float)selectedObject.getSetting("RadiusOffset");
						Vector3 vPosition = (Vector3)selectedObject.getSetting("RadialPosition");
						float fAngle = (float)selectedObject.getSetting("RotationAngle");
						KerbalKonstructs.instance.deselectObject();
						spawnInstance(oModel, fOffset, vPosition, fAngle);
					}
				GUILayout.EndHorizontal();

				GUI.enabled = !editingSite;

				string sLaunchPadTransform = (string)selectedObject.getSetting("LaunchPadTransform");
				string sDefaultPadTransform = (string)selectedObject.model.getSetting("DefaultLaunchPadTransform");
				string sLaunchsiteDesc = (string)selectedObject.getSetting("LaunchSiteDescription");
				string sModelDesc = (string)selectedObject.model.getSetting("description");

				if (sLaunchPadTransform == "" && sDefaultPadTransform == "")
					GUI.enabled = false;

				if (GUILayout.Button(((selectedObject.settings.ContainsKey("LaunchSiteName")) ? "Edit" : "Make") + " Launchsite"))
				{
					// Edit or make a launchsite
					siteName = (string)selectedObject.getSetting("LaunchSiteName");
					siteTrans = (selectedObject.settings.ContainsKey("LaunchPadTransform")) ? sLaunchPadTransform : sDefaultPadTransform;
						
					if (sLaunchsiteDesc != "")
						siteDesc = sLaunchsiteDesc;
					else
						siteDesc = sModelDesc;

					siteCategory = (string)selectedObject.getSetting("Category");
					siteType = (SiteType)selectedObject.getSetting("LaunchSiteType");
					flOpenCost = (float)selectedObject.getSetting("OpenCost");
					flCloseValue = (float)selectedObject.getSetting("CloseValue");
					stOpenCost = string.Format("{0}", flOpenCost);
					stCloseValue = string.Format("{0}", flCloseValue);
					siteAuthor = (selectedObject.settings.ContainsKey("author")) ? (string)selectedObject.getSetting("author") : (string)selectedObject.model.getSetting("author");
					// Debug.Log("KK: Making or editing a launchsite");
					editingSite = true;
				}
					
				GUI.enabled = true;

				GUILayout.BeginHorizontal();
					if (GUILayout.Button("Save", GUILayout.Width(130)))
						KerbalKonstructs.instance.saveObjects();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Deselect", GUILayout.Width(130)))
					{
						KerbalKonstructs.instance.saveObjects();
						KerbalKonstructs.instance.deselectObject();
					}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);

				if (GUILayout.Button("Delete Instance", GUILayout.Height(23)))
				{
					KerbalKonstructs.instance.deleteObject(selectedObject);
				}

				if (Event.current.keyCode == KeyCode.Return || (pospasted))
				{
					pospasted = false;
					manuallySet = true;
					position.x = float.Parse(xPos);
					position.y = float.Parse(yPos);
					position.z = float.Parse(zPos);
					vis = float.Parse(visrange);
					alt = float.Parse(altitude);
					float rot = float.Parse(rotation);
					while (rot > 360 || rot < 0)
					{
						if (rot > 360)
						{
							rot -= 360;
						}
						else if (rot < 0)
						{
							rot += 360;
						}
					}
					newRot = rot;
					rotation = rot.ToString();
					shouldUpdateSelection = true;
				}

				if (shouldUpdateSelection)
				{
					if (!manuallySet)
					{
						position += (Vector3)selectedObject.getSetting("RadialPosition");						
						alt += (float)selectedObject.getSetting("RadiusOffset");
						newRot += (float)selectedObject.getSetting("RotationAngle");
						vis += (float)selectedObject.getSetting("VisibilityRange");

						while (newRot > 360 || newRot < 0)
						{ 
							if (newRot > 360)
							{
								newRot -= 360;
							}
							else if (newRot < 0)
							{
								newRot += 360;
							}
						}

						while (vis > 100000 || vis < 1000)
						{
							if (vis > 100000)
							{
								vis = 100000;
							}
							else if (vis < 1000)
							{
								vis = 1000;
							}
						}
					}

					selectedObject.setSetting("RadialPosition", position);
					selectedObject.setSetting("RadiusOffset", alt);
					selectedObject.setSetting("RotationAngle", newRot);
					selectedObject.setSetting("VisibilityRange", vis);
					updateSelection(selectedObject);
				}

				GUILayout.EndArea();
			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		public StaticObject spawnInstance(StaticModel model)
		{
			StaticObject obj = new StaticObject();
			obj.gameObject = GameDatabase.Instance.GetModel(model.path + "/" + model.getSetting("mesh"));
			obj.setSetting("RadiusOffset", (float)FlightGlobals.ActiveVessel.altitude);
			obj.setSetting("CelestialBody", KerbalKonstructs.instance.getCurrentBody());
			obj.setSetting("Group", "Ungrouped");
			obj.setSetting("RadialPosition", KerbalKonstructs.instance.getCurrentBody().transform.InverseTransformPoint(FlightGlobals.ActiveVessel.transform.position));
			obj.setSetting("RotationAngle", 0f);
			obj.setSetting("Orientation", Vector3.up);
			obj.setSetting("VisibilityRange", 25000f);
			obj.model = model;

			KerbalKonstructs.instance.getStaticDB().addStatic(obj);
			enableColliders = false;
			KerbalKonstructs.instance.spawnObject(obj, true);
			return obj;
		}

		public StaticObject spawnInstance(StaticModel model, float fOffset, Vector3 vPosition, float fAngle)
		{
			StaticObject obj = new StaticObject();
			obj.gameObject = GameDatabase.Instance.GetModel(model.path + "/" + model.getSetting("mesh"));
			obj.setSetting("RadiusOffset", fOffset);
			obj.setSetting("CelestialBody", KerbalKonstructs.instance.getCurrentBody());
			obj.setSetting("Group", "Ungrouped");
			obj.setSetting("RadialPosition", vPosition);
			obj.setSetting("RotationAngle", fAngle);
			obj.setSetting("Orientation", Vector3.up);
			obj.setSetting("VisibilityRange", 25000f);
			obj.model = model;

			KerbalKonstructs.instance.getStaticDB().addStatic(obj);
			enableColliders = false;
			KerbalKonstructs.instance.spawnObject(obj, true);
			return obj;
		}
		#endregion

			#region Launchsite Editor
		// Launchsite Editor handlers
		string stOpenCost;
		string stCloseValue;

		// LAUNCHSITE EDITOR
		void drawSiteEditorWindow(int id)
		{
			GUILayout.Box((string)selectedObject.model.getSetting("title"));
			
			GUILayout.BeginHorizontal();
				GUILayout.Label("Site Name: ", GUILayout.Width(120));
				siteName = GUILayout.TextField(siteName);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label("Site Category: ", GUILayout.Width(120));
				GUILayout.Label(siteCategory, GUILayout.Width(80));
				GUILayout.FlexibleSpace();
				GUI.enabled = !(siteCategory == "RocketPad");
				if (GUILayout.Button("RP"))
					siteCategory = "RocketPad";
				GUI.enabled = !(siteCategory == "Runway");
				if (GUILayout.Button("RW"))
					siteCategory = "Runway";
				GUI.enabled = !(siteCategory == "Helipad");
				if (GUILayout.Button("HP"))
					siteCategory = "Helipad";
				GUI.enabled = !(siteCategory == "Other");
				if (GUILayout.Button("OT"))
					siteCategory = "Other";
			GUILayout.EndHorizontal();

			GUI.enabled = true;

			GUILayout.BeginHorizontal();
				GUILayout.Label("Site Type: ", GUILayout.Width(120));
				if (siteType == (SiteType)0)
					GUILayout.Label("VAB", GUILayout.Width(40));
				if (siteType == (SiteType)1)
					GUILayout.Label("SPH", GUILayout.Width(40));
				if (siteType == (SiteType)2)
					GUILayout.Label("Any", GUILayout.Width(40));
				GUILayout.FlexibleSpace();
				GUI.enabled = !(siteType == (SiteType)0);
				if (GUILayout.Button("VAB"))
					siteType = ((SiteType)0);
				GUI.enabled = !(siteType == (SiteType)1);
				if (GUILayout.Button("SPH"))
					siteType = ((SiteType)1);
				GUI.enabled = !(siteType == (SiteType)2);
				if (GUILayout.Button("Any"))
					siteType = ((SiteType)2);
			GUILayout.EndHorizontal();

			GUI.enabled = true;
			
			GUILayout.BeginHorizontal();
				GUILayout.Label("Author: ", GUILayout.Width(120));
				siteAuthor = GUILayout.TextField(siteAuthor);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label("Open Cost: ", GUILayout.Width(120));
				stOpenCost = GUILayout.TextField(stOpenCost);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label("Close Value: ", GUILayout.Width(120));
				stCloseValue = GUILayout.TextField(stCloseValue);
			GUILayout.EndHorizontal();

			GUILayout.Label("Description: ");
			descScroll = GUILayout.BeginScrollView(descScroll);
				siteDesc = GUILayout.TextArea(siteDesc, GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();

			GUI.enabled = true;
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("Save", GUILayout.Width(115)))
				{
					Boolean addToDB = (selectedObject.settings.ContainsKey("LaunchSiteName") && siteName != "");
					selectedObject.setSetting("LaunchSiteName", siteName);
					selectedObject.setSetting("LaunchSiteType", siteType);
					selectedObject.setSetting("LaunchPadTransform", siteTrans);
					selectedObject.setSetting("LaunchSiteDescription", siteDesc);
					selectedObject.setSetting("OpenCost", float.Parse(stOpenCost));
					selectedObject.setSetting("CloseValue", float.Parse(stCloseValue));
					selectedObject.setSetting("OpenCloseState", "Open");
					selectedObject.setSetting("Category", siteCategory);
					if (siteAuthor != (string)selectedObject.model.getSetting("author"))
						selectedObject.setSetting("LaunchSiteAuthor", siteAuthor);
					
					if(addToDB)
					{
						LaunchSiteManager.createLaunchSite(selectedObject);
					}
					KerbalKonstructs.instance.saveObjects();
					
					List<LaunchSite> basesites = LaunchSiteManager.getLaunchSites();
					PersistenceFile<LaunchSite>.SaveList(basesites, "LAUNCHSITES", "KK");
					editingSite = false;
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Cancel", GUILayout.Width(115)))
				{
					editingSite = false;
				}
			GUILayout.EndHorizontal();

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}
		#endregion

		#endregion

		#region Career Persistence
		void saveStaticPersistence(StaticObject obj)
		{
			Boolean bFoundStatic = false;

			// Debug.Log("KK: saveStaticPersistence");
			var FacilityKey = obj.getSetting("RadialPosition");
			// Debug.Log("KK: FacilityKey is " + FacilityKey.ToString());

			string saveConfigPath = string.Format("{0}saves/{1}/KKFacilities.cfg", KSPUtil.ApplicationRootPath, HighLogic.SaveFolder);

			ConfigNode rootNode = new ConfigNode();

			if (!File.Exists(saveConfigPath))
			{
				ConfigNode GameNode = rootNode.AddNode("GAME");
				ConfigNode ScenarioNode = GameNode.AddNode("SCENARIO");
				ScenarioNode.AddValue("Name", "KKStatics");
				rootNode.Save(saveConfigPath);
			}

			rootNode = ConfigNode.Load(saveConfigPath);
			
			ConfigNode rootrootNode = rootNode.GetNode("GAME");
			ConfigNode cnHolder = new ConfigNode();

			foreach (ConfigNode ins in rootrootNode.GetNodes("SCENARIO"))
			{
				cnHolder = ins;
				foreach (ConfigNode insins in ins.GetNodes("KKStatic"))
				{
					// Debug.Log("KK: Found a KKStatic");
					string sRadPos = insins.GetValue("RadialPosition");
					if (sRadPos == null)
					{
						// Debug.Log("KK: Got a KKStatic but it has no key! WTF?????");
						continue;
					}
					if (sRadPos == FacilityKey.ToString())
					{
						// Debug.Log("KK: Got a KKStatic key match - editing the node");

						if (insins.HasValue("LqFCurrent"))
							insins.RemoveValue("LqFCurrent");
						if (insins.HasValue("OxFCurrent"))
							insins.RemoveValue("OxFCurrent");
						if (insins.HasValue("MoFCurrent"))
							insins.RemoveValue("MoFCurrent");

						insins.AddValue("LqFCurrent", obj.getSetting("LqFCurrent").ToString());
						insins.AddValue("OxFCurrent", obj.getSetting("OxFCurrent").ToString());
						insins.AddValue("MoFCurrent", obj.getSetting("MoFCurrent").ToString());
						bFoundStatic = true;
						break;
					}
				}
			}

			if (!bFoundStatic)
			{
				// Debug.Log("KK: No KKStatic found. Creating a new node.");

				ConfigNode newStatic = new ConfigNode("KKStatic");
				newStatic.AddValue("RadialPosition", obj.getSetting("RadialPosition"));
				newStatic.AddValue("LqFCurrent", obj.getSetting("LqFCurrent"));
				newStatic.AddValue("OxFCurrent", obj.getSetting("OxFCurrent"));
				newStatic.AddValue("MoFCurrent", obj.getSetting("MoFCurrent"));
				cnHolder.AddNode(newStatic);
			}

			Debug.Log("KK: rootNode.save");
			rootNode.Save(saveConfigPath);
		}

		void loadStaticPersistence(StaticObject obj)
		{
			// Debug.Log("KK: loadStaticPersistence");
			var FacilityKey = obj.getSetting("RadialPosition");
			// Debug.Log("KK: FacilityKey is " + FacilityKey.ToString());
			
			string saveConfigPath = string.Format("{0}saves/{1}/KKFacilities.cfg", KSPUtil.ApplicationRootPath, HighLogic.SaveFolder);

			ConfigNode rootNode = new ConfigNode();

			if (!File.Exists(saveConfigPath))
			{
				ConfigNode GameNode = rootNode.AddNode("GAME");
				ConfigNode ScenarioNode = GameNode.AddNode("SCENARIO");
				ScenarioNode.AddValue("Name", "KKStatics");
				rootNode.Save(saveConfigPath);
			}

			rootNode = ConfigNode.Load(saveConfigPath);			
			ConfigNode rootrootNode = rootNode.GetNode("GAME");

			Boolean bMatch = false;

			foreach (ConfigNode ins in rootrootNode.GetNodes("SCENARIO"))
			{
				if (ins.GetValue("Name") == "KKStatics")
				{
					// Debug.Log("KK: Found SCENARIO named KKStatics");

					foreach (ConfigNode insins in ins.GetNodes("KKStatic"))
					{
						// Debug.Log("KK: Found a KKStatic");
						string sRadPos = insins.GetValue("RadialPosition");
						if (sRadPos == FacilityKey.ToString())
						{
							// Debug.Log("KK: Got a KKStatic key match");
							obj.setSetting("LqFCurrent", float.Parse(insins.GetValue("LqFCurrent")));
							obj.setSetting("OxFCurrent", float.Parse(insins.GetValue("OxFCurrent")));
							obj.setSetting("MoFCurrent", float.Parse(insins.GetValue("MoFCurrent")));
							bMatch = true;
							break;
						}
						// else
							// Debug.Log("KK: No KKStatic key match");
					}
					break;
				}
			}

			if (!bMatch)
			{
				// Debug.Log("KK: KKStatic not yet persistent for this save. Initialising KKStatic");
				obj.setSetting("LqFCurrent", 0.00f);
				obj.setSetting("OxFCurrent", 0.00f);
				obj.setSetting("MoFCurrent", 0.00f);
				saveStaticPersistence(obj);
			}
		}
		#endregion

		#region Utility Functions

		public Boolean isCareerGame()
		{
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
			{
				if (!KerbalKonstructs.instance.disableCareerStrategyLayer)
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public void updateSelection(StaticObject obj)
		{
			selectedObject = obj;
			xPos = ((Vector3)obj.getSetting("RadialPosition")).x.ToString();
			yPos = ((Vector3)obj.getSetting("RadialPosition")).y.ToString();
			zPos = ((Vector3)obj.getSetting("RadialPosition")).z.ToString();
			altitude = ((float)obj.getSetting("RadiusOffset")).ToString();
			rotation = ((float)obj.getSetting("RotationAngle")).ToString();
			visrange = ((float)obj.getSetting("VisibilityRange")).ToString();
			selectedObject.update();
		}

		public float getIncrement()
		{
			return float.Parse(increment);
		}

		public SiteType getSiteType(int selection)
		{
			switch(selection)
			{
				case 0:
					return SiteType.VAB;
				case 1:
					return SiteType.SPH;
				default:
					return SiteType.Any;
			}
		}
		#endregion
	}
}
