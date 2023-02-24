using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Xml;
using System;
using System.Text.RegularExpressions;
using System.Linq;
public class SpriteSheetEditor : EditorWindow {

    [MenuItem("CustomTools/SpriteSheetEditor")]
    private static void ShowWindow() {
        var window = GetWindow<SpriteSheetEditor>();
        window.titleContent = new GUIContent("SpriteSheetEditor");
        window.Show();
    }

    string spriteSheetName;
    string resourceDirectory = "units";
    Sprite sprite;
    XmlDocument pListFile; 
    string pListFilePath;
    float animationScale = 1;


    string spritePath;
    private void OnGUI() {
        
        spriteSheetName = EditorGUILayout.TextField("Sprite Sheet Name", spriteSheetName);  
        resourceDirectory = EditorGUILayout.TextField("Resource Directory", resourceDirectory);
        sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), false);
        animationScale = EditorGUILayout.FloatField("Animation Scale",animationScale);
        if(animationScale == 0)
            animationScale = 1;
        if(GUILayout.Button("Choose PList")){
            pListFilePath = EditorUtility.OpenFilePanel("Choose Property List", "", "plist");
        }
        EditorGUILayout.LabelField(pListFilePath.Replace(Application.dataPath, "Assets"));
        


        EditorGUILayout.Space();
        if(GUILayout.Button("Confirm")){
            CreateSpriteSheet();
        }

    }

    private void CreateSpriteSheet(){
        //validation
        if(spriteSheetName == ""){
            Debug.LogError("No file name given");
            return;
        }
        if(sprite == null){
            Debug.LogError("No Sprite Given");
            return;
        }
        if(pListFilePath == ""){
            Debug.Log("No PropertyList given");
            return;
        }


        //Creating the folder && moving the files
        string filePath = Application.dataPath + $"/resources/{resourceDirectory}/{spriteSheetName}";
        System.IO.Directory.CreateDirectory(filePath);
        AssetDatabase.Refresh(); //to ensure directory is found

        string oldPath = AssetDatabase.GetAssetPath(sprite);
        string newPath = $"Assets/resources/{resourceDirectory}/{spriteSheetName}/{spriteSheetName}.png";

        /// https://docs.unity3d.com/ScriptReference/AssetDatabase.ValidateMoveAsset.html
        string moveResult = AssetDatabase.ValidateMoveAsset(oldPath, newPath);
        if(moveResult == "")
            AssetDatabase.MoveAsset(oldPath, newPath);
        else {
            Debug.LogError($"Couldn't move {oldPath} because {moveResult}"); 
        }

        //reading the file
        //string fullPath = Application.dataPath.Replace("Assets", AssetDatabase.GetAssetPath(textAsset)); //replaces the final "Asset" application.datapath with the file locaiton starting with asset 
        List<FrameData> frameData = ParseFrameData(pListFilePath);

        //editing the sprite
        SliceSprite(sprite, new Vector2Int(141,141), frameData);

        //create animations from sprite sheet
        CreateAnimations();
        
    }
    private List<FrameData> ParseFrameData(string filePath){
        pListFile = new();
        pListFile.Load(filePath);
        Dictionary<String, System.Object> topLevel = (Dictionary<String, System.Object>)PlistParser.Parse(pListFile);

        //get the list of all frames
        Dictionary<String, System.Object> frameInfosUnparsed = (Dictionary<String, System.Object>)topLevel["frames"];


        List<FrameData> frameData = new();
        FrameData data = new();
        foreach (KeyValuePair<String, System.Object> entry in frameInfosUnparsed){
            Dictionary<String, System.Object> frameInfo = (Dictionary<String, System.Object>)entry.Value;
            data = new FrameData(
                entry.Key,
                conversionHelper(frameInfo["sourceSize"]),
                conversionHelper(frameInfo["frame"]),
                conversionHelper(frameInfo["offset"]),
                conversionHelper(frameInfo["sourceColorRect"]),
                (bool)frameInfo["rotated"]
            );
            frameData.Add(data);
        }

        return frameData;
        Vector2Int conversionHelper(object obj){
            string input = obj as string;
            string[] parts = input.Replace("{","").Replace("}","").Replace(" ","").Split(',');

            int[] results = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++){
                results[i] = int.Parse(parts[i]);
            }
            return new Vector2Int(results[0], results[1]);
        }
    }
    private void SliceSprite(Sprite sprite, Vector2Int slicingSize, List<FrameData> frameDatas){
        spritePath = AssetDatabase.GetAssetPath(sprite);
        // https://forum.unity.com/threads/solved-slicing-a-sprite-through-script-on-importing.701294/
        TextureImporter textureImporter = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if(textureImporter == null){
            Debug.LogError("Import didnt work");
            Debug.Log(spritePath);
            return;
        }

        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePixelsPerUnit = slicingSize.x / 2; //doubles the size of the model to take up pretty much a one by one square with the sprite itself itself
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.maxTextureSize = 2048;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.crunchedCompression = false;
        textureImporter.compressionQuality = 100;
        textureImporter.isReadable = true;
        textureImporter.textureShape = TextureImporterShape.Texture2D;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
    
        AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
        EditorUtility.SetDirty(textureImporter);


        Texture2D sourceTexture = (Texture2D) AssetDatabase.LoadAssetAtPath(spritePath, typeof(Texture2D));


        List<SpriteMetaData> spriteMetaDatas = new List<SpriteMetaData>();
        SpriteMetaData metaData = new();
        foreach (FrameData data in frameDatas){
            string fileName = Regex.Replace(data.frameName, @".*_.*_(.*)_(.*)\.png", $"{spriteSheetName}_$1$2");
            metaData = new SpriteMetaData{
                name = fileName,
                rect = new Rect(data.frameBasePos.x, sourceTexture.height - data.frameBasePos.y - data.sourceSize.y, data.sourceSize.x, data.sourceSize.y),
                alignment = 0,
                pivot = new Vector2(0,0)
            };
            spriteMetaDatas.Add(metaData);
        }

        textureImporter.spritesheet = spriteMetaDatas.ToArray();
        AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
    }
    private void CreateAnimations(){
        List<Sprite> allSprites = AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().ToList();

        //sort sprites
        Dictionary<String, List<Sprite>> animations = new Dictionary<string, List<Sprite>>();

        foreach (Sprite sprite in allSprites){
            //if it doesnt contain the key
            string key = getAnimationName(sprite.name);
            if(!animations.ContainsKey(getAnimationName(sprite.name)))
                animations.Add(key, new List<Sprite>());
            animations[key].Add(sprite);
        }

        //https://docs.unity3d.com/ScriptReference/Animations.AnimatorController.html
        UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"Assets/resources/{resourceDirectory}/{spriteSheetName}/{spriteSheetName}_Animator.controller");
        controller.AddParameter("OnDeath", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("OnHit", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("OnAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);

        AnimationClip animClip = new();
        foreach (KeyValuePair<String, List<Sprite>> animation in animations){
            animClip = new();
            //animClip.frameRate = ; //unity anim changes sprite once every 0.01
            
            EditorCurveBinding spriteBinding = new EditorCurveBinding();
            spriteBinding.type = typeof(SpriteRenderer);
            spriteBinding.path = "";
            spriteBinding.propertyName = "m_Sprite";
            
            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[animation.Value.Count];
            for (int i = 0; i < animation.Value.Count; i++){
                spriteKeyFrames[i] = new ObjectReferenceKeyframe();
                spriteKeyFrames[i].time = i / animationScale;
                spriteKeyFrames[i].value = animation.Value[i];
            }
            AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);

            AssetDatabase.CreateAsset(animClip, $"Assets/resources/{resourceDirectory}/{spriteSheetName}/{spriteSheetName}_{animation.Key}.anim");
            controller.AddMotion(animClip);
        }

        
        

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        
        string getAnimationName(string spriteName){
            return spriteName.Replace($"{spriteSheetName}_", "").TrimEnd(new char[] {'0', '1', '2', '3', '4', '5', '6','7','8','9'});
        }
    }

    private struct FrameData{
        public FrameData(String _frameName, Vector2Int _sourceSize, Vector2Int _frameBasePos, Vector2Int _offset, Vector2Int _sourceColorBasePos, bool _rotated){
            frameName = _frameName;
            sourceSize = _sourceSize;
            frameBasePos = _frameBasePos;
            offset = _offset;
            sourceColorBasePos = _sourceColorBasePos;
            rotated = _rotated;
        }
        public string frameName;
        public Vector2Int sourceSize;
        public Vector2Int frameBasePos;
        public Vector2Int offset;
        public Vector2Int sourceColorBasePos;
        bool rotated;  
    }
}




///from here https://github.com/open-duelyst/duelyst/blob/075a9cde916bf0c019bee196c6d2d4cfd584bb07/app/vendor/cocos2d-html5/cocos2d/core/platform/CCSAXParser.js#L81 translated using ChatGPT to C#
public static class PlistParser
{
    public static object Parse(XmlDocument plist)
    {
        // Get first real node
        XmlNode node = null;
        for (int i = 0, len = plist.ChildNodes.Count; i < len; i++)
        {
            node = plist.ChildNodes[i];
            if (node.NodeType == XmlNodeType.Element)
                break;
        }
        return ParseNode(node);
    }

    private static object ParseNode(XmlNode node)
    {
        object data = null;
        string tagName = node.Name;

        switch (tagName)
        {
            case "dict":
                data = ParseDict(node);
                break;
            case "array":
                data = ParseArray(node);
                break;
            case "string":
                if (node.ChildNodes.Count == 1)
                    data = node.FirstChild.Value;
                break;
            case "false":
                data = false;
                break;
            case "true":
                data = true;
                break;
            case "real":
                data = float.Parse(node.FirstChild.Value);
                break;
            case "integer":
                data = int.Parse(node.FirstChild.Value);
                break;
            case "plist":
                data = ParseNode(node.FirstChild);
                break;
        }
        return data;
    }

    private static object ParseArray(XmlNode node)
    {
        var data = new List<object>();
        for (int i = 0, len = node.ChildNodes.Count; i < len; i++)
        {
            var child = node.ChildNodes[i];
            if (child.NodeType != XmlNodeType.Element)
                continue;
            data.Add(ParseNode(child));
        }
        return data;
    }

    private static object ParseDict(XmlNode node)
    {
        var data = new Dictionary<string, object>();
        string key = null;
        for (int i = 0, len = node.ChildNodes.Count; i < len; i++)
        {
            var child = node.ChildNodes[i];
            if (child.NodeType != XmlNodeType.Element)
                continue;

            // Grab the key, next node should be the value
            if (child.Name == "key")
                key = child.FirstChild.Value;
            else
                data[key] = ParseNode(child); // Parse the value node
        }
        return data;
    }
}