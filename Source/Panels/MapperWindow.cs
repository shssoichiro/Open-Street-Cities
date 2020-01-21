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
        UIButton pauseMakeButton;
        UILabel makeErrorLabel;
        
        SortedList<OSMRoadTypes, UILabel> roadCheckbox = new SortedList<OSMRoadTypes, UILabel>();
        SortedList<OSMRoadTypes, UILabel> roadCheckboxLabel = new SortedList<OSMRoadTypes, UILabel>();
        UILabel totalRoadLabel;

        //UITextField makeSpeedInput;
       // UILabel makeSpeedLabel;

        OSMInterface osm;

        int currentMakeSpeed = 10;   // roads per frame

        int roadCheckBoxStartY = 0;

        public ICities.LoadMode mode;
        RoadMaker2 roadMaker;
        bool createRoads = false;
        bool pauseCreateRoads = false;
        int currentIndex;
        int currentRoadTypeIndex;

        const string CHECKBOX_UNCHECKED = "☐";
        const string CHECKBOX_CHECKED = "☑";

        const int maxNodes = 32767;

        Color COLOR_WARNING = new Color(.9f, .6f, .02f);
        Color COLOR_ERROR = new Color(1f, .25f, 0);
        Color COLOR_SUCCESS = new Color(.3f, .8f, .1f);

        Color COLOR_TEXT = Color.white;
        Color COLOR_DISABLED = Color.gray;

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
            pauseMakeButton = AddUIComponent<UIButton>();
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
            title.text = "OpenStreetMap Import 0.1b - Create roads from OSM file";
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
            SetTextBox(toleranceInput, "1", x + 120, y);
            y += vertPadding;

            SetLabel(curveToleranceLabel, "Curve Tolerance", x, y);
            SetTextBox(curveToleranceInput, "0.5", x + 120, y);
            y += vertPadding;

            SetLabel(scaleLabel, "Scale Factor", x, y);
            SetTextBox(scaleInput, "1", x + 120, y);
            y += vertPadding;

            SetLabel(tilesLabel, "Tiles to Boundary", x, y);
            SetTextBox(tilesInput, "4.5", x + 120, y);

            y += 20;
            SetLabel(tilesHintLabel, "e.g. \"2.5\" for 5x5=25 tiles; \"4.5\" for 9x9=81 tiles", x + 125, y);
            tilesHintLabel.textScale = .6f;
            tilesHintLabel.textColor = COLOR_DISABLED;

            y += 20;
            
            SetButton(loadMapButton, "Import OSM from file", x, y);
            loadMapButton.eventClick += LoadMapButton_eventClick;

            SetButton(discardMapButton, "Discard OSM data", x + 235, y);
            discardMapButton.eventClick += DiscardMapButton_eventClick;
            disableButton(discardMapButton);

            y += vertPadding;

            SetLabel(errorLabel, "No OSM data loaded.", x, y);
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

            SetButton(pauseMakeButton, "Pause making roads", x + 235, y);
            pauseMakeButton.eventClick += PauseMakeButton_eventClick;
            disableButton(pauseMakeButton);

            y += 30;

            SetLabel(makeErrorLabel, "NOTE: The application will not respond while importing OSM data. " + Environment.NewLine +
                                     "Please be patient, loading might take some minutes, depending on " + Environment.NewLine +
                                     "the amount of data.", x, y);
            makeErrorLabel.textScale = 0.7f;
            makeErrorLabel.textColor = COLOR_WARNING;

            y += 20;
            
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

            createRoads = false;
            pauseCreateRoads = false;
            updateMakeButton();
            updatePauseButton();
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
                    errorLabel.textColor = COLOR_ERROR;
                    return;
                }
            }
            try
            {
                errorLabel.text = "Loading OSM file ...";
                errorLabel.textColor = COLOR_TEXT;

                this.osm = new OSMInterface(pathInput.text.Trim(), double.Parse(toleranceInput.text.Trim()), double.Parse(curveToleranceInput.text.Trim()), double.Parse(tilesInput.text.Trim()), double.Parse(scaleInput.text.Trim()));
                currentIndex = 0;
                currentRoadTypeIndex = 0;

                int totalCount = 0;
                foreach (var rt in osm.ways) {
                    totalCount += rt.Value.Count;
                }
                errorLabel.text = "OSM file loaded: " + totalCount + " roads (" + osm.ways.Count.ToString() + " types) found.";
                errorLabel.textColor = COLOR_SUCCESS;
                makeErrorLabel.text = "";
                roadMaker = new RoadMaker2(this.osm);
                
                int y = roadCheckBoxStartY;
                int x = 15;

                if (roadMaker != null && roadMaker.netInfos != null) {
                    int rtIndex = 0;
                    foreach (var osmrt in osm.roadTypeCount) {

                        int c = 0;
                        if (roadMaker.osm.ways.ContainsKey(osmrt.Key)) {
                            foreach (var n in osm.ways[osmrt.Key]) {
                                c += n.nodes.Count();
                            }
                        }
                        osm.roadTypeCount[osmrt.Key] = c;
                        if (c > 0) {
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
                            chk.textScale = .8f;

                            SetCheckBox(chk, false, lbl, x, y);
                            x += 16;

                            SetLabel(lbl, osmrt.Key.ToString() + "(" + c + " nodes)", x, y);
                            lbl.textScale = .7f;

                            rtIndex++;
                            if (rtIndex % 2 == 0) {
                                x = 15;
                                y += 14;
                            } else {
                                x = 15 + 250 * (rtIndex % 2);
                            }
                        }
                    }
                    x = 270;
                    y += 5;
                    
                    SetLabel(totalRoadLabel, "", x, y);
                    totalRoadLabel.textScale = .7f;

                }

                disableOSMImportFields();
                updateLimits();
            }
            catch (Exception ex)
            {
                errorLabel.text = ex.ToString();
                errorLabel.textColor = COLOR_ERROR;
            }
        }
        private void disableButton(UIButton btn) {
            btn.isEnabled = false;
            btn.textColor = COLOR_DISABLED;
            btn.normalBgSprite = "ButtonMenuDisabled";
            btn.opacity = .333f; 
        }
        private void enableButton(UIButton btn) {
            btn.isEnabled = true;
            btn.textColor = COLOR_TEXT;
            btn.normalBgSprite = "ButtonMenu";
            btn.opacity = 1f;
        }

        private void disableInput(UITextField input) {
            input.readOnly = true;
            input.textColor = COLOR_DISABLED;
        }
        private void enableInput(UITextField input) {
            input.readOnly = false;
            input.textColor = COLOR_TEXT;
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
            check.color = COLOR_TEXT;
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
                totalRoadLabel.textColor = COLOR_ERROR;
            } else if(totalNodeCount > maxNodes * .9f) {
                totalRoadLabel.textColor = COLOR_WARNING;
            } else {
                totalRoadLabel.textColor = COLOR_TEXT;
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

            if (createRoads) {
                enableButton(makeButton);
                enableButton(pauseMakeButton);
            } else {
                if (buildNodeCount > 0) {
                    enableButton(makeButton);
                } else {
                    disableButton(makeButton);
                }
                disableButton(pauseMakeButton);
            }

            updateMakeButton();
            updatePauseButton();

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
                createRoads = !createRoads;
                if (createRoads) {
                    // started
                    currentIndex = 0;
                    currentRoadTypeIndex = 0;
                    makeErrorLabel.text = "Create roads ... ";
                    makeErrorLabel.textColor = COLOR_TEXT;

                } else {
                    // stopped 
                    currentIndex = 0;
                    currentRoadTypeIndex = 0;
                    pauseCreateRoads = false;
                }
                updatePauseButton();
                updateMakeButton();
            } else {
                makeErrorLabel.text = "No ways found.";
                makeErrorLabel.textColor = Color.red;
            }
        }

        private void updateMakeButton() {
            if (createRoads) {
                makeButton.text = "Stop making roads";
            } else {
                makeButton.text = "Make roads";
            }
        }

        private void updatePauseButton() {
            if (pauseCreateRoads) {
                pauseMakeButton.text = "Continue making roads";
            } else {
                pauseMakeButton.text = "Pause making roads";
            }
            if (createRoads) {
                enableButton(pauseMakeButton);
            } else {
                disableButton(pauseMakeButton);
            }
        }

        /*private void updateMakeRoadSpeed() {
            currentMakeSpeed = Math.Min(Math.Max(1, int.Parse(makeSpeedInput.text)), 1000);
            makeSpeedInput.text = currentMakeSpeed.ToString();
        }*/
        
        private void PauseMakeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) 
        {
            pauseCreateRoads = !pauseCreateRoads;
            updatePauseButton();
        }

        private void setCheckboxBuilt(UILabel chk, UILabel lbl) {
            chk.SimulateClick();
            lbl.textColor = COLOR_DISABLED;
            chk.textColor = COLOR_DISABLED;
        }
        
        public override void Update()
        {
            Boolean reachedTheEnd = false;
            if (createRoads && !pauseCreateRoads)
            {
                for (int i = 0; i < currentMakeSpeed; i++) {
                    if (currentRoadTypeIndex < roadCheckbox.Count) {
                        OSMRoadTypes rt = roadCheckbox.Keys.ElementAt(currentRoadTypeIndex);
                        int way_count = roadMaker.osm.ways[rt].Count();

                        if (roadCheckbox[rt].text == CHECKBOX_CHECKED && way_count > 0 && currentIndex < way_count) {
                            SimulationManager.instance.AddAction(roadMaker.MakeRoad(currentIndex, rt));
                            currentIndex++;

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
                        i = currentMakeSpeed; // break;
                    }
                }
            }
            if (reachedTheEnd) {
                var instance = Singleton<NetManager>.instance;
                int currentNodeCount = instance.m_nodeCount;

                String s = "You can still add different road-types.";
                if (currentNodeCount >= maxNodes) {
                    s = "Engine limit reached! You cannot build any more roads!" + Environment.NewLine +
                        "REMOVE A BUNCH OF ROADS TO MAKE THE MAP PLAYABLE!";
                    makeErrorLabel.textColor = COLOR_ERROR;
                } else {
                    makeErrorLabel.textColor = COLOR_SUCCESS;
                }

                makeErrorLabel.text += Environment.NewLine + "Done. " + s;
                
                createRoads = false;
                reachedTheEnd = false;
                currentIndex = 0;
                currentRoadTypeIndex = 0;

                updateLimits();
            }

            base.Update();
        }
    }
    
}
