using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
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

        UITextField scaleTextBox;
        UILabel scaleTextBoxLabel;

        UITextField tolerance;
        UILabel toleranceLabel;

        UITextField curveTolerance;
        UILabel curveToleranceLabel;

        UITextField tiles;
        UILabel tilesLabel;

        UILabel errorLabel;

        UIButton makeButton;
        UILabel makeErrorLabel;

        UILabel importLabel;
        UICheckBox highwayCheckbox;
        UILabel highwayCheckboxLabel;
        

        public ICities.LoadMode mode;
        RoadMaker2 roadMaker;
        bool createRoads;
        int currentIndex;

        public override void Awake()
        {
            this.isInteractive = true;
            this.enabled = true;
            
            width = 500;

            title = AddUIComponent<UILabel>();

            pathTextBox = AddUIComponent<UITextField>();
            pathTextBoxLabel = AddUIComponent<UILabel>();
            loadMapButton = AddUIComponent<UIButton>();

            scaleTextBox = AddUIComponent<UITextField>();
            scaleTextBoxLabel = AddUIComponent<UILabel>();


            tolerance = AddUIComponent<UITextField>();
            toleranceLabel = AddUIComponent<UILabel>();

            curveTolerance = AddUIComponent<UITextField>();
            curveToleranceLabel = AddUIComponent<UILabel>();

            tiles = AddUIComponent<UITextField>();
            tilesLabel = AddUIComponent<UILabel>();

            errorLabel = AddUIComponent<UILabel>();

            importLabel = AddUIComponent<UILabel>();
            highwayCheckbox = AddUIComponent<UICheckBox>();
            highwayCheckboxLabel = AddUIComponent<UILabel>();

            makeButton = AddUIComponent<UIButton>();
            makeErrorLabel = AddUIComponent<UILabel>();

            base.Awake();

        }
        public override void Start()
        {
            base.Start();

            relativePosition = new Vector3(396, 58);
            backgroundSprite = "MenuPanel2";
            isInteractive = true;
            SetupControls();
        }

        public void SetupControls()
        {
            

            title.text = "OpenStreetCities - OSM Import";
            title.relativePosition = new Vector3(15, 15);
            title.textScale = 0.9f;
            title.size = new Vector2(200, 30);
            var vertPadding = 30;
            var x = 15;
            var y = 25;
            y += vertPadding;
            SetLabel(pathTextBoxLabel, "Path", x, y);
            SetTextBox(pathTextBox, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "map.osm"), x + 120, y);
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
            y += vertPadding;

            
            SetButton(loadMapButton, "Load OSM from file", y);
            loadMapButton.eventClick += LoadMapButton_eventClick;
            y += vertPadding;

            SetLabel(errorLabel, "No OSM data loaded.", x, y);
            errorLabel.textScale = 0.7f;
            y += vertPadding + 50;
            
            SetLabel(importLabel, "IMPORT:", x, y);
            y += vertPadding;

            SetCheckBox(highwayCheckbox, true, x, y);

            x += 20;
            SetLabel(highwayCheckboxLabel, "highways", x, y);
            x -= 20;
            y += vertPadding;

            SetButton(makeButton, "Make roads", y);
            makeButton.eventClick += MakeButton_eventClick;
            makeButton.Disable();
            y += vertPadding;

            SetLabel(makeErrorLabel, "No roads loaded.", x, y);
            makeErrorLabel.textScale = 0.7f;

            y += 50;


            height = y + vertPadding + 6;
        }

        private void LoadMapButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            var path = pathTextBox.text.Trim();
            if (!File.Exists(path))
            {
                if (!File.Exists(path))
                {
                    errorLabel.text = "Cannot find osm file: " + path;
                    return;
                }
            }
            try
            {
                errorLabel.text = "Loading OSM file ...";
                var osm = new OSMInterface(pathTextBox.text.Trim(), double.Parse(scaleTextBox.text.Trim()), double.Parse(tolerance.text.Trim()), double.Parse(curveTolerance.text.Trim()), double.Parse(tiles.text.Trim()));
                currentIndex = 0;
                


                errorLabel.text = "OSM file loaded: " + osm.ways.Count.ToString() + " ways found.";
                roadMaker = new RoadMaker2(osm);

                makeButton.Enable();
                //loadMapButton.Disable();
            }
            catch (Exception ex)
            {
                errorLabel.text = ex.ToString();
            }
        }

        private void SetButton(UIButton okButton, string p1,int x, int y)
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

        private void SetButton(UIButton okButton, string p1, int y)
        {
            okButton.text = p1;
            okButton.normalBgSprite = "ButtonMenu";
            okButton.hoveredBgSprite = "ButtonMenuHovered";
            okButton.disabledBgSprite = "ButtonMenuDisabled";
            okButton.focusedBgSprite = "ButtonMenuFocused";
            okButton.pressedBgSprite = "ButtonMenuPressed";
            okButton.size = new Vector2(260, 24);
            okButton.relativePosition = new Vector3((int)(width - okButton.size.x) / 2,y);
            okButton.textScale = 0.8f;

        }

        private void SetCheckBox(UICheckBox check, Boolean start_on, int x, int y)
        {

            check.isChecked = start_on;
            check.absolutePosition = new Vector3(x, y);
            check.color = Color.white;
            check.size = new Vector2(16, 16);
            check.Show();

            check.enabled = true;
            check.eventClick += (component, param) =>
            {
                check.isChecked = !check.isChecked;
            };
        }

        private void SetTextBox(UITextField scaleTextBox, string p, int x, int y)
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

        private void SetLabel(UILabel pedestrianLabel, string p, int x, int y)
        {
            pedestrianLabel.relativePosition = new Vector3(x, y);
            pedestrianLabel.text = p;
            pedestrianLabel.textScale = 0.8f;
            pedestrianLabel.size = new Vector3(120,20);
        }

        private void MakeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (roadMaker != null)
            {
                createRoads = !createRoads;
            } else {
                makeErrorLabel.text = "No ways found.";
            }
        }

        public override void Update()
        {
            if (createRoads)
            {

                if (currentIndex < roadMaker.osm.ways.Count())
                {
                    //roadMaker.MakeRoad(currentIndex);
                    SimulationManager.instance.AddAction(roadMaker.MakeRoad(currentIndex));
                    currentIndex += 1;
                    var instance = Singleton<NetManager>.instance;
                    makeErrorLabel.text = String.Format("Making road {0} out of {1}. Nodes: {2}. Segments: {3}", currentIndex, roadMaker.osm.ways.Count(), instance.m_nodeCount, instance.m_segmentCount);
                }
            }

            if (roadMaker != null && currentIndex == roadMaker.osm.ways.Count())
            {
                makeErrorLabel.text = "Done.";
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
