using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using Newtonsoft.Json;

namespace IceBlink2
{
    /*
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EventObjEditorReturnObject
    {
        [JsonIgnore]
        public ParentForm prntForm;
        
        private TriggerType eventType = TriggerType.None;
        private string filenameOrTag = "none";
        private Point transPoint = new Point(0, 0);
        private string parm1 = "none";
        private string parm2 = "none";
        private string parm3 = "none";
        private string parm4 = "none";

        [DescriptionAttribute("Event Type chosen for these parameters"), ReadOnly(true)]
        public TriggerType EventType
        {
            get { return eventType; }
            set { eventType = value; }
        }
        [DescriptionAttribute("Filename or Tag of the Event Type"), ReadOnly(true)]
        public string FilenameOrTag
        {
            get { return filenameOrTag; }
            set { filenameOrTag = value; }
        }
        [DescriptionAttribute("Transition to square of Area selected if Transition Type"), ReadOnly(true)]
        public Point TransPoint
        {
            get { return transPoint; }
            set { transPoint = value; }
        }
        [DescriptionAttribute("Parameter for the Script chosen if Event Type is Script"), ReadOnly(true)]
        public string Parm1
        {
            get { return parm1; }
            set { parm1 = value; }
        }
        [DescriptionAttribute("Parameter for the Script chosen if Event Type is Script"), ReadOnly(true)]
        public string Parm2
        {
            get { return parm2; }
            set { parm2 = value; }
        }
        [DescriptionAttribute("Parameter for the Script chosen if Event Type is Script"), ReadOnly(true)]
        public string Parm3
        {
            get { return parm3; }
            set { parm3 = value; }
        }
        [DescriptionAttribute("Parameter for the Script chosen if Event Type is Script"), ReadOnly(true)]
        public string Parm4
        {
            get { return parm4; }
            set { parm4 = value; }
        }

        public EventObjEditorReturnObject()
        {
        }
        public override string ToString()
        {
            return FilenameOrTag;
        }
    }*/
}
