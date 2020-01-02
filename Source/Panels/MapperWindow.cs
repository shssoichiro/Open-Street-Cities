using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System.IO;
using Mapper.OSM;
using System.Collections.Generic;

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
        
        Dictionary<RoadTypes, UILabel> roadCheckbox = new Dictionary<RoadTypes, UILabel>();
        Dictionary<RoadTypes, UILabel> roadCheckboxLabel = new Dictionary<RoadTypes, UILabel>();
        UILabel totalRoadLabel;

        OSMInterface osm;

        int roadCheckBoxStartY = 0;

        public ICities.LoadMode mode;
        RoadMaker2 roadMaker;
        bool createRoads;
        int currentIndex;
        int currentRoadTypeIndex;

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

            totalRoadLabel = AddUIComponent<UILabel>();
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
            errorLabel.textScale = 0.6f;
            y += vertPadding * 2;
            
            SetLabel(importLabel, "Create road types:", x, y);
            y += 20;

            roadCheckBoxStartY = y;
            //SetCheckBox(motorwayCheckbox, true, motorwayCheckboxLabel, x, y);
            //x += 20;
            //SetLabel(motorwayCheckboxLabel, "motorway", x, y);
            ///x -= 20;
            y += vertPadding * 3;

            SetButton(makeButton, "Make roads", y);
            makeButton.eventClick += MakeButton_eventClick;
            makeButton.Disable();
            y += vertPadding;

            SetLabel(makeErrorLabel, "No roads loaded.", x, y);
            makeErrorLabel.textScale = 0.6f;

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
                this.osm = new OSMInterface(pathTextBox.text.Trim(), double.Parse(scaleTextBox.text.Trim()), double.Parse(tolerance.text.Trim()), double.Parse(curveTolerance.text.Trim()), double.Parse(tiles.text.Trim()));
                currentIndex = 0;
                currentRoadTypeIndex = 0;

                errorLabel.text = "OSM file loaded: " + osm.ways.Count.ToString() + " ways found.";
                roadMaker = new RoadMaker2(this.osm);

                roadCheckbox.Clear();
                int y = roadCheckBoxStartY;
                int x = 15;

                if (roadMaker != null && roadMaker.netInfos != null) {
                    int rtIndex = 0;
                    foreach (var rt in osm.roadTypeCount) {
                        UILabel chk;
                        UILabel lbl;
                        if (!roadCheckbox.ContainsKey(rt.Key)) {
                            chk = AddUIComponent<UILabel>();
                            roadCheckbox.Add(rt.Key, chk);
                            lbl = AddUIComponent<UILabel>();
                            roadCheckboxLabel.Add(rt.Key, lbl);
                        } else {
                            chk = roadCheckbox[rt.Key];
                            lbl = roadCheckboxLabel[rt.Key];
                        }
                        chk.textScale = .7f;

                        SetCheckBox(chk, true, lbl, x, y);
                        x += 16;
                        SetLabel(lbl, rt.Key.ToString() + "("+rt.Value.ToString()+" nodes)", x, y);
                        lbl.textScale = .6f;
                        
                        rtIndex++;
                        if (rtIndex % 2 == 0) {
                            x = 15;
                            y += 12;
                        } else {
                            x = 15 + 250 * (rtIndex % 2);
                        }
                    }
                    x = 250;
                    y += 12;
                    
                    SetLabel(totalRoadLabel, "Total: 0 / 32256", x, y);
                    totalRoadLabel.textScale = .7f;
                }

                makeButton.Enable();

                updateLimits();
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

        private void SetCheckBox(UILabel check, Boolean start_on, UILabel label, int x, int y)
        {
            Boolean isChecked = start_on;
            check.relativePosition = new Vector3(x, y);
            check.color = Color.white;
            check.size = new Vector2(13, 13);
            check.text = (isChecked ? "☑" : "☐");
            check.Show();
            check.enabled = true;

            MouseEventHandler checkClick = (component, param) => {
                isChecked = !isChecked;
                check.text = (isChecked ? "☑" : "☐");

                updateLimits(); 
            };

            check.eventClicked += checkClick;

            label.eventClicked += checkClick;
        }
        
        private void updateLimits() {
            int maxNodes = 32256;
            int total = 0;

            roadMaker.clearEnabledRoadTypes();
            foreach (var rt in osm.roadTypeCount) {
                if (roadCheckbox.ContainsKey(rt.Key) && roadCheckbox[rt.Key].text == "☑") { // @todo: improve checkboxes!
                    total += rt.Value;
                    roadMaker.addEnabledRoadTypes(rt.Key);
                }
            }

            if (total > maxNodes) {
                totalRoadLabel.textColor = Color.red;
            } else if(total > maxNodes * .8f) {
                totalRoadLabel.textColor = new Color(1f, 1f, 0f);
            } else {
                totalRoadLabel.textColor = Color.white;
            }
            totalRoadLabel.text = "Total: " + total + " / 32256";
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

        private void SetLabel(UILabel lbl, string p, int x, int y)
        {
            lbl.relativePosition = new Vector3(x, y);
            lbl.text = p;
            lbl.textScale = 0.8f;
            lbl.size = new Vector3(120,20);
        }

        private void MakeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (roadMaker != null)
            {
                currentIndex = 0;
                currentRoadTypeIndex = 0;
                createRoads = !createRoads;
                if (createRoads) {
                    makeErrorLabel.text = "Create roads ... ";
                    makeErrorLabel.textColor = Color.white;
                }
            } else {
                makeErrorLabel.text = "No ways found.";
                makeErrorLabel.textColor = Color.red;
            }
        }

        public override void Update()
        {
            Boolean reachedTheEnd = false;
            if (createRoads)
            {
                if (currentRoadTypeIndex < roadCheckbox.Count) {
                    RoadTypes rt = roadCheckbox.Keys.ElementAt(currentRoadTypeIndex);
                    
                    if (roadCheckbox[rt].text == "☑" && currentIndex < roadMaker.osm.ways[rt].Count()) {
                        SimulationManager.instance.AddAction(roadMaker.MakeRoad(currentIndex, rt));
                        currentIndex += 1;
                        var instance = Singleton<NetManager>.instance;
                        makeErrorLabel.text = String.Format("RoadType {0} / {1}; road {2} / {3}. Nodes: {4}. Segments: {5}", currentRoadTypeIndex, roadMaker.osm.ways.Count(), currentIndex, roadMaker.osm.ways[rt].Count(), instance.m_nodeCount, instance.m_segmentCount);
                    } else {
                        // end of current roadtype
                        currentRoadTypeIndex++;
                        currentIndex = 0;
                    }
                } else {
                    reachedTheEnd = true;
                }
            }

            if (roadMaker != null && reachedTheEnd)
            {
                makeErrorLabel.text += "Done.";
                makeErrorLabel.textColor = Color.green;
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
