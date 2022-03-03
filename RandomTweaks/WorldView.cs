using UnityEngine;

namespace RandomTweaks
{
    public class WorldView : MonoBehaviour
    {
        public static Rect windowRect = new Rect(0f, 100f, 100f, 250f);

        public void windowFunc(int windowID)
        {
            GUI.Label(new Rect(10f, 20f, 160f, 20f), "Test");
            GUI.DragWindow();
        }

        public void OnGUI()
        {

            Event current = Event.current;
            if (current.keyCode == KeyCode.O)
            {
                
            }

            //windowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), windowRect, new GUI.WindowFunction(windowFunc), "Advanced");

        }
    }

}