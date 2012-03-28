using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SynType.Math_Classes
{
    [TypeConverter(typeof(UnitConverter))]
    //A physical unit comprised of a value in a given unit type, i.e. 1 mol, mol being the unit
    public class Unit
    {
        public double BaseValue { get { Console.WriteLine("Returning base value " + Value + " * " + Powers[Power]); return Value * Powers[Power]; } }
        public double Value { get; set; }
        public string UnitPrefix { get { return GetUnitPrefix(); } }
        public string UnitType { get; set; }
        public int Power { get; set; }
        public Dictionary<int, double> Powers = new Dictionary<int, double>()
        {
            {0, 1},
            {1, Math.Pow(10, -6)},
            {2, Math.Pow(10, -3) },
            {3, Math.Pow(10, 3) }
        };
      
        public Unit(double value, string unitType, int power)
        {
            Value = value;
            UnitType = unitType;
            Power = power;

            if (power == (int)UNIT_POWERS.none)
            {
                // sometimes we are fed base values with no power as our value, this can be converted to a new value with a power, f.e. 0.001g = 1mg
                //To do this we multiply the value we have by powers of 10 and see if it is in range of a given power
                double tempValue = Value * Math.Pow(10, 6); //10^6 is the reverse of u 
                if (tempValue >= 1 && tempValue <= 999)
                {
                    Value = tempValue;
                    Power = (int)UNIT_POWERS.u;
                    return;
                }

                tempValue = Value * Math.Pow(10, 3); // milli
                if (tempValue >= 1 && tempValue <= 999)
                {
                    Value = tempValue;
                    Power = (int)UNIT_POWERS.m;
                    return;
                }
                tempValue = Value * Math.Pow(10, -3); // kg
                if (tempValue >= 1 && tempValue <= 999)
                {
                    Value = tempValue;
                    Power = (int)UNIT_POWERS.k;
                    return;
                }
            }
        }
        public string GetUnitPrefix()
        {
            if (Power == (int)UNIT_POWERS.none)
            {
                return string.Empty;
            }
            else if (Power == (int)UNIT_POWERS.u)
            {
                return "u";
            }
            else if (Power == (int)UNIT_POWERS.m)
            {
                return "m";
            }
            else if (Power == (int)UNIT_POWERS.k)
            {
                return "k";
            }
            else
                return string.Empty;
        }
        public static int GetUnitPower(string prefix)
        {
            if (prefix == string.Empty)
                return (int)UNIT_POWERS.none;
            else if (prefix == "u")
                return (int)UNIT_POWERS.u;
            else if (prefix == "m")
                return (int)UNIT_POWERS.m;
            else if (prefix == "k")
                return (int)UNIT_POWERS.k;
            else
                return (int)UNIT_POWERS.none;
        }
        public override string ToString()
        {
            if (Value <= Math.Pow(10, -6))
                return Value + " " + UnitPrefix + UnitType;
            else
                return Math.Round(Value, 5) + " " + UnitPrefix + UnitType;
        }
    }
    //Units can be multiplied by powers of 10 to convert them to micro, milli, kilo
    public enum UNIT_POWERS
    {
        none = 0,
        u = 1, //micro
        m = 2, //milli
        k = 3

    }

    public class UnitConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                //break the string # unitprefixunittype into value and unit type and prefix
                string[] splits = ((string)value).Split(' ');
                if (splits.Length <= 1)
                    return new Unit(0, "null", (int)UNIT_POWERS.none);
                //The first index should be the value so we try and get that
                double unitValue = 0; string unitPrefix = string.Empty; string unitType = string.Empty;
                bool unitValueParsed = double.TryParse(splits[0], out unitValue);
                if (!unitValueParsed)
                {
                    //We failed so return an empty unit of null
                    return new Unit(0, "null", (int)UNIT_POWERS.none);
                }
                //next we figure out the type and prefix, we can get between 1 and 4 letters to determine this
                if (splits[1].Length == 1)
                {
                    //if its length is 1 then we don't have a prefix, so its gonna be gram or liter
                    unitType = splits[1][0].ToString();
                }
                else if (splits[1].Length == 2)
                {
                    //if its 2 we have a prefix and either gram or liter
                    unitPrefix = splits[1][0].ToString();
                    unitType = splits[1][1].ToString();
                }
                else if (splits[1].Length == 3)
                {
                    //if its 3 then we just have mol
                    unitType = "mol";
                }
                else if (splits[1].Length == 4)
                {
                    //and if 4 then we have a prefix and mol
                    unitPrefix = splits[1][0].ToString();
                    unitType = "mol";
                }
                else if (splits[1].Length == 5)
                {
                    //if 5 we have mol/l
                    unitType = "mol/l";
                }
                else if (splits[1].Length == 6)
                {
                    //if 6 then we have prefix and mol/l
                    unitPrefix = splits[1][0].ToString();
                    unitType = "mol/l";
                }
                else
                {
                    return new Unit(0, "null", (int)UNIT_POWERS.none);
                }
                return new Unit(unitValue, unitType, Unit.GetUnitPower(unitPrefix));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
