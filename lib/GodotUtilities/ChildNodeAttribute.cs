using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

/* -------------------------------------------------------------------------- */
/*              Original author is https://github.com/firebelley              */
/* -------------------------------------------------------------------------- */
/* -------- Pulled from https://github.com/firebelley/GodotUtilities -------- */

namespace Godot
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class NodeAttribute : Attribute
    {
        public string NodePath { get; }

        public NodeAttribute(string nodePath = null)
        {
            NodePath = nodePath;
        }
    }

    public static class ChildNodeAttributeExtension
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static void WireNodes(this Node n)
        {
            var lowerCaseChildNameToChild = n.GetChildren().Cast<Node>().ToDictionary(x => x.Name.ToLower(), x => x);

            var fields = n.GetType().GetFields(BINDING_FLAGS);
            foreach (var memberInfo in fields)
            {
                SetChildNode(n, memberInfo, lowerCaseChildNameToChild);
                SetParentNode(n, memberInfo);
            }

            var properties = n.GetType().GetProperties(BINDING_FLAGS);
            foreach (var memberInfo in properties)
            {
                SetChildNode(n, memberInfo, lowerCaseChildNameToChild);
                SetParentNode(n, memberInfo);
            }
        }

        private static void SetChildNode(Node node, MemberInfo memberInfo, Dictionary<string, Node> lowerCaseChildNameToChild)
        {
            var attribute = Attribute.GetCustomAttribute(memberInfo, typeof(NodeAttribute));
            if (!(attribute is NodeAttribute childNodeAttribute))
            {
                return;
            }

            Node childNode;
            if (childNodeAttribute.NodePath != null)
            {
                childNode = node.GetNodeOrNull(childNodeAttribute.NodePath);
            }
            else
            {
                var memberNameLower = memberInfo.Name.ToLower();
                var lookupSuccess = lowerCaseChildNameToChild.TryGetValue(memberNameLower, out childNode);
                if (!lookupSuccess)
                {
                    childNode = lowerCaseChildNameToChild
                        .Where(x => memberNameLower.Contains(x.Key))
                        .OrderByDescending(x => x.Key.Length)
                        .FirstOrDefault().Value;

                    if (childNode == null)
                    {
                        childNode = TryGetUniqueNode(node, memberInfo);
                    }
                }
            }
            if (childNode == null)
            {
                var filename = !string.IsNullOrEmpty(node.Filename) ? node.Filename : "the scene.";
                GD.PrintErr($"Could not match member {memberInfo.Name} to any Node in {filename}.");
            }

            Type memberType = memberInfo.GetUnderlyingType();
            if (!memberType.IsAssignableFrom(childNode.GetType())) {
                GD.PrintErr($"Could not match member {memberInfo.Name} to any Node of type {memberType}. Found {childNode.GetType()}.");
            }

            SetMemberValue(node, memberInfo, childNode);
        }

        private static void SetParentNode(Node node, MemberInfo memberInfo)
        {
            var attribute = Attribute.GetCustomAttribute(memberInfo, typeof(ParentAttribute));
            if (!(attribute is ParentAttribute))
            {
                return;
            }

            Node parentNode = node.GetParent();
            SetMemberValue(node, memberInfo, parentNode);
        }

        private static void SetMemberValue(Node node, MemberInfo memberInfo, Node childNode)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(node, childNode);
            }
            else if (memberInfo is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(node, childNode);
            }
        }

        private static Node TryGetUniqueNode(Node node, MemberInfo memberInfo)
        {
            var name = memberInfo.Name;
            var child = node.GetNodeOrNull($"%{name}");
            if (child == null && name.Length > 1)
            {
                name = name[0].ToString().ToUpper() + name.Substring(1);
                child = node.GetNodeOrNull($"%{name}");
            }
            return child;
        }

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                    "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
    }
}