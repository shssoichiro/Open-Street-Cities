﻿using System;

namespace Mapper
{
    public class MapperLoading : LoadingExtensionBase
    {
        GameObject buildingWindowGameObject;
        GameObject buttonObject;
        GameObject buttonObject2;
        UIButton menuButton;

        MapperWindow7 buildingWindow;
        private LoadMode _mode;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewMap && mode != LoadMode.LoadMap)
            {
                return;
            }

            _mode = mode;

            buildingWindowGameObject = new GameObject("buildingWindowObject");

            var view = UIView.GetAView();
            this.buildingWindow = buildingWindowGameObject.AddComponent<MapperWindow7>();
            this.buildingWindow.transform.parent = view.transform;
            this.buildingWindow.position = new Vector3(300, 122);
            this.buildingWindow.Hide();


            UITabstrip strip = null;
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                strip = ToolsModifierControl.mainToolbar.component as UITabstrip;
            }
            else
            {
                strip = UIView.Find<UITabstrip>("MainToolstrip");
            }

            buttonObject = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
            buttonObject2 = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
            menuButton = strip.AddTab("mapperMod", buttonObject, buttonObject2, new Type[] { }) as UIButton;
            menuButton.eventClick += UiButton_eventClick;
        }

        private void UiButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {

            if (!this.buildingWindow.isVisible)
            {
                this.buildingWindow.isVisible = true;
                this.buildingWindow.BringToFront();
                this.buildingWindow.Show();
            }
            else
            {
                this.buildingWindow.isVisible = false;
                this.buildingWindow.Hide();
            }
        }

        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame && _mode != LoadMode.NewMap && _mode != LoadMode.LoadMap)
            {
                return;
            }

            if (buildingWindowGameObject != null)
            {
                UnityEngine.Object.Destroy(buildingWindowGameObject);
            }

            if (buttonObject != null)
            {
                UnityEngine.Object.Destroy(buttonObject);
                UnityEngine.Object.Destroy(buttonObject2);
                UnityEngine.Object.Destroy(menuButton);
            }
        }

    }
}
