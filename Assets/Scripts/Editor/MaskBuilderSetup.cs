using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Editor script to create the mask builder carousel structure.
/// Run from menu: Tools > Setup Mask Builder
/// </summary>
public class MaskBuilderSetup : EditorWindow
{
    [MenuItem("Tools/Setup Mask Builder")]
    public static void ShowWindow()
    {
        GetWindow<MaskBuilderSetup>("Mask Builder Setup");
    }

    private Transform parentTransform;
    
    void OnGUI()
    {
        GUILayout.Label("Mask Builder Carousel Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        parentTransform = EditorGUILayout.ObjectField("Parent (Canvas or Panel)", parentTransform, typeof(Transform), true) as Transform;
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Feature Picker Structure"))
        {
            if (parentTransform == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a parent transform (Canvas or Panel)", "OK");
                return;
            }
            
            CreatePickerStructure();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("This will create:", EditorStyles.miniLabel);
        GUILayout.Label("- FeaturePicker with VerticalCarousel", EditorStyles.miniLabel);
        GUILayout.Label("- Pickers for: Faces, Eyes, Ears, Chin, Horns,", EditorStyles.miniLabel);
        GUILayout.Label("  Hair, Beard, Stash, Noses, Mouths, Brows", EditorStyles.miniLabel);
        GUILayout.Label("- Each with FolderBasedCarousel", EditorStyles.miniLabel);
    }

    void CreatePickerStructure()
    {
        // Feature folder names matching Resources folders
        string[] features = { "Faces", "Eyes", "Ears", "Chin", "Horns", "Hair", "Beard", "Stash", "Noses", "Mouths", "Brows" };
        
        // Create FeaturePicker parent
        GameObject featurePicker = new GameObject("FeaturePicker");
        featurePicker.transform.SetParent(parentTransform, false);
        
        RectTransform fpRect = featurePicker.AddComponent<RectTransform>();
        fpRect.anchoredPosition = Vector2.zero;
        fpRect.sizeDelta = new Vector2(300, 200);
        
        VerticalCarousel vCarousel = featurePicker.AddComponent<VerticalCarousel>();
        PickerController controller = featurePicker.AddComponent<PickerController>();
        
        // Create pickers array
        RectTransform[] pickerRects = new RectTransform[features.Length];
        GameObject[] pickerObjects = new GameObject[features.Length];
        
        for (int i = 0; i < features.Length; i++)
        {
            string featureName = features[i];
            
            // Create picker container
            GameObject picker = new GameObject($"{featureName}_Picker");
            picker.transform.SetParent(featurePicker.transform, false);
            
            RectTransform pickerRect = picker.AddComponent<RectTransform>();
            pickerRect.anchoredPosition = Vector2.zero;
            pickerRect.sizeDelta = new Vector2(200, 150);
            
            // Add FolderBasedCarousel
            FolderBasedCarousel fbc = picker.AddComponent<FolderBasedCarousel>();
            fbc.resourceFolderName = featureName;
            
            // Create DisplayImage child
            GameObject displayObj = new GameObject("DisplayImage");
            displayObj.transform.SetParent(picker.transform, false);
            
            RectTransform displayRect = displayObj.AddComponent<RectTransform>();
            displayRect.anchoredPosition = Vector2.zero;
            displayRect.sizeDelta = new Vector2(100, 100);
            
            Image displayImage = displayObj.AddComponent<Image>();
            fbc.displayImage = displayImage;
            
            // Hide all except first
            picker.SetActive(i == 0);
            
            pickerRects[i] = pickerRect;
            pickerObjects[i] = picker;
        }
        
        // Setup VerticalCarousel
        vCarousel.pickers = pickerRects;
        vCarousel.slideDuration = 0.5f;
        vCarousel.slideDistance = 150f;
        
        // Setup PickerController
        controller.verticalCarousel = vCarousel;
        controller.pickers = pickerObjects;
        controller.debugMode = true;
        
        // Select the created object
        Selection.activeGameObject = featurePicker;
        
        EditorUtility.DisplayDialog("Success", 
            $"Created FeaturePicker with {features.Length} pickers!\n\n" +
            "Now assign:\n" +
            "- Up/Down buttons to VerticalCarousel\n" +
            "- Left/Right buttons to PickerController\n" +
            "- MaskView images to each FolderBasedCarousel", 
            "OK");
        
        Debug.Log("[MaskBuilderSetup] Created FeaturePicker structure with all pickers!");
    }
}
