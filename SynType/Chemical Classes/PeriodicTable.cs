using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace SynType.Chemical_Classes
{
    public class PeriodicTable
    {
        Dictionary<string, Element> elements; //a string(symbol) is attached to an element instance
        public Dictionary<string, Element> Elements { get { return elements; } set { elements = value; } }
        public PeriodicTable()
        {
            elements = new Dictionary<string, Element>();
            BuildTable();
        }
        public void BuildTable()
        {
            Dictionary<string, Element> tempDict = new Dictionary<string, Element>();
            XmlDocument doc = new XmlDocument();
            doc.Load("periodic.xml");
            XmlNodeList xmlElements = doc.GetElementsByTagName("ATOM");
            foreach (XmlNode e in xmlElements)
            {
                string name = "", symbol = "";
                int number = 0, phase =(int) PHASE_STATE.Solid;
                float weight = 0.0f, bp = 0.0f, mp = 0.0f, density = 0.0f;
                foreach (XmlNode node in e.ChildNodes)
                {
                    switch(node.Name)
                    {
                        case "NAME": name = node.InnerText;
                            break;
                        case "SYMBOL": symbol = node.InnerText;
                            break;
                        case "ATOMIC_WEIGHT": float.TryParse(node.InnerText, out weight);
                            break;
                        case "ATOMIC_NUMBER": int.TryParse(node.InnerText, out number);
                            break;
                        case "BOILING_POINT": float.TryParse(node.InnerText, out bp);
                            break;
                        case "MELTING_POINT": float.TryParse(node.InnerText, out mp);
                            break;
                        case "DENSITY": float.TryParse(node.InnerText, out density);
                            break;
                        
                    
                    }
                    
                }

                if (mp > 300) //solid above 300 
                    phase = (int)PHASE_STATE.Solid;
                else if (mp <= 300) //a liquid at 300
                    phase = (int)PHASE_STATE.Liquid;
                else if (bp < 300) //gas below 300
                    phase = (int)PHASE_STATE.Gas;
                Element elementToAdd = new Element(name, symbol, number, weight, bp, mp, density, phase);
                tempDict.Add(elementToAdd.Symbol, elementToAdd);
            }
       
            //Last we sort the dictionary by atomic number
            var items = from symb in tempDict.Keys
                        orderby tempDict[symb].Number ascending
                        select symb;
           ;
         
            
            foreach (var e in items)
            {
                elements.Add(e, tempDict[e]);
            }
           
        }
        public void PrintTable()
        {
            foreach (string s in elements.Keys)
            {
                Console.WriteLine(elements[s].ToString());
            }
        }
    }
    public class Element
    {
        string name;
        string symbol;
        int atomic_number;
        float weight;
        float boilingPoint;
        float meltingPoint;
        float density; //the density at temperature K with density g/cm^3
        int phase; //set to one of the phase_state enums

        public string Name { get { return name; }}
        public string Symbol { get { return symbol; } }
        public int Number { get { return atomic_number; } }
        public float Weight { get { return weight; } }
        public float BoilingPoint { get { return boilingPoint; } }
        public float MeltingPoint { get { return meltingPoint; } }
        public float Density { get { return density; } }
        public int Phase { get { return phase; } }
        public Element(string name, string symbol, int number, float weight, float bp, 
            float mp, float density, int phase)
        {
            this.name = name;
            this.symbol = symbol;
            this.atomic_number = number;
            this.weight = weight;
            this.phase = phase;
            this.boilingPoint = bp;
            this.meltingPoint = mp;
            this.density = density;
        }
        public override string ToString()
        {
            string rString = name + " (" + symbol + ") #" + atomic_number + " MW:" + weight +
                "\nPhase at RT: " + (PHASE_STATE)phase;
            return rString;
        }
    }
    public enum PHASE_STATE
    {
        Solid = 0,
        Liquid,
        Gas,
        Solution
    }
}
