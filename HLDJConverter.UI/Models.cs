using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter.UI
{
    public sealed class Settings: Notifiable
    {
        private string mOutputFolder;
        public string OutputFolder
        {
            get { return mOutputFolder; }
            set { SetField(ref mOutputFolder, value); }
        }
    }

    public sealed class ConversionJob: Notifiable
    {
        private string mDisplayName;
        public string DisplayName
        {
            get { return mDisplayName; }
            set { SetField(ref mDisplayName, value); }
        }
        
        private string mStatus;
        public string Status
        {
            get { return mStatus; }
            set { SetField(ref mStatus, value); }
        }
    }
}
