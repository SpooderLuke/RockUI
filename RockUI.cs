using Harmony;
using HarmonyLib;
using Il2CppRUMBLE.Interactions.InteractionBase;
using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
[assembly: MelonInfo(typeof(RockUI.Main), "RockUI", "1.0.1", "SpooderCode")]
[assembly: MelonGame(null, null)]
[assembly: MelonColor(255,0,255,150)]
namespace RockUI
{
    public class Main : MelonMod
    {
        public override void OnLateInitializeMelon()
        {
            RumbleModdingAPI.RMAPI.Actions.onMapInitialized += init;
        }
        public void init(string obj)
        {
            if (RumbleModdingAPI.RMAPI.Calls.Scene.GetSceneName() == "Gym")
            {
                if (RockUI.SliderPrefab == null)
                {
                    RockUI.SliderPrefab = GameObject.Instantiate(RumbleModdingAPI.RMAPI.GameObjects.Gym.INTERACTABLES.MenuSlab.Content.Input.StickMovementDeadzone.Slider.GetGameObject());
                    GameObject.DontDestroyOnLoad(RockUI.SliderPrefab);
                }
                if (RockUI.LeverPrefab == null)
                {
                    RockUI.LeverPrefab = GameObject.Instantiate(RumbleModdingAPI.RMAPI.GameObjects.Gym.INTERACTABLES.Howard.Howardsconsole.Interactebles.InteractionLever.GetGameObject());
                    GameObject.DontDestroyOnLoad(RockUI.LeverPrefab);
                }
                RockUI.panelMesh = RumbleModdingAPI.RMAPI.GameObjects.Gym.INTERACTABLES.MenuSlab.Content.Input.SnapRotationDeadzoneSetting.Mesh.GetGameObject().GetComponent<MeshFilter>().mesh;
                RockUI.panelMat = RumbleModdingAPI.RMAPI.GameObjects.Gym.INTERACTABLES.MenuSlab.Content.Input.SnapRotationDeadzoneSetting.Mesh.GetGameObject().GetComponent<MeshRenderer>().material;
                //debug();
            }
        }
        private float a = 0.0f;
        private string t = "";
        private void debug()
        {
            RockPanel mainpanel = new RockPanel();
            mainpanel.size = new Vector2(10, 15);

            RockButton debugbutton = new RockButton();
            debugbutton.anchor = UIElement.AnchorType.Center;
            debugbutton.anchorOffset = new Vector2(0, 2);
            debugbutton.pressAction = log;

            RockText debugtext = new RockText();
            debugtext.anchor = UIElement.AnchorType.Center;
            debugtext.anchorOffset = new Vector2(0, 4);
            debugtext.text = "Print Debug Message";

            RockText debugtext2 = new RockText();
            debugtext2.anchor = UIElement.AnchorType.Center;
            debugtext2.anchorOffset = new Vector2(0, -2);
            debugtext2.text = "Number to Print";
            RockSlider debugslider = new RockSlider();
            debugslider.anchor = UIElement.AnchorType.Center;
            debugslider.anchorOffset = new Vector2(0, -4);
            debugslider.valueChangedAction = setAmount;

            RockText debugtext3 = new RockText();
            debugtext3.anchor = UIElement.AnchorType.Center;
            debugtext3.anchorOffset = new Vector2(-3, 2);
            debugtext3.text = "!!!";
            RockLever debuglever = new RockLever();
            debuglever.anchor = UIElement.AnchorType.Center;
            debuglever.anchorOffset = new Vector2(-3, 0);
            debuglever.leverToggledAction = setEnding;

            mainpanel.AddChildUI(debugtext);
            mainpanel.AddChildUI(debugbutton);
            mainpanel.AddChildUI(debuglever);
            mainpanel.AddChildUI(debugtext2);
            mainpanel.AddChildUI(debugtext3);
            mainpanel.AddChildUI(debugslider);

            GameObject debugmenu = RockUI.BuildUI(mainpanel);
            debugmenu.transform.position = new Vector3(0, 1, 0);
        }
        private void log()
        {
            MelonLogger.Msg("Pressed Button, Amount: " + a.ToString() + t);
        }
        private void setAmount(float amount)
        {
            a = 10 * amount;
        }
        private void setEnding(int ending)
        {
            if (ending == 1)
            {
                t = "!!!!!!!!!";
            }
            else
            {
                t = "";
            }
        }
        public class RockUI
        {
            public static GameObject SliderPrefab = null;
            public static GameObject LeverPrefab = null;

            public static Mesh panelMesh = null;
            public static Material panelMat = null;
            public static GameObject BuildUI(UIElement uiElement, GameObject UIRoot = null, GameObject ParentObj = null, UIElement ParentUI = null)
            {
                var g = uiElement.Create();
                if (UIRoot == null)
                {
                    UIRoot = new GameObject();
                }
                if (ParentObj == null)
                {
                    ParentObj = UIRoot;
                }
                g.transform.SetParent(UIRoot.transform, false);
                float offset = 0.068f;
                if (uiElement is RockPanel)
                {
                    offset = 0.054f;
                }
                if (ParentUI is RockPanel)
                {
                    g.transform.position = ParentObj.transform.GetChild((int)uiElement.anchor).position + new Vector3(0, 0, -offset);
                }
                else
                {
                    g.transform.position = ParentObj.transform.position + new Vector3(0, 0, -offset);
                }
                g.transform.localPosition += new Vector3(uiElement.anchorOffset.x, uiElement.anchorOffset.y, 0) / 10;
                if (uiElement is RockSlider)
                {
                    g.transform.localPosition -= new Vector3(0.116f, 0, 0);
                }
                foreach (UIElement childUI in uiElement.childElements)
                {
                    BuildUI(childUI, UIRoot, g, uiElement);
                }
                return UIRoot;
            }
        }

        public abstract class UIElement
        {
            public List<UIElement> childElements = new List<UIElement>();
            public Vector2 anchorOffset = new Vector2(0, 0);
            public enum AnchorType
            {
                TopLeft,
                TopRight,
                BottomLeft,
                BottomRight,
                LeftMiddle,
                RightMiddle,
                TopMiddle,
                BottomMiddle,
                Center,
            }
            public AnchorType anchor = AnchorType.Center;
            public abstract GameObject Create();

            public void AddChildUI(UIElement childUI)
            {
                childElements.Add(childUI);
            }
        }

        public class RockPanel : UIElement
        {
            public Vector2 size = new Vector2(100, 100);

            public override GameObject Create()
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.GetComponent<MeshFilter>().mesh = RockUI.panelMesh;
                g.layer = 10;
                g.transform.localScale = new Vector3(size.x / 10, size.y / 10, 0.12f);
                g.GetComponent<MeshRenderer>().material = RockUI.panelMat;
                for (int i = 0; i < 9; i++)
                {
                    var anchorObj = new GameObject();
                    anchorObj.transform.SetParent(g.transform, false);
                    switch (i)
                    {
                        case 0:
                            anchorObj.transform.localPosition = new Vector3(-0.5f, 0.5f, 0); break;
                        case 1:
                            anchorObj.transform.localPosition = new Vector3(0.5f,0.5f, 0); break;
                        case 2:
                            anchorObj.transform.localPosition = new Vector3(-0.5f, -0.5f, 0); break;
                        case 3:
                            anchorObj.transform.localPosition = new Vector3(0.5f, -0.5f, 0); break;
                        case 4:
                            anchorObj.transform.localPosition = new Vector3(-0.5f, 0, 0); break;
                        case 5:
                            anchorObj.transform.localPosition = new Vector3(0.5f, 0, 0); break;
                        case 6:
                            anchorObj.transform.localPosition = new Vector3(0, 0.5f, 0); break;
                        case 7:
                            anchorObj.transform.localPosition = new Vector3(0, -0.5f, 0); break;
                        case 8:
                            anchorObj.transform.localPosition = new Vector3(0, 0, 0); break;
                    }
                }
                return g;
            }
        }
        public class RockButton : UIElement
        {
            public Action pressAction;
            public override GameObject Create()
            {
                var g = RumbleModdingAPI.RMAPI.Create.NewButton(Vector3.zero, Quaternion.identity, pressAction);
                g.transform.Rotate(90f, 180f, 0f);
                return g;
            }
        }
        public class RockText : UIElement
        {
            public String text;
            public int fontSize = 1;
            public Color fontColor = Color.black;
            public override GameObject Create()
            {
                return RumbleModdingAPI.RMAPI.Create.NewText(text, fontSize, fontColor, UnityEngine.Vector3.zero, Quaternion.identity);
            }
        }

        public class RockSlider : UIElement
        {
            public bool useSteps = false;
            public int stepCount = 2;

            public Action<float> valueChangedAction;
            public Action<int> stepReachedAction;
            public override GameObject Create()
            {
                var g = GameObject.Instantiate(RockUI.SliderPrefab);
                g.transform.rotation = Quaternion.identity;
                g.transform.Rotate(90, 0, 180);
                InteractionSlider s = g.transform.Find("Slider handle").GetComponent<InteractionSlider>();
                s.useSteps = useSteps;
                s.stepCount = stepCount;
                s.onNormalizedValueChanged.RemoveAllListeners();
                s.onStepReached.RemoveAllListeners();
                if (valueChangedAction != null)
                {
                    s.onNormalizedValueChanged.AddListener(valueChangedAction);
                }
                if (stepReachedAction != null)
                {
                    s.onStepReached.AddListener(stepReachedAction);
                }
                return g;
            }
        }
        public class RockLever : UIElement
        {
            public Action<int> leverToggledAction;
            public override GameObject Create()
            {
                var g = GameObject.Instantiate(RockUI.LeverPrefab);
                g.transform.rotation = Quaternion.identity;
                g.transform.Rotate(0, -90, 90);
                InteractionLever s = g.transform.Find("ReferenceTransform/Lever").GetComponent<InteractionLever>();
                s.onStepReached.RemoveAllListeners();
                s.rotatorMax = 15;
                s.rotatorMin = -105;
                if (leverToggledAction != null)
                {
                    s.onStepReached.AddListener(leverToggledAction);
                }
                return g;
            }
        }
    }
}