using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TrimPlus
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    class TrimPlusSettings : MonoBehaviour
    {

        KeyBinding listeningFor = null;
        bool listeningForPrimary;
        
        static bool likeAVirgin = true;

        public void Awake()
        {
            if (likeAVirgin)
            {
                TrimPlus.LoadFromConfig();
                likeAVirgin = false;
            }
            TrimPlus.SetDefaultBindings();
        }        

        public void Update()
        {
            if(listeningFor != null)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    listeningFor = null;
                    return;
                }

                for (int i = 0; i <= 429; i++)
                {
                    try
                    {
                        KeyCode code = (KeyCode)i;
                        if(Input.GetKeyDown(code))
                        {
                            if (listeningForPrimary)
                            {
                                listeningFor.primary = new KeyCodeExtended(code);
                                listeningFor = null;
                                break;
                            }
                            else
                            {
                                listeningFor.secondary = new KeyCodeExtended(code);
                                listeningFor = null;
                                break;
                            }
                        }
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                    
                }   

            }
        }


        Rect windowRect = new Rect(0, 0, 600, 50);
        bool collapsed = true;

        public void OnGUI()
        {
            GUI.skin = HighLogic.Skin;
            windowRect = GUILayout.Window(3453466, windowRect, windowFunc, "TRIM PLUS", GUILayout.Width(600f));
            //windowRect.height = 0f;
        }

        public void windowFunc(int id)
        {
            GUILayout.BeginArea(new Rect(2f, 2f, 40f, 40f));
            if (GUI.Button(new Rect(0, 0, 30, 30), collapsed ? "+" : "-"))
            {
                collapsed = !collapsed;
                if (collapsed)
                    windowRect.height = 42;
            }
            GUILayout.EndArea();
            GUILayout.Space(3);
            
            if (!collapsed)
            {
                for (int i = 0; i < (int)BindingName.COUNT; i++)
                {
                    KeyBinding binding = TrimPlus.Bindings[i];

                    if (listeningFor != null)
                        GUI.enabled = false;
                    else
                        GUI.enabled = true;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(((BindingName)i).ToString(), GUILayout.Width(150f));

                    if (GUILayout.Button(listeningFor == binding && listeningForPrimary ? "press key..." : binding.primary.ToString()))
                    {
                        listeningFor = binding;
                        listeningForPrimary = true;
                    }

                    if (GUILayout.Button(listeningFor == binding && !listeningForPrimary ? "press key..." : binding.secondary.ToString()))
                    {
                        listeningFor = binding;
                        listeningForPrimary = false;
                    }

                    if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                    {
                        binding.primary = new KeyCodeExtended(KeyCode.None);
                        binding.secondary = new KeyCodeExtended(KeyCode.None);
                    }

                    GUILayout.EndHorizontal();

                    GUI.enabled = true;

                }

                if (GUILayout.Button("save"))
                    TrimPlus.SaveToConfig();

            }
            GUI.DragWindow();       
        }

    }
}
