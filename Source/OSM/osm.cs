namespace Mapper.OSM
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class osm
    {

        private string noteField;
        private bool uploadField;

        private osmMeta metaField;

        private osmBounds boundsField;

        private osmNode[] nodeField;

        private osmWay[] wayField;

        private osmRelation[] relationField;

        private decimal versionField;

        private string generatorField;

        /// <upload/>
        public bool upload
        {
            get => this.uploadField;
            set => this.uploadField = value;
        }


        /// <remarks/>
        public string note
        {
            get => this.noteField;
            set => this.noteField = value;
        }

        /// <remarks/>
        public osmMeta meta
        {
            get => this.metaField;
            set => this.metaField = value;
        }

        /// <remarks/>
        public osmBounds bounds
        {
            get => this.boundsField;
            set => this.boundsField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("node")]
        public osmNode[] node
        {
            get => this.nodeField;
            set => this.nodeField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("way")]
        public osmWay[] way
        {
            get => this.wayField;
            set => this.wayField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("relation")]
        public osmRelation[] relation
        {
            get => this.relationField;
            set => this.relationField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get => this.versionField;
            set => this.versionField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string generator
        {
            get => this.generatorField;
            set => this.generatorField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmMeta
    {

        private System.DateTime osm_baseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime osm_base
        {
            get => this.osm_baseField;
            set => this.osm_baseField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmBounds
    {

        private decimal minlatField;

        private decimal minlonField;

        private decimal maxlatField;

        private decimal maxlonField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal minlat
        {
            get => this.minlatField;
            set => this.minlatField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal minlon
        {
            get => this.minlonField;
            set => this.minlonField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal maxlat
        {
            get => this.maxlatField;
            set => this.maxlatField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal maxlon
        {
            get => this.maxlonField;
            set => this.maxlonField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmNode
    {

        private osmNodeTag[] tagField;

        private long idField;

        private decimal latField;

        private decimal lonField;

        private int versionField;

        private System.DateTime timestampField;

        private uint changesetField;

        private uint uidField;

        private string userField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag")]
        public osmNodeTag[] tag
        {
            get => this.tagField;
            set => this.tagField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long id
        {
            get => this.idField;
            set => this.idField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lat
        {
            get => this.latField;
            set => this.latField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lon
        {
            get => this.lonField;
            set => this.lonField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int version
        {
            get => this.versionField;
            set => this.versionField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get => this.timestampField;
            set => this.timestampField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint changeset
        {
            get => this.changesetField;
            set => this.changesetField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint uid
        {
            get => this.uidField;
            set => this.uidField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string user
        {
            get => this.userField;
            set => this.userField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmNodeTag
    {

        private string kField;

        private string vField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string k
        {
            get => this.kField;
            set => this.kField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string v
        {
            get => this.vField;
            set => this.vField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmWay
    {

        private osmWayND[] ndField;

        private osmWayTag[] tagField;

        private uint idField;

        private int versionField;

        private System.DateTime timestampField;

        private uint changesetField;

        private uint uidField;

        private string userField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("nd")]
        public osmWayND[] nd
        {
            get => this.ndField;
            set => this.ndField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag")]
        public osmWayTag[] tag
        {
            get => this.tagField;
            set => this.tagField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint id
        {
            get => this.idField;
            set => this.idField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int version
        {
            get => this.versionField;
            set => this.versionField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get => this.timestampField;
            set => this.timestampField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint changeset
        {
            get => this.changesetField;
            set => this.changesetField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint uid
        {
            get => this.uidField;
            set => this.uidField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string user
        {
            get => this.userField;
            set => this.userField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmWayND
    {

        private long refField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long @ref
        {
            get => this.refField;
            set => this.refField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmWayTag
    {

        private string kField;

        private string vField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string k
        {
            get => this.kField;
            set => this.kField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string v
        {
            get => this.vField;
            set => this.vField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmRelation
    {

        private osmRelationMember[] memberField;

        private osmRelationTag[] tagField;

        private uint idField;

        private int versionField;

        private System.DateTime timestampField;

        private uint changesetField;

        private uint uidField;

        private string userField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("member")]
        public osmRelationMember[] member
        {
            get => this.memberField;
            set => this.memberField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag")]
        public osmRelationTag[] tag
        {
            get => this.tagField;
            set => this.tagField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint id
        {
            get => this.idField;
            set => this.idField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int version
        {
            get => this.versionField;
            set => this.versionField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get => this.timestampField;
            set => this.timestampField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint changeset
        {
            get => this.changesetField;
            set => this.changesetField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint uid
        {
            get => this.uidField;
            set => this.uidField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string user
        {
            get => this.userField;
            set => this.userField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmRelationMember
    {

        private string typeField;

        private long refField;

        private string roleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get => this.typeField;
            set => this.typeField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long @ref
        {
            get => this.refField;
            set => this.refField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role
        {
            get => this.roleField;
            set => this.roleField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class osmRelationTag
    {

        private string kField;

        private string vField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string k
        {
            get => this.kField;
            set => this.kField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string v
        {
            get => this.vField;
            set => this.vField = value;
        }
    }


}
