using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TrimPlus
{
    // This MUST be a continuous list beginning with 0.
    public enum BindingName
    {
        TRIM_PITCH_UP = 0,
        TRIM_PITCH_DOWN,
        TRIM_ROLL_LEFT,
        TRIM_ROLL_RIGHT,
        TRIM_YAW_LEFT,
        TRIM_YAW_RIGHT,
        RESET_TRIM,
        RESET_TRIM_PITCH,
        RESET_TRIM_ROLL,
        RESET_TRIM_YAW,
        SET_TRIM,
        SET_TRIM_PITCH,
        SET_TRIM_ROLL,
        SET_TRIM_YAW,
        COUNT
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class TrimPlus : MonoBehaviour
    {        
        public static KeyBinding[] Bindings = new KeyBinding[(int)BindingName.COUNT];
        
        float trimPitchRate = 0.1f;
        float trimRollRate = 0.1f;
        float trimYawRate = 0.1f;
        
        public void Awake()
        {
            SetDefaultBindings();
        }
        
        /// <summary>
        /// Sets default key bindings to all not assigned bindings
        /// </summary>
        /// <param name="force">force default for all bindings even if not null</param>
        public static void SetDefaultBindings(bool force = false)
        {
            for (int i = 0; i < Bindings.Length; i++)
            {
                KeyBinding binding = Bindings[i];

                if(binding == null || force)
                {
                    switch((BindingName)i)
                    {
                        case BindingName.TRIM_PITCH_UP:
                            Bindings[i] = new KeyBinding(KeyCode.PageDown, KeyCode.None);
                            break;
                        case BindingName.TRIM_PITCH_DOWN:
                            Bindings[i] = new KeyBinding(KeyCode.PageUp, KeyCode.None);
                            break;
                        default:
                            Bindings[i] = new KeyBinding(KeyCode.None, KeyCode.None);
                            break;
                    }

                }
            }
        }

        public static void LoadFromConfig()
        {
            try
            {
                ConfigNode node;
                node = ConfigNode.Load("trimPlus.cfg");

                for (int i = 0; i < Bindings.Length; i++)
                {
                    if(node == null)
                    {
                        Bindings[i] = null;
                        continue;
                    }
                        
                    KeyBinding newBinding;
                    try
                    {
                        
                        string nodeName = ((BindingName)i).ToString();                   
                        newBinding = new KeyBinding();
                        newBinding.Load(node.GetNode(nodeName));
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                        newBinding = null;
                    }
                    Bindings[i] = newBinding;            
                }


            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static void SaveToConfig()
        {
            ConfigNode root = new ConfigNode();

            for (int i = 0; i < Bindings.Length; i++)
            {
                ConfigNode newNode = new ConfigNode(((BindingName)i).ToString());
                Bindings[i].Save(newNode);

                root.AddNode(newNode);
                
            }

            root.Save("trimPlus.cfg");

        }

        public void FixedUpdate()
        {
            if (Bindings[(int)BindingName.TRIM_PITCH_UP].GetKey())
            {
                FlightInputHandler.state.pitchTrim = Mathf.Clamp(FlightInputHandler.state.pitchTrim + trimPitchRate * Time.deltaTime, -1f, 1f);
                //FlightGlobals.ActiveVessel.ctrlState.pitchTrim = Mathf.Clamp(FlightGlobals.ActiveVessel.ctrlState.pitchTrim + trimPitchRate * Time.deltaTime, -1f, 1f);
            }

            if (Bindings[(int)BindingName.TRIM_PITCH_DOWN].GetKey())
            {
                FlightInputHandler.state.pitchTrim = Mathf.Clamp(FlightInputHandler.state.pitchTrim - trimPitchRate * Time.deltaTime, -1f, 1f);
                //FlightGlobals.ActiveVessel.ctrlState.pitchTrim = Mathf.Clamp(FlightGlobals.ActiveVessel.ctrlState.pitchTrim - trimPitchRate * Time.deltaTime, -1f, 1f);
            }

            if (Bindings[(int)BindingName.TRIM_ROLL_LEFT].GetKey())
            {
                FlightInputHandler.state.rollTrim = Mathf.Clamp(FlightInputHandler.state.rollTrim - trimRollRate * Time.deltaTime, -1f, 1f);
            }

            if (Bindings[(int)BindingName.TRIM_ROLL_RIGHT].GetKey())
            {
                FlightInputHandler.state.rollTrim = Mathf.Clamp(FlightInputHandler.state.rollTrim + trimRollRate * Time.deltaTime, -1f, 1f);
            }

            if (Bindings[(int)BindingName.TRIM_YAW_LEFT].GetKey())
            {
                FlightInputHandler.state.yawTrim = Mathf.Clamp(FlightInputHandler.state.yawTrim - trimYawRate * Time.deltaTime, -1f, 1f);
            }

            if (Bindings[(int)BindingName.TRIM_YAW_RIGHT].GetKey())
            {
                FlightInputHandler.state.yawTrim = Mathf.Clamp(FlightInputHandler.state.yawTrim + trimYawRate * Time.deltaTime, -1f, 1f);
            }

            if (Bindings[(int)BindingName.RESET_TRIM_PITCH].GetKey() || Bindings[(int)BindingName.RESET_TRIM].GetKey())
                FlightInputHandler.state.pitchTrim = 0.0f;

            if (Bindings[(int)BindingName.RESET_TRIM_ROLL].GetKey() || Bindings[(int)BindingName.RESET_TRIM].GetKey())
                FlightInputHandler.state.rollTrim = 0.0f;

            if (Bindings[(int)BindingName.RESET_TRIM_YAW].GetKey() || Bindings[(int)BindingName.RESET_TRIM].GetKey())
                FlightInputHandler.state.yawTrim = 0.0f;

            if (Bindings[(int)BindingName.SET_TRIM_PITCH].GetKeyDown() || Bindings[(int)BindingName.SET_TRIM].GetKeyDown())
                FlightInputHandler.state.pitchTrim = FlightInputHandler.state.pitch;

            if (Bindings[(int)BindingName.SET_TRIM_ROLL].GetKeyDown() || Bindings[(int)BindingName.SET_TRIM].GetKeyDown())
                FlightInputHandler.state.rollTrim = FlightInputHandler.state.roll;//Mathf.Clamp(/*FlightInputHandler.state.rollTrim +*/ FlightInputHandler.state.roll, -1f, 1f);

            if (Bindings[(int)BindingName.SET_TRIM_YAW].GetKeyDown() || Bindings[(int)BindingName.SET_TRIM].GetKeyDown())
                FlightInputHandler.state.yawTrim = FlightInputHandler.state.yaw;//Mathf.Clamp(/*FlightInputHandler.state.yawTrim +*/ FlightInputHandler.state.yaw, -1f, 1f);

        }

        
            
    }
}
