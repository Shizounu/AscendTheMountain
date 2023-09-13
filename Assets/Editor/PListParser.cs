using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace Editor.SpriteSheedEditor {
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
}