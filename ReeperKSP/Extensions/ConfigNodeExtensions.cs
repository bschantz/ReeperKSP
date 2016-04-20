using System;
using System.IO;
using System.Linq;
using ReeperCommon.Containers;
using ReeperCommon.Extensions;

namespace ReeperKSP.Extensions
{
    public static class ConfigNodeExtensions
    {
        /// <summary>
        /// Parse an Enum type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(this ConfigNode node, string valueName, T defaultValue)
        {
            if (!node.HasValue(valueName))
                return defaultValue;

            var value = node.GetValue(valueName);

            return (T)Enum.Parse(typeof(T), value, true);
        }



        /// <summary>
        /// Parse a value from a ConfigNode
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ParseWithDefault<T>(this ConfigNode node, string valueName, T defaultValue)
        {
            if (!node.HasValue(valueName))
                return defaultValue;

            var value = node.GetValue(valueName);

            if (typeof(T) == typeof(string) || typeof(T) == typeof(String))
                return (T)(object)value;

            var method = typeof(T).GetMethod("TryParse", new[] {
                typeof (string),
                typeof(T).MakeByRefType()
            });

            if (method.IsNull())
                throw new MissingMethodException(typeof (T).FullName, "TryParse");
                
            var args = new object[] { value, default(T) };

            if ((bool)method.Invoke(null, args))
                return (T)args[1];    

            throw new Exception(string.Format("Failed to invoke TryParse with {0}", value));
        }



        /// <summary>
        /// Set a value in a ConfigNode. If the value already exists, the existing value is changed
        /// otherwise a new entry iscreated
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        public static void Set<T>(this global::ConfigNode node, string valueName, T value)
        {
            if (node.HasValue(valueName))
                node.SetValue(valueName, value.ToString());
            else node.AddValue(valueName, value);
        }


        public static void Write(this ConfigNode node, string fullPath, string header)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentException("Invalid path: " + fullPath, "fullPath");

            
            using (var stream = new StreamWriter(new FileStream(fullPath, FileMode.Create, FileAccess.Write)))
            {
                if (!string.IsNullOrEmpty(header))
                    stream.WriteLine(header.StartsWith("\\\\") ? header : "\\\\ " + header.Trim());

                WriteNode(stream, node, 0);
            }
        }


        private static void WriteNode(TextWriter stream, ConfigNode node, int depth)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (node == null) throw new ArgumentNullException("node");
            if (depth < 0)
                throw new ArgumentException("Invalid depth: " + depth, "depth");


            WriteNestedLine(stream, depth, node.name.Trim());
            WriteNestedLine(stream, depth, "{");

            {
                // write all values of this node
                foreach (ConfigNode.Value value in node.values)
                    WriteNestedLine(stream, depth + 1,
                        value.name.Trim() + " = " + value.value.Trim());
            }

            if (node.CountValues > 0 && node.CountNodes > 0)
                stream.Write(stream.NewLine);

            { // write all sub nodes of this node
                foreach (ConfigNode sub in node.nodes)
                    WriteNode(stream, sub, depth + 1);
            }

            WriteNestedLine(stream, depth, "}");
        }


        private static void WriteNestedLine(TextWriter stream, int depth, string data)
        {
            stream.Write(new string('\t', depth));
            stream.Write(data);
            stream.Write(stream.NewLine);
        }


        public static string ToSafeString(this ConfigNode config)
        {
            return config == null ? "<null ConfigNode>" : config.ToString().Replace("{", "{{").Replace("}", "}}"); // these are to escape the brackets which may otherwise be interpeted by String.Format as format tokens
        }


        public static string GetValueOrDefault<T>(this ConfigNode config, string valueName, string defaultValue = "", bool caseSensitive = true)
        {
            return config.GetValueEx(valueName, caseSensitive).Or(defaultValue);
        }


        public static Maybe<string> GetValueEx(this ConfigNode config, string valueName, bool caseSensitive = true)
        {
            if (string.IsNullOrEmpty(valueName))
                throw new ArgumentException("Must provide a valueName", "valueName");

            if (!caseSensitive) valueName = valueName.ToUpperInvariant();

            for (int i = 0; i < config.CountValues; ++i)
                if (string.Equals(caseSensitive ? config.values[i].name : config.values[i].name.ToUpperInvariant(),
                    valueName))
                {
                    return ParseWithDefault(config, config.values[i].name, string.Empty).ToMaybe();
                }

            return Maybe<string>.None;
        }


        public static Maybe<ConfigNode> GetNodeEx(this ConfigNode config, string nodeName, bool caseSensitive = true)
        {
            if (string.IsNullOrEmpty(nodeName))
                throw new ArgumentException("Must provide a nodeName", "nodeName");

            if (!caseSensitive) nodeName = nodeName.ToUpperInvariant();

            for (int i = 0; i < config.CountNodes; ++i)
                if (string.Equals(caseSensitive ? config.nodes[i].name : config.nodes[i].name.ToUpperInvariant(),
                    nodeName))
                    return config.nodes[i].ToMaybe();

            return Maybe<ConfigNode>.None;
        }



        public static bool MatchesContentsOf(this ConfigNode us, ConfigNode other, bool ignoreTopLevelNodeName = false)
        {
            if (other == null) return false;
            if (us == null) return false;

            if (!ignoreTopLevelNodeName && !string.Equals(us.name, other.name))
                return false;

            if (us.CountNodes != other.CountNodes || us.CountValues != other.CountValues)
                return false;

            var ourValues = us.values.Cast<ConfigNode.Value>().ToList();
            var theirValues = other.values.Cast<ConfigNode.Value>().ToList();

            foreach (var ourValue in ourValues)
            {
                var localValue = ourValue;
                var target = theirValues.FirstOrDefault(v => v.name == localValue.name && v.value == localValue.value);

                if (target == null) return false;

                theirValues.Remove(target);
            }

            if (theirValues.Any())
                return false;

            var ourNodes = us.nodes.Cast<ConfigNode>().ToList();
            var theirNodes = other.nodes.Cast<ConfigNode>().ToList();

            foreach (var ourNode in ourNodes)
            {
                var localNode = ourNode;
                var target = theirNodes.FirstOrDefault(v => v.name == localNode.name && v.MatchesContentsOf(localNode));

                if (target == null) return false;

                theirNodes.Remove(target);
            }

            if (theirNodes.Any())
                return false;

            return true;
        }
    }
}
