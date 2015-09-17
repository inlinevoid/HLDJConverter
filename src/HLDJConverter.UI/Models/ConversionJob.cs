using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter.UI.Models
{
    public sealed class ConversionItem: Notifiable
    {
        private string mTitle;
        public string Title
        {
            get { return mTitle; }
            set { SetField(ref mTitle, value); }
        }
        
        private string mStatusDetails;
        public string StatusDetails
        {
            get {return mStatusDetails; }
            set {SetField(ref mStatusDetails, value); UpdateStatusDisplay(); }
        }

        private ConversionJobStatus mStatus;
        public ConversionJobStatus Status
        {
            get {return mStatus; }
            set {SetField(ref mStatus, value); UpdateStatusDisplay(); }
        }

        private string mStatusDisplay;
        public string StatusDisplay
        {
            get { return mStatusDisplay; }
            set { SetField(ref mStatusDisplay, value); }
        }
        
        private void UpdateStatusDisplay()
        {
            StatusDisplay = Status.ToString();
            if(!String.IsNullOrEmpty(StatusDetails))
            {
                StatusDisplay += $": {StatusDetails}";
            }
        }
    }

    public enum ConversionJobStatus
    {
        Resolving,
        Downloading,
        DownloadingFinished,
        Converting,
        ConvertingFinished,
        Done,
        Error,
    }
}
