// Copyright © 2025 Always Active Technologies PTY Ltd

using System.Xml.Serialization;
using SQLite;

namespace TechAptV1.Client.Models;

[Table("Number")]
[XmlType("Number")]
public class Number
{
    [Column("Value")]
    [XmlAttribute("Value")]
    public int Value { get; set; }

    [Column("IsPrime")]
    [XmlAttribute("IsPrime")]
    public int IsPrime { get; set; }
}
