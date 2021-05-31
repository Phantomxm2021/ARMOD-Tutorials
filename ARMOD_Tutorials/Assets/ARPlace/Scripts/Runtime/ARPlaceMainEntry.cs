using UnityEngine;
using System;
using System.Collections;
using com.Phantoms.ActionNotification.Runtime;
using com.Phantoms.ARMODAPI.Runtime;
using UnityEngine.UI;

namespace ARPlace
{
    public class ARPlaceMainEntry
    {
        private static API API = new API();

        private const string CONST_PROJECT_NAME = "ARPlace";
        private const string CONST_AR_OBJECT_NAME = "ARPlaceObject";
        private const string CONST_FOCUS_GROUP_NAME = "FocusGroup";
        private const string CONST_CANVAS_NAME = "Canvas";

        private const string CONST_FOCUS_FIND_NAME = "Find";
        private const string CONST_FOCUS_FOUND_NAME = "Found";
        private const string CONST_PLACE_BUTTON_NAME = "PlaceButton";

        private GameObject arObject;
        private GameObject focusGroup;
        private GameObject canvas;

        private GameObject found;
        private GameObject find;
        private GameObject placeButtonGameObject;

        private Button placeButton;

        private Transform focusGroupTrans;

        private bool canPlace = true;

        //Please delete the function if it is not used
        public void OnLoad()
        {
            //Use this for initialization
            API.LoadAsset<GameObject>(CONST_PROJECT_NAME, CONST_AR_OBJECT_NAME,
                _result =>
                {
                    arObject = API.InstanceGameObject(_result, string.Empty, null);
                    arObject.SetActive(false);
                });

            API.LoadAsset<GameObject>(CONST_PROJECT_NAME, CONST_FOCUS_GROUP_NAME, _result =>
            {
                focusGroup = API.InstanceGameObject(_result, string.Empty, null);
                found = API.FindGameObjectByName(CONST_FOCUS_FOUND_NAME);
                find = API.FindGameObjectByName(CONST_FOCUS_FIND_NAME);
                find.SetActive(true);
                focusGroupTrans = focusGroup.transform;
            });

            API.LoadAsset<GameObject>(CONST_PROJECT_NAME, CONST_CANVAS_NAME, _result =>
            {
                canvas = API.InstanceGameObject(_result, string.Empty, null);
                placeButtonGameObject = API.FindGameObjectByName(CONST_PLACE_BUTTON_NAME);
                placeButton = placeButtonGameObject.GetComponent<Button>();
                placeButton.onClick.AddListener(() =>
                {
                    arObject.transform.position = focusGroupTrans.position;
                    arObject.transform.rotation = focusGroupTrans.rotation;
                    
                    arObject.SetActive(true);
                    focusGroup.SetActive(false);
                    placeButtonGameObject.SetActive(false);
                    
                    canPlace = false;
                });
            });
        }

        //Please delete the function if it is not used
        public void OnEvent(BaseNotificationData _data)
        {
            if (!canPlace) return;
            if (!focusGroup) return;
            if (!find) return;
            if (!found) return;
            if (!placeButtonGameObject) return;
            
            //General event callback
            if (_data is FocusResultNotificationData tmp_FocusResultNotificationData)
            {
                switch (tmp_FocusResultNotificationData.FocusState)
                {
                    case FindingType.Finding:
                        found.SetActive(false);
                        find.SetActive(true);
                        if (!focusGroup.activeSelf)
                            focusGroup.SetActive(true);
                        placeButtonGameObject.SetActive(false);
                        break;
                    case FindingType.Found:
                        find.SetActive(false);
                        found.SetActive(true);
                        if (!focusGroup.activeSelf)
                            focusGroup.SetActive(true);
                        placeButtonGameObject.SetActive(true);
                        break;
                    case FindingType.Limit:
                        focusGroup.SetActive(false);
                        placeButtonGameObject.SetActive(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                focusGroupTrans.position = tmp_FocusResultNotificationData.FocusPos;
                focusGroupTrans.rotation = tmp_FocusResultNotificationData.FocusRot;
            }
            
        }
    }
}