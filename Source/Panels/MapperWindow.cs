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

        UITextField pathInput;
        UILabel pathLabel;

        UITextField scaleInput;
        UILabel scaleLabel;

        UITextField toleranceInput;
        UILabel toleranceLabel;

        UITextField tilesInput;
        UILabel tilesLabel;
        UILabel tilesHintLabel;

        UITextField curveToleranceInput;
        UILabel curveToleranceLabel;

        UIButton loadMapButton;
        UIButton discardMapButton;
        UILabel errorLabel;
        UILabel checkRoadTypesLabel;

        UIButton makeButton;
        UIButton stopMakeButton;
        UILabel makeErrorLabel;
        
        SortedList<OSMRoadTypes, UILabel> roadCheckbox = new SortedList<OSMRoadTypes, UILabel>();
        SortedList<OSMRoadTypes, UILabel> roadCheckboxLabel = new SortedList<OSMRoadTypes, UILabel>();
        UILabel totalRoadLabel;

        //UITextField makeSpeedInput;
        UILabel makeSpeedLabel;

        OSMInterface osm;

        int currentMakeSpeed = 5;   // roads per frame

        int roadCheckBoxStartY = 0;

        public ICities.LoadMode mode;
        RoadMaker2 roadMaker;
        bool createRoads;
        int currentIndex;
        int currentRoadTypeIndex;

        const string CHECKBOX_UNCHECKED = "☐";
        const string CHECKBOX_CHECKED = "☑";

        const int maxNodes = 32767;

        public override void Awake()
        {
            this.isInteractive = true;
            this.enabled = true;
            
            width = 500;

            title = AddUIComponent<UILabel>();

            pathInput = AddUIComponent<UITextField>();
            pathLabel = AddUIComponent<UILabel>();
 
            toleranceInput = AddUIComponent<UITextField>();
            toleranceLabel = AddUIComponent<UILabel>();

            curveToleranceInput = AddUIComponent<UITextField>();
            curveToleranceLabel = AddUIComponent<UILabel>();

            scaleInput = AddUIComponent<UITextField>();
            scaleLabel = AddUIComponent<UILabel>();

            tilesInput = AddUIComponent<UITextField>();
            tilesLabel = AddUIComponent<UILabel>();
            tilesHintLabel = AddUIComponent<UILabel>();

            loadMapButton = AddUIComponent<UIButton>();
            discardMapButton = AddUIComponent<UIButton>();
            checkRoadTypesLabel = AddUIComponent<UILabel>();

            errorLabel = AddUIComponent<UILabel>();

            totalRoadLabel = AddUIComponent<UILabel>();

            //makeSpeedInput = AddUIComponent<UITextField>();
            //makeSpeedLabel = AddUIComponent<UILabel>();

            makeButton = AddUIComponent<UIButton>();
            stopMakeButton = AddUIComponent<UIButton>();
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
            SetLabel(pathLabel, "Path", x, y);
            SetTextBox(pathInput, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "map.osm"), x + 120, y);
            y += vertPadding;
            
            SetLabel(toleranceLabel, "Tolerance", x, y);
            SetTextBox(toleranceInput, "0.5", x + 120, y);
            y += vertPadding;

            SetLabel(curveToleranceLabel, "Curve Tolerance", x, y);
            SetTextBox(curveToleranceInput, "0.5", x + 120, y);
            y += vertPadding;

            SetLabel(scaleLabel, "Scale factor", x, y);
            SetTextBox(scaleInput, "1", x + 120, y);
            y += vertPadding;

            SetLabel(tilesLabel, "Tiles to Boundary", x, y);
            SetTextBox(tilesInput, "4.5", x + 120, y);

            y += 20;
            SetLabel(tilesHintLabel, "e.g. \"2.5\" for 5x5=25 tiles; \"4.5\" for 9x9=81 tiles", x + 125, y);
            tilesHintLabel.textScale = .6f;
            tilesHintLabel.textColor = Color.gray;

            y += vertPadding;
            
            SetButton(loadMapButton, "Load OSM from file", x, y);
            loadMapButton.eventClick += LoadMapButton_eventClick;

            SetButton(discardMapButton, "Discard OSM data", x + 235, y);
            discardMapButton.eventClick += DiscardMapButton_eventClick;
            disableButton(discardMapButton);

            y += vertPadding;

            SetLabel(errorLabel, "No OSM data loaded." + Environment.NewLine +
                                 "NOTE: The application will not respond while importing OSM data. " + Environment.NewLine + 
                                 "Please be patient, loading might take some minutes, depending on " + Environment.NewLine + 
                                 "the amount of data.", x, y);
            errorLabel.textScale = 0.6f;
            y += vertPadding * 2;
            
            SetLabel(checkRoadTypesLabel, "Create road types", x, y);
            y += 18;

            roadCheckBoxStartY = y;
            y += 220;

            //SetLabel(makeSpeedLabel, "Roads per frame", x, y);
            //SetTextBox(makeSpeedInput, "3", x + 120, y);
            //disableInput(makeSpeedInput);
            //y += vertPadding;

            SetButton(makeButton, "Make roads", x, y);
            makeButton.eventClick += MakeButton_eventClick;
            disableButton(makeButton);

            SetButton(stopMakeButton, "Stop making roads", x + 235, y);
            stopMakeButton.eventClick += StopMakeButton_eventClick;
            disableButton(stopMakeButton);

            y += vertPadding;

            SetLabel(makeErrorLabel, "", x, y);
            makeErrorLabel.textScale = 0.6f;

            y += 50;
            
            height = y + vertPadding + 6;
        }
        private void DiscardMapButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
            enableOSMImportFields();

            foreach (var rt in roadCheckbox) {
                Destroy(rt.Value);
            }
            roadCheckbox.Clear();
            foreach(var rt in roadCheckboxLabel) {
                Destroy(rt.Value);
            }
            roadCheckboxLabel.Clear();

            errorLabel.text = "No OSM data loaded.";
        }

        private void LoadMapButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            var path = pathInput.text.Trim();
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
                
                this.osm = new OSMInterface(pathInput.text.Trim(), double.Parse(toleranceInput.text.Trim()), double.Parse(curveToleranceInput.text.Trim()), double.Parse(tilesInput.text.Trim()), double.Parse(scaleInput.text.Trim()));
                currentIndex = 0;
                currentRoadTypeIndex = 0;

                int totalCount = 0;
                foreach (var rt in osm.ways) {
                    totalCount += rt.Value.Count;
                }
                errorLabel.text = "OSM file loaded: " + totalCount + " roads (" + osm.ways.Count.ToString() + " types) found.";

                roadMaker = new RoadMaker2(this.osm);
                
                int y = roadCheckBoxStartY;
                int x = 15;

                if (roadMaker != null && roadMaker.netInfos != null) {
                    int rtIndex = 0;
                    foreach (var osmrt in osm.roadTypeCount) {
                        UILabel chk;
                        UILabel lbl;
                        if (!roadCheckbox.ContainsKey(osmrt.Key)) {
                            chk = AddUIComponent<UILabel>();
                            roadCheckbox.Add(osmrt.Key, chk);
                            lbl = AddUIComponent<UILabel>();
                            roadCheckboxLabel.Add(osmrt.Key, lbl);
                        } else {
                            chk = roadCheckbox[osmrt.Key];
                            lbl = roadCheckboxLabel[osmrt.Key];
                        }
                        chk.textScale = .7f;

                        SetCheckBox(chk, false, lbl, x, y);
                        x += 16;

                        int c = 0;
                        if (roadMaker.osm.ways.ContainsKey(osmrt.Key)) {
                            foreach (var n in osm.ways[osmrt.Key]) {
                                c += n.nodes.Count();
                            }
                        }
                        osm.roadTypeCount[osmrt.Key] = c;

                        SetLabel(lbl, osmrt.Key.ToString() + "("+ c + " nodes)", x, y);
                        lbl.textScale = .7f;
                        
                        rtIndex++;
                        if (rtIndex % 2 == 0) {
                            x = 15;
                            y += 14;
                        } else {
                            x = 15 + 250 * (rtIndex % 2);
                        }
                    }
                    x = 270;
                    y += 12;
                    
                    SetLabel(totalRoadLabel, "", x, y);
                    totalRoadLabel.textScale = .7f;

                }

                disableOSMImportFields();
                updateLimits();
            }
            catch (Exception ex)
            {
                errorLabel.text = ex.ToString();
            }
        }
        private void disableButton(UIButton btn) {
            btn.isEnabled = false;
            btn.textColor = Color.gray;
            btn.normalBgSprite = "ButtonMenuDisabled";
            btn.opacity = .333f; 
        }
        private void enableButton(UIButton btn) {
            btn.isEnabled = true;
            btn.textColor = Color.white;
            btn.normalBgSprite = "ButtonMenu";
            btn.opacity = 1f;
        }

        private void disableInput(UITextField input) {
            input.readOnly = true;
            input.textColor = Color.gray;
        }
        private void enableInput(UITextField input) {
            input.readOnly = false;
            input.textColor = Color.white;
        }

        private void disableOSMImportFields() {

            disableInput(pathInput);
            disableInput(toleranceInput);
            disableInput(curveToleranceInput);
            disableInput(scaleInput);
            disableInput(tilesInput);
            disableButton(loadMapButton);

            enableButton(makeButton);
            //enableInput(makeSpeedInput);
            enableButton(discardMapButton);
            
        }
        private void enableOSMImportFields() {
            enableInput(pathInput);
            enableInput(toleranceInput);
            enableInput(curveToleranceInput);
            enableInput(scaleInput);
            enableInput(tilesInput);
            enableButton(loadMapButton);
            

            disableButton(makeButton);
            //disableInput(makeSpeedInput);
            disableButton(discardMapButton);
        }

        private void SetButton(UIButton btn, string p1,int x, int y)
        {
            btn.text = p1;
            btn.normalBgSprite = "ButtonMenu";
            btn.hoveredBgSprite = "ButtonMenuHovered";
            btn.disabledBgSprite = "ButtonMenuDisabled";
            btn.focusedBgSprite = "ButtonMenuFocused";
            btn.pressedBgSprite = "ButtonMenuPressed";
            btn.size = new Vector2(220, 24);
            btn.relativePosition = new Vector3(x, y - 3);
            btn.textScale = 0.8f;
        }

        private void SetButton(UIButton btn, string p1, int y)
        {
            btn.text = p1;
            btn.normalBgSprite = "ButtonMenu";
            btn.hoveredBgSprite = "ButtonMenuHovered";
            btn.disabledBgSprite = "ButtonMenuDisabled";
            btn.focusedBgSprite = "ButtonMenuFocused";
            btn.pressedBgSprite = "ButtonMenuPressed";
            btn.size = new Vector2(220, 24);
            btn.relativePosition = new Vector3((int)(width - btn.size.x) / 2, y);
            btn.textScale = 0.8f;

        }

        private void SetCheckBox(UILabel check, Boolean start_on, UILabel label, int x, int y)
        {
            Boolean isChecked = start_on;
            check.relativePosition = new Vector3(x, y);
            check.color = Color.white;
            check.size = new Vector2(13, 13);
            check.text = (isChecked ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED);
            check.Show();
            check.enabled = true;

            MouseEventHandler checkClick = (component, param) => {
                isChecked = !isChecked;
                check.text = (isChecked ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED);
                updateLimits(); 
            };

            check.eventClicked += checkClick;

            label.eventClicked += checkClick;
        }
        
        private void updateLimits() {
            
            int buildNodeCount = 0;
            int totalNodeCount = 0;

            var instance = Singleton<NetManager>.instance;
            int currentNodeCount = instance.m_nodeCount;

            roadMaker.clearEnabledRoadTypes();
            foreach (var rt in osm.roadTypeCount) {
                if (roadCheckbox.ContainsKey(rt.Key) && roadCheckbox[rt.Key].text == CHECKBOX_CHECKED) { // @todo: improve checkboxes!
                    buildNodeCount += rt.Value;
                    roadMaker.addEnabledRoadTypes(rt.Key);
                }
            }
            totalNodeCount = currentNodeCount + buildNodeCount;
            if (totalNodeCount >= maxNodes) {
                totalRoadLabel.textColor = Color.red;
            } else if(totalNodeCount > maxNodes * .9f) {
                totalRoadLabel.textColor = new Color(1f, 1f, 0f);
            } else {
                totalRoadLabel.textColor = Color.white;
            }
            
            if (buildNodeCount > 0) {
                totalRoadLabel.text = "Current nodes:        " + currentNodeCount + Environment.NewLine +
                                      "New nodes:               " + buildNodeCount + Environment.NewLine +
                                      "Estimated count:   " + totalNodeCount + Environment.NewLine + 
                                      "Node limit:                " + maxNodes;
            } else {
                totalRoadLabel.text = "Current nodes:        " + currentNodeCount + Environment.NewLine + 
                                      "Node limit:                " + maxNodes.ToString();
            }
            if (!createRoads) {
                if (buildNodeCount > 0) {
                    enableButton(makeButton);
                }
            } else {
                disableButton(makeButton);
            }
        }

        private void SetTextBox(UITextField textbox, string p, int x, int y)
        {
            textbox.relativePosition = new Vector3(x, y - 4);
            textbox.horizontalAlignment = UIHorizontalAlignment.Left;
            textbox.text = p;
            textbox.textScale = 0.8f;
            textbox.color = Color.black;
            textbox.cursorBlinkTime = 0.45f;
            textbox.cursorWidth = 1;
            textbox.selectionBackgroundColor = new Color(233,201,148,255);
            textbox.selectionSprite = "EmptySprite";
            textbox.verticalAlignment = UIVerticalAlignment.Middle;
            textbox.padding = new RectOffset(5, 0, 5, 0);
            textbox.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            textbox.normalBgSprite = "TextFieldPanel";
            textbox.hoveredBgSprite = "TextFieldPanelHovered";
            textbox.focusedBgSprite = "TextFieldPanel";
            textbox.disabledBgSprite = "TextFieldPanelDisabled";
            textbox.size = new Vector3(width - 120 - 30, 20);
            textbox.isInteractive = true;
            textbox.enabled = true;
            textbox.readOnly = false;
            textbox.builtinKeyNavigation = true;
            
        }

        private void SetLabel(UILabel lbl, string p, int x, int y)
        {
            lbl.relativePosition = new Vector3(x, y);
            lbl.text = p;
            lbl.textScale = 0.7f;
            lbl.size = new Vector3(120,20);
        }

        private void MakeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (roadMaker != null)
            {
                currentIndex = 0;
                currentRoadTypeIndex = 0;
                createRoads = true;
                if (createRoads) {
                    makeErrorLabel.text = "Create roads ... ";
                    makeErrorLabel.textColor = Color.white;
                }
                enableButton(stopMakeButton);
                disableButton(makeButton);

                //updateMakeRoadSpeed();
                //disableInput(makeSpeedInput);
            } else {
                makeErrorLabel.text = "No ways found.";
                makeErrorLabel.textColor = Color.red;
            }
        }

        /*private void updateMakeRoadSpeed() {
            currentMakeSpeed = Math.Min(Math.Max(1, int.Parse(makeSpeedInput.text)), 1000);
            makeSpeedInput.text = currentMakeSpeed.ToString();
        }*/
        
        private void StopMakeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) 
        {
            createRoads = !createRoads;
            if (createRoads) {
                stopMakeButton.text = "Pause making roads";
                //disableInput(makeSpeedInput);
            } else {
                stopMakeButton.text = "Continue making roads";
                //enableInput(makeSpeedInput);
            }

            
        }

        private void setCheckboxBuilt(UILabel chk, UILabel lbl) {
            chk.SimulateClick();
            lbl.textColor = Color.gray;
            chk.textColor = Color.gray;
        }
        
        public override void Update()
        {
            Boolean reachedTheEnd = false;
            if (createRoads)
            {
                    if (currentRoadTypeIndex < roadCheckbox.Count) {
                        OSMRoadTypes rt = roadCheckbox.Keys.ElementAt(currentRoadTypeIndex);
                        int way_count = roadMaker.osm.ways[rt].Count();

                        if (roadCheckbox[rt].text == CHECKBOX_CHECKED && way_count > 0 && currentIndex < way_count) {
                            SimulationManager.instance.AddAction(roadMaker.MakeRoad(currentIndex, rt));
                            currentIndex += 1;
                        
                            var instance = Singleton<NetManager>.instance;
                            makeErrorLabel.text = String.Format("RoadType {0} / {1}; {2} {3} / {4}. Nodes: {5}. Segments: {6}", currentRoadTypeIndex, roadMaker.osm.ways.Count(), rt.ToString(), currentIndex, way_count, instance.m_nodeCount, instance.m_segmentCount);
                        } else {
                            // end of current roadtype
                            if (currentIndex > 0) {
                                setCheckboxBuilt(roadCheckbox[rt], roadCheckboxLabel[rt]);
                            }
                            currentRoadTypeIndex++;
                            currentIndex = 0;
                        }
                    } else {
                        reachedTheEnd = true;
                        updateLimits();
                    }

            }

            if (roadMaker != null && reachedTheEnd)
            {
                var instance = Singleton<NetManager>.instance;
                int currentNodeCount = instance.m_nodeCount;

                String s = "You can still add different road-types.";
                if (currentNodeCount >= maxNodes) {
                    s = "Engine limit reached! You cannot build any more roads!" + Environment.NewLine +
                        "REMOVE A BUNCH OF ROADS TO MAKE THE MAP PLAYABLE!";
                    makeErrorLabel.textColor = Color.red;
                } else {
                    makeErrorLabel.textColor = Color.green;
                }

                makeErrorLabel.text += Environment.NewLine + "Done. " + s;
                
                createRoads = false;
                reachedTheEnd = false;
                disableButton(stopMakeButton);
            }
            base.Update();
        }
    }
    
}
