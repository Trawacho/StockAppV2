using StockApp.Core.Factories;
using StockApp.Core.Turnier;
using StockApp.UI.com;
using StockApp.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.UI.Stores;

public interface ITurnierStore
{
    ITurnier Turnier { get; set; }
    event EventHandler CurrentTurnierChanged;
    event EventHandler FileNameChanged;
    void Load();
    void Load(string fullFileName);

    void Save();
    void SaveAs();
    string FileName { get; }
    bool IsDuty();
    /// <summary>
    /// Maximale Anzahl an Teams, die in einer Gruppe erstellt werden kann. <br></br>
    /// Wert wird aus den verfügbaren Spielplänen ermittelt
    /// </summary>
    int MaxCountOfTeams { get; }

    IEnumerable<IVerein> TemplateVereine { get; }

}


public class TurnierStore : ITurnierStore
{
    private ITurnier _turnier;
    private readonly IXmlFileService _xmlFileService;

    public event EventHandler CurrentTurnierChanged;
    public event EventHandler FileNameChanged;

    private void XmlFullFileNameChanged(object sender, EventArgs e)
    {
        var handler = FileNameChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }


    public TurnierStore(IEnumerable<IVerein> templateVereine)
    {
        Turnier = Core.Turnier.Turnier.Create();
        _xmlFileService = new XmlFileService(Turnier);
        _xmlFileService.FullFilePathChanged += XmlFullFileNameChanged;
        MaxCountOfTeams = GamePlanFactory.LoadAllGameplans().Select(t => t.Teams).Max();
        TemplateVereine = templateVereine;
    }


    public ITurnier Turnier
    {
        get => _turnier;
        set
        {
            _turnier = value;
            CurrentTurnierChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public string FileName => _xmlFileService.FullFilePath;

    public int MaxCountOfTeams { get; init; }

    public IEnumerable<IVerein> TemplateVereine { get; init; }

    public void Load() => _xmlFileService.Load(ref _turnier);
    public void Load(string fullFileName) => _xmlFileService.Load(ref _turnier, fullFileName);
    public void Save() => _xmlFileService.Save(ref _turnier);
    public void SaveAs() => _xmlFileService.SaveAs(ref _turnier);
    public bool IsDuty() => _xmlFileService.IsDuty(ref _turnier);

}




