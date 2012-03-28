using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynType.File_ReadWrite
{
    enum PropertyTypes
    {
        String,
        FontStyle,
        Font,
        Coordinate,
        Point2D,
        Rectangle
    }
    class Property
    {
        public byte[] tagID; // 2 bytes
        public string type;
        public byte[] data;
        public CDXObject parent;
        public Property(byte[] id, byte[] data)
        {
            this.tagID = id;
            this.data = data;
        }
    }
    struct CDXString
    {
        //Number of font styles we have
        UInt16 styleRunCount;
        //The font styles used
        CDXFontStyle[] fontStyleRun;
        //The text contained in the object
        public string text;


        public CDXString(UInt16 styleRunCount, CDXFontStyle[] fontStyleRun, string text, string type)
        {
            this.styleRunCount = styleRunCount;
            this.fontStyleRun = fontStyleRun;
            this.text = text;
        }

    }
    /*The font style is an 8 byte structure containg information on the font style being used in a CDXString  * */
    struct CDXFontStyle
    {
        UInt16 fontIndex;
        /* Font Types
         * 0 : plain, 1: bold, 2: italic, 4: underline, (8: outline, 10: shadow * for MAC), 20: subscript, 40: superscript, 60 formula (both sub and super depending on formula) 
         * sub, super and formula are mutually exclusive, the others can be combined to give * */
        UInt16 fontType;
        UInt16 fontSize; //measured in 20ths of a point
        UInt16 fontColor;

        public CDXFontStyle(UInt16 fontIndex, UInt16 fontType, UInt16 fontSize, UInt16 fontColor, string type)
        {
            this.fontIndex = fontIndex;
            this.fontType = fontType;
            this.fontSize = fontSize;
            this.fontColor = fontColor;
        }
    }
    struct CDXCoordinate
    {
        public Int32 coordinate;
    }
    struct CDXRectangle
    {
        CDXCoordinate top, left, bottom, right; //in this order
    }
    class STNode
    {
        public CDXCoordinate[] position;
        public UInt32 id;
        public Int16 zOrder;
        public byte stereochem; // CDXATomCIPTYPE
        public bool isAtom; //a node can be an atom or a fragment
        public int atom;
        public STNode()
        {
            atom = 6;
        }

    }
    class STBond
    {
        public UInt32 beginID; //the id of the start node
        public UInt32 endID; //the id of the end node
        public byte stereochem; //Bond_CIPStereochemistry enumeration 0: undetermined, 1 symmetric, 2: E , 3:Z
        public UInt16 bondOrder;

        public STBond()
        {
            stereochem = 0;
            bondOrder = 1;
        }
    }

    class CDXObject
    {
        public byte[] tagID; // 2 bytes
        public byte[] objectID; //4 bytes
        public CDXObject parentObject;
        public List<CDXObject> childObjects;
        public List<Property> childProperties;
        public CDXObject()
        {
            childObjects = new List<CDXObject>();
            childProperties = new List<Property>();
        }
    }
}
