using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using System.IO;

namespace Map.Events
{
    public class EventManager : MonoBehaviour
    {
        [Header("References")]
        public TextMeshProUGUI header;
        public TextMeshProUGUI body;
        public Transform content;

        [Header("Prefabs")]
        public ActionButton actionButtonPrefab;

        [ContextMenu("Do Init")]
        public void Init()  {
            string path = Application.dataPath + "/Events/";
            DirectoryInfo eventDirectory = new DirectoryInfo(path);
            DirectoryInfo[] eventFolders = eventDirectory.GetDirectories();

            foreach (var mapEvent in eventFolders) {
                FileInfo[] files = mapEvent.GetFiles();
                foreach (var file in files) { 
                    Debug.Log(file.FullName);
                }
            }

        }

        private void DrawSlide(MapEvent mapEvent, string slideID) {
            MapEventSlide slide = mapEvent.Slides.Where(cur => cur.SlideID == slideID).FirstOrDefault();
            if (slide == null) {
                Debug.LogError("Didnt find matching slide");
                return;
            }

            body.text = slide.Text;

            foreach (var item in slide.mapEventActions)
                AddActionButton(item);
            

        }

        private void AddActionButton(MapEventAction action)
        {
            ActionButton newButton = Instantiate(actionButtonPrefab, content.transform);

            newButton.ChangeButtonText(action.ActionText);
            newButton.callback.AddListener(() => DoActionLogic(action.ActionLogics));
        }
        private void DoActionLogic(List<MapEventActionLogic> logic) {
            Debug.Log($"Did action with logic {logic}");
        }
    }

}

