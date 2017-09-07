using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Plugins;
using UnityEngine;
using System.IO;
using Mapper.OSM;

namespace Mapper
{
    public class MapperWindow7 : UIPanel
    {
        UILabel title;

        UITextField pathTextBox;
        UILabel pathTextBoxLabel;
        UIButton loadMapButton;

        UIButton pedestriansCheck;
        UILabel pedestrianLabel;

        UIButton roadsCheck;
        UILabel roadsLabel;

        UIButton highwaysCheck;
        UILabel highwaysLabel;

        UITextField scaleTextBox;
        UILabel scaleTextBoxLabel;

        UITextField tolerance;
        UILabel toleranceLabel;

        UITextField curveTolerance;
        UILabel curveToleranceLabel;

        UITextField tiles;
        UILabel tilesLabel;

        UILabel errorLabel;

        UIButton okButton;

        public ICities.LoadMode mode;
        RoadMaker2 roadMaker;
        bool createRoads;
        int currentIndex;
         bool peds = true;
         bool roads = true;
         bool highways = true;

        public override void Awake()
        {
            this.isInteractive = true;
            this.enabled = true;
            
            width = 500;

            title = AddUIComponent<UILabel>();

            pathTextBox = AddUIComponent<UITextField>();
            pathTextBoxLabel = AddUIComponent<UILabel>();
            loadMapButton = AddUIComponent<UIButton>();

            pedestriansCheck = AddUIComponent<UIButton>();
            pedestrianLabel = AddUIComponent<UILabel>();
            roadsCheck = AddUIComponent<UIButton>();
            roadsLabel = AddUIComponent<UILabel>();
            highwaysCheck = AddUIComponent<UIButton>();
            highwaysLabel = AddUIComponent<UILabel>();

            scaleTextBox = AddUIComponent<UITextField>();
            scaleTextBoxLabel = AddUIComponent<UILabel>();

            tolerance = AddUIComponent<UITextField>();
            toleranceLabel = AddUIComponent<UILabel>();

            curveTolerance = AddUIComponent<UITextField>();
            curveToleranceLabel = AddUIComponent<UILabel>();

            tiles = AddUIComponent<UITextField>();
            tilesLabel = AddUIComponent<UILabel>();

            errorLabel = AddUIComponent<UILabel>();

            okButton = AddUIComponent<UIButton>();

            base.Awake();

        }

        public override void Start()
        {
            base.Start();

            relativePosition = new Vector3(396, 58);
            backgroundSprite = "MenuPanel2";
            isInteractive = true;
            //this.CenterToParent();
            SetupControls();
        }

        public void SetupControls()
        {
            

            title.text = "Open Street Map Import";
            title.relativePosition = new Vector3(15, 15);
            title.textScale = 0.9f;
            title.size = new Vector2(200, 30);
            var vertPadding = 30;
            var x = 15;
            var y = 50;

            SetLabel(pedestrianLabel, "Pedestrian Paths", x, y);
            SetButton(pedestriansCheck, "True", x + 114, y);
            pedestriansCheck.eventClick +=PedestriansCheck_eventClick;
            x += 190;
            SetLabel(roadsLabel, "Roads", x, y);
            SetButton(roadsCheck, "True", x + 80, y);
            roadsCheck.eventClick += RoadsCheck_eventClick;
            x += 140;
            SetLabel(highwaysLabel, "Highways", x, y);
            SetButton(highwaysCheck, "True", x + 80, y);
            highwaysCheck.eventClick += HighwaysCheck_eventClick;

            x = 15;
            y += vertPadding;

            SetLabel(scaleTextBoxLabel, "Scale", x, y);
            SetTextBox(scaleTextBox, "1", x + 120, y);
            y += vertPadding;


            SetLabel(toleranceLabel, "Tolerance", x, y);
            SetTextBox(tolerance, "6", x + 120, y);
            y += vertPadding;

            SetLabel(curveToleranceLabel, "Curve Tolerance", x, y);
            SetTextBox(curveTolerance, "6", x + 120, y);
            y += vertPadding;

            SetLabel(tilesLabel, "Tiles to Boundary", x, y);
            SetTextBox(tiles, "2.5", x + 120, y);
            y += vertPadding + 12;

            SetLabel(pathTextBoxLabel, "Path", x, y);
            SetTextBox(pathTextBox, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "map"), x + 120, y);
            y += vertPadding - 5;
            SetButton(loadMapButton, "Load OSM From File", y);
            loadMapButton.eventClick += LoadMapButton_eventClick;
            y += vertPadding + 5;

            SetLabel(errorLabel, "No OSM data loaded.", x, y);
            errorLabel.textScale = 0.6f;
            y += vertPadding + 12;

            SetButton(okButton, "Make Roads", y);
            okButton.eventClick += OkButton_eventClick;
            okButton.Disable();
            y += vertPadding;

            height = y + vertPadding + 6;

        }

        private void HighwaysCheck_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            highways = !highways;
            highwaysCheck.text = highways.ToString();
        }

        private void RoadsCheck_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            roads = !roads;
            roadsCheck.text = roads.ToString();
        }

        private void PedestriansCheck_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            peds = !peds;
            pedestriansCheck.text = peds.ToString();
        }

        private void LoadMapButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            var path = pathTextBox.text.Trim();
            if (!File.Exists(path))
            {
                path += ".osm";
                if (!File.Exists(path))
                {
                    errorLabel.text = "Cannot find osm file: " + path;
                    return;
                }
            }
            try
            {
                var osm = new OSMInterface(pathTextBox.text.Trim(), double.Parse(scaleTextBox.text.Trim()), double.Parse(tolerance.text.Trim()), double.Parse(curveTolerance.text.Trim()), double.Parse(tiles.text.Trim()));
                currentIndex = 0;
                roadMaker = new RoadMaker2(osm);
                errorLabel.text = "File Loaded.";
                okButton.Enable();
                loadMapButton.Disable();
            }
            catch (Exception ex)
            {

                errorLabel.text = ex.ToString();
				DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, ex.StackTrace);
            }
        }

        private void SetButton(UIButton okButton, string p1,long x, long y)
        {
            okButton.text = p1;
            okButton.normalBgSprite = "ButtonMenu";
            okButton.hoveredBgSprite = "ButtonMenuHovered";
            okButton.disabledBgSprite = "ButtonMenuDisabled";
            okButton.focusedBgSprite = "ButtonMenuFocused";
            okButton.pressedBgSprite = "ButtonMenuPressed";
            okButton.size = new Vector2(50, 18);
            okButton.relativePosition = new Vector3(x, y - 3);
            okButton.textScale = 0.8f;
        }

        private void SetButton(UIButton okButton, string p1, long y)
        {
            okButton.text = p1;
            okButton.normalBgSprite = "ButtonMenu";
            okButton.hoveredBgSprite = "ButtonMenuHovered";
            okButton.disabledBgSprite = "ButtonMenuDisabled";
            okButton.focusedBgSprite = "ButtonMenuFocused";
            okButton.pressedBgSprite = "ButtonMenuPressed";
            okButton.size = new Vector2(260, 24);
            okButton.relativePosition = new Vector3((long)(width - okButton.size.x) / 2,y);
            okButton.textScale = 0.8f;

        }

        private void SetCheckBox(UICustomCheckbox3 pedestriansCheck, long x, long y)
        {

            pedestriansCheck.IsChecked = true;
            pedestriansCheck.relativePosition = new Vector3(x, y);
            pedestriansCheck.size = new Vector2(13, 13);
            pedestriansCheck.Show();
            pedestriansCheck.color = new Color32(185, 221, 254, 255);
            pedestriansCheck.enabled = true;            
            pedestriansCheck.spriteName = "AchievementCheckedFalse";
            pedestriansCheck.eventClick += (component, param) =>
            {
                pedestriansCheck.IsChecked = !pedestriansCheck.IsChecked;
            };
        }

        private void SetTextBox(UITextField scaleTextBox, string p, long x, long y)
        {
            scaleTextBox.relativePosition = new Vector3(x, y - 4);
            scaleTextBox.horizontalAlignment = UIHorizontalAlignment.Left;
            scaleTextBox.text = p;
            scaleTextBox.textScale = 0.8f;
            scaleTextBox.color = Color.black;
            scaleTextBox.cursorBlinkTime = 0.45f;
            scaleTextBox.cursorWidth = 1;
            scaleTextBox.selectionBackgroundColor = new Color(233,201,148,255);
            scaleTextBox.selectionSprite = "EmptySprite";
            scaleTextBox.verticalAlignment = UIVerticalAlignment.Middle;
            scaleTextBox.padding = new RectOffset(5, 0, 5, 0);
            scaleTextBox.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            scaleTextBox.normalBgSprite = "TextFieldPanel";
            scaleTextBox.hoveredBgSprite = "TextFieldPanelHovered";
            scaleTextBox.focusedBgSprite = "TextFieldPanel";
            scaleTextBox.size = new Vector3(width - 120 - 30, 20);
            scaleTextBox.isInteractive = true;
            scaleTextBox.enabled = true;
            scaleTextBox.readOnly = false;
            scaleTextBox.builtinKeyNavigation = true;
            
        }

        private void SetLabel(UILabel pedestrianLabel, string p, long x, long y)
        {
            pedestrianLabel.relativePosition = new Vector3(x, y);
            pedestrianLabel.text = p;
            pedestrianLabel.textScale = 0.8f;
            pedestrianLabel.size = new Vector3(120,20);
        }

        private void OkButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (roadMaker != null)
            {
                createRoads = !createRoads;
            }
        }

        public override void Update()
        {
            if (createRoads)
            {
                var pp = peds;
                var rr = roads;
                var hh = highways;                
                if (currentIndex < roadMaker.osm.ways.Count())
                {
                    //roadMaker.MakeRoad(currentIndex);
                    SimulationManager.instance.AddAction(roadMaker.MakeRoad(currentIndex,pp,rr,hh));
                    currentIndex += 1;
                }

                if (currentIndex < roadMaker.osm.ways.Count())
                {
                    //roadMaker.MakeRoad(currentIndex);
                    SimulationManager.instance.AddAction(roadMaker.MakeRoad(currentIndex, pp, rr, hh));
                    currentIndex += 1;
                }

                if (currentIndex < roadMaker.osm.ways.Count())
                {
                    //roadMaker.MakeRoad(currentIndex);
                    SimulationManager.instance.AddAction(roadMaker.MakeRoad(currentIndex, pp, rr, hh));
                    currentIndex += 1;
                    var instance = Singleton<NetManager>.instance;
                    errorLabel.text = String.Format("Making road {0} out of {1}. Nodes: {2}. Segments: {3}", currentIndex, roadMaker.osm.ways.Count(), instance.m_nodeCount, instance.m_segmentCount);
                }
            }

            if (roadMaker != null && currentIndex == roadMaker.osm.ways.Count())
            {
                errorLabel.text = "Done.";
                createRoads = false;
            }
            base.Update();
        }
    }

    public class UICustomCheckbox3 : UISprite
    {
        public bool IsChecked { get; set; }

        public override void Start()
        {
            base.Start();
            IsChecked = true;
            spriteName = "AchievementCheckedTrue";
        }

        public override void Update()
        {
            base.Update();
            spriteName = IsChecked ? "AchievementCheckedTrue" : "AchievementCheckedFalse";
        }
    }    
}
