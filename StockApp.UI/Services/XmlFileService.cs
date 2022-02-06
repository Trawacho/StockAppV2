using Microsoft.Win32;
using StockApp.Core.Turnier;
using StockApp.XML;
using System;

namespace StockApp.UI.Services
{
    public interface IXmlFileService
    {
        string FullFilePath { get; set; }
        void Load(ref ITurnier turnier);
        void Save(ref ITurnier turnier);
        void SaveAs(ref ITurnier turnier);
        bool IsDuty(ref ITurnier turnier);

        event EventHandler FullFilePathChanged;
    }
    internal class XmlFileService : IXmlFileService
    {
        const string _fileFilter = "StockApp files (*.skmr)|*.skmr|All files (*.*)|*.*";
        private string _xmlTurnierString = string.Empty;

        public event EventHandler FullFilePathChanged;
        protected virtual void RaiseFullFilePathChanged()
        {
            var handler = FullFilePathChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public XmlFileService(ITurnier turnier)
        {
            _xmlTurnierString = SavingModule.ConvertToXml(ref turnier);
        }



        private string _fullFilePath;

        public string FullFilePath
        {
            get { return _fullFilePath; }
            set { _fullFilePath = value; RaiseFullFilePathChanged(); }
        }

        public void SaveAs(ref ITurnier turnier)
        {
            var fileDialog = new SaveFileDialog
            {
                Filter = _fileFilter,
                FilterIndex = 1
            };
            if (fileDialog.ShowDialog() == true)
            {
                FullFilePath = fileDialog.FileName;
                SavingModule.Save(ref turnier, FullFilePath);
                _xmlTurnierString = SavingModule.ConvertToXml(ref turnier);
            }
        }

        public void Save(ref ITurnier turnier)
        {
            if (FullFilePath == null)
            {
                SaveAs(ref turnier);
            }
            else
            {
                SavingModule.Save(ref turnier, FullFilePath);
                _xmlTurnierString = SavingModule.ConvertToXml(ref turnier);
            }
        }

        public void Load(ref ITurnier turnier)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = _fileFilter,
                FilterIndex = 1
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FullFilePath = openFileDialog.FileName;
                LoadingModule.Load(ref turnier, FullFilePath);
                _xmlTurnierString = SavingModule.ConvertToXml(ref turnier);
            }
        }

        public bool IsDuty(ref ITurnier turnier) => !string.Equals(SavingModule.ConvertToXml(ref turnier), _xmlTurnierString);
    }
}
