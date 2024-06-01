using StockApp.UI.Enumerations;
using System;
using System.Xml;

namespace StockApp.UI.Settings;

public class TeamBewerbSettings : ISettingsSerializer
{

    public string Name => "TeamBewerb";

    public TeamBewerbSettings()
    {
        TeamBewerbInputMethod = TeamBewerbInputMethod.PerTeam;
        HasNamesOnScoreCard = true;
        HasOpponentOnScoreCard = true;
        HasSummarizedScoreCards = true;
    }

    public void ReadXML(XmlReader reader)
    {
        reader.ReadStartElement();

        while (reader.NodeType == XmlNodeType.Element)
        {
            switch (reader.Name)
            {
                case nameof(TeamBewerbInputMethod):
                    TeamBewerbInputMethod = (TeamBewerbInputMethod)Enum.Parse(typeof(TeamBewerbInputMethod), reader.ReadElementContentAsString());
                    break;
                case nameof(HasSummarizedScoreCards):
                    HasSummarizedScoreCards = XmlHelper.ParseBoolean(reader.ReadElementContentAsString());
                    break;
                case nameof(HasNamesOnScoreCard):
                    HasNamesOnScoreCard = XmlHelper.ParseBoolean(reader.ReadElementContentAsString());
                    break;
                case nameof(HasOpponentOnScoreCard):
                    HasOpponentOnScoreCard = XmlHelper.ParseBoolean(reader.ReadElementContentAsString());
                    break;
                default:
                    reader.ReadOuterXml();
                    break;
            }
        }

        reader.ReadEndElement();
    }

    public void WriteXML(XmlWriter writer)
    {
        writer.WriteElementString(nameof(TeamBewerbInputMethod), TeamBewerbInputMethod.ToString());
        writer.WriteElementString(nameof(HasSummarizedScoreCards), HasSummarizedScoreCards.ToString());
        writer.WriteElementString(nameof(HasNamesOnScoreCard), HasNamesOnScoreCard.ToString());
        writer.WriteElementString(nameof(HasOpponentOnScoreCard), HasOpponentOnScoreCard.ToString());
    }

    /// <summary>
    /// Eingabeoption
    /// </summary>
    public TeamBewerbInputMethod TeamBewerbInputMethod { get; set; }

    /// <summary>
    /// Druckoption: Wertungskarte wird bei mehreren Runden zusammengefasst 
    /// </summary>
    public bool HasSummarizedScoreCards { get; set; }

    /// <summary>
    /// Druckoption: Wertungskarte mit Mannschaftsname (nicht Name der Gegener!!!)
    /// </summary>
    public bool HasNamesOnScoreCard { get; set; }

    /// <summary>
    /// Druckoption: Wertungskarte mit Namen der Gegner
    /// </summary>
    public bool HasOpponentOnScoreCard { get; set; }

}
