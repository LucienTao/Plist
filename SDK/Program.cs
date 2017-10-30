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
            var result = AboutPlist.parse("/home/duoyi/SDK/SDK/Plist/app.plist");
            var a = result["items"][0]["assets"][0]["kind"];
            Console.ReadKey();

        }
    }








    class AboutPlist
    {
        public abstract class prototypeNode{
            public abstract prototypeNode this[string key]{get;set;}
            public abstract prototypeNode this[int key]{get;set;}

        }
        // public class Node<TValue>:prototypeNode{
        //     public TValue value{get;set;}
        //     public TValue this[string key]{
        //         get{
        //             return value;
        //         }
        //     }
        // }





        public delegate dynamic ConvertNode(XElement node);
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

        public static dynamic parse(string path)
        {
            XDocument xmlDoc = XDocument.Load(path);
            XElement root = xmlDoc.Root.Element("dict");
            return ConvertDict[NodeType.DICT](root);
        }
        public static dynamic parseDict(XElement input)
        {
            Console.WriteLine("execute parseDict.");
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
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
            //Node<Dictionary<string,prototypeNode>> temp = new Node<Dictionary<string, prototypeNode>>();
            //temp.value = result;
            return result;
        }
        public static dynamic parseArray(XElement input)
        {
            Console.WriteLine("execute parseArray.");
            List<dynamic> a = new List<dynamic>();
            var nodes = input.Nodes().GetEnumerator();
            while (nodes.MoveNext())
            {
                XElement node = (XElement)nodes.Current;
                NodeType type = GetType(node);
                a.Add(ConvertDict[type](node));
            }
            //Node<Array> result = new Node<Array>();
            //result.value = a.ToArray();

            return a;
        }
        public static dynamic parseString(XElement input)
        {
            Console.WriteLine("execute parseString.");
            //Node<string> result = new Node<string>();
            //result.value = input.Value;
            return input.Value;
        }
        public static dynamic parseInteger(XElement input)
        {
            Console.WriteLine("execute parseInteger.");
            //Node<int> result = new Node<int>();
            //result.value = Convert.ToInt32(input.Value);
            return Convert.ToInt32(input.Value);
        }
        public static dynamic parseTrue(XElement input)
        {
            Console.WriteLine("execute parseTrue.");
            //Node<bool> result = new Node<bool>();
            //result.value = input.Name.LocalName.Equals("false") ? false : true;
            return input.Name.LocalName.Equals("false") ? false : true;

        }
        public static dynamic parseFalse(XElement input)
        {
            Console.WriteLine("execute parseFalse.");
            //Node<bool> result = new Node<bool>();
            //result.value = input.Name.LocalName.Equals("false") ? false : true;
            return input.Name.LocalName.Equals("false") ? false : true;
        }
        public static dynamic parseDate(XElement input)
        {
            Console.WriteLine("execute parseDate.");
            //Node<DateTime> result = new Node<DateTime>();
            //result.value = Convert.ToDateTime(input.Value);
            return Convert.ToDateTime(input.Value);
        }
        public static dynamic parseData(XElement input)
        {
            Console.WriteLine("execute parseData.");
            //Node<byte[]> result = new Node<byte[]>();
            //result.value = Convert.FromBase64String(input.Value);
            return Convert.FromBase64String(input.Value);
        }
        public static dynamic parseReal(XElement input)
        {
            Console.WriteLine("execute parseReal.");
            //Node<decimal> result = new Node<decimal>();
            //result.value = Convert.ToDecimal(input.Value);
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
