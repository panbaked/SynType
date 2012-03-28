using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SynType.File_ReadWrite
{
    static class CDXFileReader
    {
        private static CDXString ReadString(BinaryReader reader, UInt16 count)
        {
            //Get the styleCount
            UInt16 styleCount = (UInt16) reader.ReadInt16();
            //If the stylecount is zero we have default ISO Latin 1 no font or size
            CDXFontStyle[] fontStyles = new CDXFontStyle[styleCount];
            if (styleCount > 0)
            {
                //Now we know how many styles we have, so loop till we have them all
                for (int i = 0; i < styleCount; i++)
                {
                    fontStyles[i] = ReadFont(reader);
                }
            }
            int textCount = count - 8*fontStyles.Length- 4; //the total count - number of font structures - 4 bytes for the stylecount
            byte[] textData = reader.ReadBytes(textCount);
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string text = enc.GetString(textData);
            
            return new CDXString(styleCount, fontStyles, text, PropertyTypes.String.ToString());
            
        }
        private static CDXCoordinate[] Parse2DCoordinate(byte[] data)
        {
            //The coords come in as y,x (int32s) so we switch them to x y
            CDXCoordinate[] rCoords = new CDXCoordinate[2];
            Int32 y = BitConverter.ToInt32(data, 0);
            Int32 x = BitConverter.ToInt32(data, 4);
            rCoords[0].coordinate = x;
            rCoords[1].coordinate = y;
            return rCoords;
        }
        private static void ReadPast(BinaryReader reader, UInt16 count)
        {
            reader.ReadBytes(count);
        }
        private static CDXFontStyle ReadFont(BinaryReader reader)
        {
            UInt16 fontIndex = (UInt16)reader.ReadInt16();
            UInt16 fontType = (UInt16)reader.ReadInt16();
            UInt16 fontSize = (UInt16)reader.ReadInt16();
            UInt16 fontColor = (UInt16)reader.ReadInt16();

            return new CDXFontStyle(fontIndex, fontType, fontSize, fontColor, PropertyTypes.FontStyle.ToString());
        }
    
        //An object can contain any number of properties or objects
        private static CDXObject ReadObject(BinaryReader reader, byte[] id)
        {
            
            CDXObject obj = new CDXObject();
            //Get the objectID
            byte[] objectID = reader.ReadBytes(4);
            obj.objectID = objectID;
            obj.tagID = id;
            
            //read next two bytes if they are 00 00 we are at the end of the object
            byte[] nextID = reader.ReadBytes(2);
            if (nextID.Length == 0) return null; //we have read to the end of the file

            UInt32 objIDNum = BitConverter.ToUInt32(objectID, 0);
            string s = String.Format("{0:x2}", objIDNum);
            string typeString = String.Format("{0:x2}", BitConverter.ToUInt16(id, 0));
            Console.WriteLine("Start of object " +s + " type "+typeString);
            Int16 nextIDInt = BitConverter.ToInt16(nextID,0);
            while (nextIDInt != 0) 
            {
                //get the bits of the nextID
                BitArray nextIDBits = new BitArray(nextID);
                Property propToAdd; CDXObject objToAdd;
                if (nextIDBits[15] == false) // we have a property
                {
                    //Read in the property, assign its parent and assign it to the base cdxobject as a child
                    propToAdd = ReadProperty(reader, nextID);
                    propToAdd.parent = obj;
                    obj.childProperties.Add(propToAdd);
                    
                }
                else
                {
                    objToAdd = ReadObject(reader, nextID);
                    objToAdd.parentObject = obj;
                    obj.childObjects.Add(objToAdd);
                }
                //get the next id
                nextID = reader.ReadBytes(2);
                nextIDInt = BitConverter.ToInt16(nextID, 0);
                if (nextIDInt == 0) break;
            }
            Console.WriteLine("--" + obj.childProperties.Count + " properties");
            Console.WriteLine("End of object " + s );
            return obj;
        }
        private static Property ReadProperty(BinaryReader reader, byte[] id)
        {
            //We now get the 2-byte length of the property
            byte[] byteCount = reader.ReadBytes(2);
            UInt16 count;
            try
            {
                count = BitConverter.ToUInt16(byteCount, 0);
                UInt16 idTag = BitConverter.ToUInt16(id, 0);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Reached end of file?");
                count = 0;
            }

            if (count == 0xFFFF)
            {
                //If the count is maxed then there are more than 65534 bytes for this property, read the next four bytes for the actual length
                Console.WriteLine("There's a fuckton of bytes for this property");
            }
            

            byte[] data = reader.ReadBytes(count);

            return new Property(id, data);
           

        }
        private static CDXObject ReadCDXFile()
        {
            using (FileStream input = File.OpenRead("isopropylamide.cdx"))
            {
                using (BinaryReader reader = new BinaryReader(input))
                {
                    System.Text.Encoding enc = System.Text.Encoding.ASCII;
                    //Read the top VjCD0100
                    byte[] headerTop = reader.ReadBytes(8);
                    string headerTopString = enc.GetString(headerTop);
                    if (headerTopString != "VjCD0100")
                    {
                        Console.WriteLine("This is not a *.cdx file");
                        return null;
                    }
                    //Read in the 4 reserved bytes
                    byte[] headerReserved = reader.ReadBytes(4);
                    int count = 4;
                    for (int i = 0; i < 4; i++)
                    {
                        if (headerReserved[i] != count)
                        {
                            Console.WriteLine("This is not a *.cdx file");
                            return null;
                        }
                        count--;
                    }
                    //Read the 16 bytes of 0
                    byte[] headerZeros = reader.ReadBytes(16);


                    //Read in the tag identifier and get its bits
                    byte[] id = reader.ReadBytes(2);
                    BitArray idBits = new BitArray(id);
                    Int16 idNum = BitConverter.ToInt16(id, 0);
                    List<Property> headerProperties = new List<Property>();
                    while (idBits[15] == false && reader.PeekChar() != -1)
                    {
                        Property p = ReadProperty(reader, id);
                        headerProperties.Add(p);
                        id = reader.ReadBytes(2);
                        idBits = new BitArray(id);
                    }
                    Console.WriteLine("done reading start props");
                    CDXObject docObject = new CDXObject();
                    while (reader.PeekChar() != -1)
                    {
                        CDXObject holdObject = ReadObject(reader, id);
                        if (holdObject != null) docObject = holdObject;
                    }
                    return docObject;
                    
                }

            }
        }
        private static void ParseCDXFile(CDXObject docObject)
        {
            //First we get the nodes, and then we get the bonds
            List<STNode> nodeList = new List<STNode>();
            List<STBond> bondList = new List<STBond>();
            foreach (CDXObject obj in docObject.childObjects)
            {
                UInt16 id = BitConverter.ToUInt16(obj.tagID,0);
                //If we have a fragment then we search through this objects children to get the nodes contained within
                if (id == (int)CDXDatumID.kCDXObj_Fragment)
                {
                    foreach (CDXObject objChild in obj.childObjects)
                    {
                        //If the child is a node we start adding a new node to our list
                        UInt16 childID = BitConverter.ToUInt16(objChild.tagID,0);
                        if (childID == (int)CDXDatumID.kCDXObj_Node)
                        {
                            //in a node we look for a few necessary objects, 2d position, z ordering, foreground color and stereochemistry
                            STNode newNode = new STNode();
                            foreach (Property prop in objChild.childProperties)
                            {
                                UInt16 propID = BitConverter.ToUInt16(prop.tagID, 0);
                                if (propID == (int)CDXDatumID.kCDXProp_Node_Element)
                                {
                                    //this property stores the atomic number of the atom in the node
                                    newNode.atom = BitConverter.ToInt16(prop.data, 0); //get int16 atom #
                                }
                                if (propID == (int)CDXDatumID.kCDXProp_2DPosition)
                                {
                                    newNode.position = Parse2DCoordinate(prop.data); //coordinate parsed
                                }
                                else if (propID == (int)CDXDatumID.kCDXProp_ZOrder)
                                {
                                    newNode.zOrder = BitConverter.ToInt16(prop.data, 0);
                                }
                                else if (propID == (int)CDXDatumID.kCDXProp_Atom_CIPStereochemistry)
                                {
                                    newNode.stereochem = prop.data[0]; //the stereochem data is simply one byte
                                }

                                
                            }
                            //Now we go through the objects to see if we have a fragment and a text block
                            foreach (CDXObject innerObject in objChild.childObjects)
                            {
                                //TO be added later
                            }
                            nodeList.Add(newNode);
                        }
                        else if (childID == (int)CDXDatumID.kCDXObj_Bond)
                        {
                            STBond newBond = new STBond();
                            //Now that we have a list of the nodes we can get a list of the bonds
                            foreach (Property prop in objChild.childProperties)
                            {
                                UInt16 propID = BitConverter.ToUInt16(prop.tagID, 0);
                                if (propID == (int)CDXDatumID.kCDXProp_Bond_Begin)
                                {
                                    newBond.beginID = BitConverter.ToUInt32(prop.data, 0);
                                }
                                else if (propID == (int)CDXDatumID.kCDXProp_Bond_End)
                                {
                                    newBond.endID = BitConverter.ToUInt32(prop.data, 0);
                                }
                                else if (propID == (int)CDXDatumID.kCDXProp_Bond_Order)
                                {
                                    newBond.bondOrder = BitConverter.ToUInt16(prop.data, 0);
                                }
                            }
                            bondList.Add(newBond);

                        }
                    }
                }
               

            }
            Console.WriteLine("Read in " + nodeList.Count + " nodes.");
            foreach (STNode node in nodeList)
            {
                string s = string.Empty;
                if(node.atom == 6)
                    s = "Atom :" + node.atom+" Position :"+ node.position[0].coordinate + ","+ node.position[1].coordinate;
                else
                    s = "Atom :" + node.atom+" Position :"+ node.position[0].coordinate + ","+ node.position[1].coordinate;

                Console.WriteLine(s);
            }
            foreach (STBond bond in bondList)
            {
                string s = string.Empty;
                if (bond.bondOrder != 1)
                {
                    s = "Bond between " + bond.beginID + " and " + bond.endID + " is a double bond";
                }
                else
                {
                    s = "Bond between " + bond.beginID + " and " + bond.endID + " is a single bond";
                }
                Console.WriteLine(s);
            }
            Console.Read();
        }
    
    }
}
