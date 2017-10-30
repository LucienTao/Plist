using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace SDK
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = (Dictionary<string,object>)AboutPlist.parse("/home/duoyi/SDK/SDK/Plist/app.plist");
            var a = result["items"];
            var type = a.GetType();
            Console.ReadKey();

        }
    }

    class AboutPlist
    {
        
        class Node{
            public object value;






        }
        // public static Dictionary<string, object> result = new Dictionary<string, object>();
        // public static Stack<object> temp = new Stack<object>();
        // public static Stack<string> keys = new Stack<string>();
        public delegate object ConvertNode(XElement node);
        public static Dictionary<NodeType, ConvertNode> ConvertDict = new Dictionary<NodeType, ConvertNode>(){
            
            {NodeType.DICT,new ConvertNode(parseDict)},
            {NodeType.ARRAY,new ConvertNode(parseArray)},
            {NodeType.STRING,new ConvertNode(parseString)},
            {NodeType.INTEGER,new ConvertNode(parseInteger)},
            {NodeType.REAL,new ConvertNode(parseReal)},
            {NodeType.DATA,new ConvertNode(parseData)},
            {NodeType.DATE,new ConvertNode(parseDate)},
            {NodeType.TRUE,new ConvertNode(parseTrue)},
            {NodeType.FALSE,new ConvertNode(parseFalse)}
        };



        public enum NodeType
        {
            KEY = 00,
            DICT = 10,
            ARRAY = 20,
            DATA = 30,
            STRING = 40,
            INTEGER = 50,
            DATE = 60,
            REAL = 70,
            FALSE = 80,
            TRUE = 90
        }
        public static NodeType GetType(XElement element)
        {
            return (NodeType)Enum.Parse(typeof(NodeType), element.Name.LocalName.ToUpper());
        }

        public static string[] mark(NodeType type)
        {
            Dictionary<NodeType, string[]> markPairs = new Dictionary<NodeType, string[]>(){
                {NodeType.KEY,new string[]{"<key>","</key>","<key","</ke"}},
                {NodeType.DICT,new string[]{"<dict>","</dict>","<dic","</di"}},
                {NodeType.ARRAY,new string[]{"<array>","</array>","<arr","</ar"}},
                {NodeType.DATA,new string[]{"<data>","</data>","<da"}},
                {NodeType.STRING,new string[]{"<string>","</string>"}},
                {NodeType.INTEGER,new string[]{"<integer>","</integer>"}},
                {NodeType.DATE,new string[]{"<date>","</date>"}},
                {NodeType.REAL,new string[]{"<real>","</real>"}},
                {NodeType.FALSE,new string[]{"<false/>"}},
                {NodeType.TRUE,new string[]{"<true/>"}}
            };
            return markPairs[type];
        }

        public static object parse(string path)
        {
            XDocument xmlDoc = XDocument.Load(path);
            XElement root = xmlDoc.Root.Element("dict");
            return ConvertDict[NodeType.DICT](root);
        }
        public static object parseDict(XElement input)
        {
            Console.WriteLine("execute parseDict.");
            Dictionary<string, object> result = new Dictionary<string, object>();
            var nodes = input.Nodes().GetEnumerator();
            while (nodes.MoveNext())
            {
                XElement eKey = (XElement)nodes.Current;
                if (eKey.Name.LocalName.Equals("key"))
                {
                    //keys.Push(eKey.Value);
                    nodes.MoveNext();
                    if (nodes.Current == null)
                    {
                        throw new ArgumentException("字典节点中找不到key对应的value");
                    }
                    else
                    {
                        XElement eValue = (XElement)nodes.Current;
                        NodeType type = GetType(eValue);
                        result.Add(eKey.Value, ConvertDict[type](eValue));
                    }

                }
                else
                {
                    throw new ArgumentException("字典节点中找不到对应key");
                }
            }
            return result;
        }
        public static object parseArray(XElement input)
        {
            Console.WriteLine("execute parseArray.");
            List<object> a = new List<object>();
            var nodes = input.Nodes().GetEnumerator();
            while (nodes.MoveNext())
            {
                XElement node = (XElement)nodes.Current;
                NodeType type = GetType(node);
                a.Add(ConvertDict[type](node));
            }
            return a.ToArray();
        }
        public static object parseString(XElement input)
        {
            Console.WriteLine("execute parseString.");
            return input.Value;
        }
        public static object parseInteger(XElement input)
        {
            Console.WriteLine("execute parseInteger.");
            return Convert.ToInt32(input.Value);
        }
        public static object parseTrue(XElement input)
        {
            Console.WriteLine("execute parseTrue.");
            return input.Name.LocalName.Equals("false") ? false : true;

        }
        public static object parseFalse(XElement input)
        {
            Console.WriteLine("execute parseFalse.");
            return input.Name.LocalName.Equals("false") ? false : true;
        }
        public static object parseDate(XElement input)
        {
            Console.WriteLine("execute parseDate.");
            return Convert.ToDateTime(input.Value);
        }
        public static object parseData(XElement input)
        {
            Console.WriteLine("execute parseData.");
            return Convert.FromBase64String(input.Value);
        }
        public static object parseReal(XElement input)
        {
            Console.WriteLine("execute parseReal.");
            return Convert.ToDecimal(input.Value);
        }

    }




    class AboutMD5
    {
        public static void CalFilesMd5(string path = null)
        {
            string root = Directory.GetCurrentDirectory() + @"\MD5\";
            string rn = "\r\n";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                root = Directory.GetCurrentDirectory() + "/MD5/";
                rn = "\n";
            }
            Dictionary<string, string> result = new Dictionary<string, string>();
            DirectoryInfo directory = new DirectoryInfo(root);
            FileSystemInfo[] filesArray = directory.GetFileSystemInfos();
            foreach (var file in filesArray)
            {
                if (file.Attributes != FileAttributes.Directory)
                {
                    result.Add(file.Name, GetMd5Hash(file.FullName));
                }
            }
            using (FileStream fs = new FileStream(root + "result.txt", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                foreach (var pair in result)
                {
                    sw.Write(pair.Key + ":" + pair.Value + rn);
                }
                sw.Close();
                sw.Dispose();
            }
            Console.ReadKey();
        }
        public static string GetMd5Hash(String path)
        {
            string result = null;
            using (MD5 temp = MD5.Create())
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    byte[] data = temp.ComputeHash(fs);
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    result = sBuilder.ToString();
                }
            }
            return result;
        }
    }
}
